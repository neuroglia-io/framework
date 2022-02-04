using CloudNative.CloudEvents;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neuroglia.Data;
using Neuroglia.Eventing.Configuration;
using Polly;
using System.Net.Mime;
using System.Reactive.Subjects;
using System.Threading.Channels;

namespace Neuroglia.Eventing.Services
{

    /// <summary>
    /// Represents the default implementation of the <see cref="ICloudEventBus"/> interface
    /// </summary>
    public class CloudEventBus
        : BackgroundService, ICloudEventBus, IDisposable
    {

        /// <summary>
        /// Initializes a new <see cref="CloudEventBus"/>
        /// </summary>
        /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
        /// <param name="logger">The service used to perform logging</param>
        /// <param name="options">The service used to access the current <see cref="CloudEventBusOptions"/></param>
        /// <param name="formatter">The service used to format <see cref="CloudEvent"/>s</param>
        /// <param name="stream">The <see cref="Subject{T}"/> used to observe consumed <see cref="CloudEvent"/>s</param>
        /// <param name="httpClientFactory">The service used to create <see cref="System.Net.Http.HttpClient"/>s</param>
        public CloudEventBus(IServiceProvider serviceProvider, ILogger<CloudEventBus> logger, IOptions<CloudEventBusOptions> options, CloudEventFormatter formatter, ISubject<CloudEvent> stream, IHttpClientFactory httpClientFactory)
        {
            this.Logger = logger;
            this.Options = options.Value;
            this.Formatter = formatter;
            this.Stream = stream;
            this.HttpClient = httpClientFactory.CreateClient(nameof(CloudEventBus));
            this.Outbox = serviceProvider.GetService<IRepository<CloudEventOutboxEntry, string>>();
        }

        /// <summary>
        /// Gets the service used to perform logging
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the current <see cref="CloudEventBusOptions"/>
        /// </summary>
        protected CloudEventBusOptions Options { get; }

        /// <summary>
        /// Gets the service used to format <see cref="CloudEvent"/>s
        /// </summary>
        protected CloudEventFormatter Formatter { get; }

        /// <summary>
        /// Gets the <see cref="Subject{T}"/> used to observe consumed <see cref="CloudEvent"/>s
        /// </summary>
        protected ISubject<CloudEvent> Stream { get; }

        /// <summary>
        /// Gets the <see cref="System.Net.Http.HttpClient"/> used to publish <see cref="CloudEvent"/>s
        /// </summary>
        protected HttpClient HttpClient { get; }

        /// <summary>
        /// Gets the <see cref="IRepository{TEntity, TKey}"/> used to manage pending outbound <see cref="CloudEvent"/>s
        /// </summary>
        protected IRepository<CloudEventOutboxEntry, string>? Outbox { get; }

        /// <summary>
        /// Gets the <see cref="Channel{T}"/> used to enqueue outbound <see cref="CloudEvent"/>s
        /// </summary>
        protected Channel<CloudEvent> Queue { get; private set; } = null!;

        /// <summary>
        /// Gets the <see cref="CloudEventBus"/>'s <see cref="CancellationTokenSource"/>
        /// </summary>
        protected CancellationTokenSource CancellationTokenSource { get; private set; } = null!;

        /// <inheritdoc/>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (this.Options.QueueCapacity.HasValue)
                this.Queue = Channel.CreateBounded<CloudEvent>(this.Options.QueueCapacity.Value);
            else
                this.Queue = Channel.CreateUnbounded<CloudEvent>();
            this.CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
            if(this.Outbox != null)
            {
                foreach (CloudEventOutboxEntry entry in await this.Outbox.ToListAsync(this.CancellationTokenSource.Token))
                {
                    using MemoryStream body = new(entry.Data);
                    CloudEvent e = await this.Formatter.DecodeStructuredModeMessageAsync(body, new(entry.ContentType), null);
                    await this.Queue.Writer.WriteAsync(e, this.CancellationTokenSource.Token);
                }
            }
            _ = Task.Run(() => this.DequeueAndPublishPendingEventsAsync().ConfigureAwait(false));
        }

        /// <inheritdoc/>
        public virtual async Task PublishAsync(CloudEvent e, CancellationToken cancellationToken = default)
        {
            await this.EnqueueAsync(e, cancellationToken);
        }

        /// <inheritdoc/>
        public virtual IDisposable Subscribe(IObserver<CloudEvent> observer)
        {
            return this.Stream.Subscribe(observer);
        }

        /// <summary>
        /// Enqueues the specified <see cref="CloudEvent"/>
        /// </summary>
        /// <param name="e">The <see cref="CloudEvent"/> to enqueue</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        protected virtual async Task EnqueueAsync(CloudEvent e, CancellationToken cancellationToken)
        {
            CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            if(this.Outbox != null)
            {
                var buffer = this.Formatter.EncodeStructuredModeMessage(e, out ContentType contentType);
                await this.Outbox.AddAsync(new(e.Id!, buffer.ToArray(), contentType), cancellationTokenSource.Token);
                await this.Outbox.SaveChangesAsync(cancellationTokenSource.Token);
            }
            await this.Queue.Writer.WriteAsync(e, cancellationTokenSource.Token);
        }

        /// <summary>
        /// Dequeues and publishes pending outbound <see cref="CloudEvent"/>s
        /// </summary>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        protected virtual async Task DequeueAndPublishPendingEventsAsync()
        {
            do
            {
                try
                {
                    CloudEvent e = await this.Queue.Reader.ReadAsync(this.CancellationTokenSource.Token);
                    if (e == null)
                        continue;
                    bool published = false;
                    var buffer = this.Formatter.EncodeStructuredModeMessage(e, out ContentType contentType);
                    do
                    {
                        var retryPolicy = Policy.Handle<Exception>()
                            .WaitAndRetryForeverAsync(retryAttempt => TimeSpan.FromSeconds(retryAttempt));
                        await retryPolicy.ExecuteAsync(async () =>
                        {
                            try
                            {
                                using ByteArrayContent content = new(buffer.ToArray());
                                content.Headers.ContentType = new(contentType.MediaType);
                                using HttpRequestMessage request = new(HttpMethod.Post, "") { Content = content };
                                using HttpResponseMessage response = await this.HttpClient.SendAsync(request, this.CancellationTokenSource.Token);
                                response.EnsureSuccessStatusCode();
                                published = true;
                            }
                            catch (Exception ex)
                            {
                                this.Logger.LogError("An error occured while posting a cloud events to the broker: {ex}", ex.ToString());
                                throw;
                            }
                        });
                    }
                    while (!this.CancellationTokenSource.IsCancellationRequested && !published);
                    if (this.Outbox != null)
                    {
                        await this.Outbox.RemoveAsync(e.Id!, this.CancellationTokenSource.Token);
                        await this.Outbox.SaveChangesAsync(this.CancellationTokenSource.Token);
                    }
                }
                catch (Exception ex)
                {
                    this.Logger.LogError("An error occured while dequeuing and publishing pending cloud events: {ex}", ex.ToString());
                }
            }
            while (!this.CancellationTokenSource.IsCancellationRequested);
        }

        private bool _Disposed;
        /// <summary>
        /// Disposes of the <see cref="CloudEventBus"/>
        /// </summary>
        /// <param name="disposing">A boolean indicating whether or not the <see cref="CloudEventBus"/> is being disposed of</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._Disposed)
            {
                if (disposing)
                    this.CancellationTokenSource?.Dispose();
                this._Disposed = true;
            }
        }

        /// <inheritdoc/>
        public override void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

    }

}
