// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License"),
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neuroglia.Eventing.CloudEvents.Infrastructure.Configuration;
using Neuroglia.Reactive;
using Neuroglia.Serialization;
using Polly;
using Polly.CircuitBreaker;
using System.Net.Http.Headers;
using System.Text;

namespace Neuroglia.Eventing.CloudEvents.Infrastructure.Services;

/// <summary>
/// Represents an <see cref="ICloudEventPublisher"/> implementation that uses <see cref="System.Net.Http.HttpClient"/>s to publish <see cref="CloudEvent"/>s 
/// </summary>
public class CloudEventPublisher
    : ICloudEventPublisher
{

    bool _disposed;

    /// <summary>
    /// Initializes a new <see cref="CloudEventPublisher"/>
    /// </summary>
    /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
    /// <param name="serializer">The service used to serialize <see cref="CloudEvent"/>s to JSON</param>
    /// <param name="cloudEventBus">The service used to manage outgoing and incoming <see cref="CloudEvent"/>s</param>
    /// <param name="cloudEventDispatchOptions">The service used to access the current <see cref="Configuration.CloudEventPublishOptions"/></param>
    /// <param name="httpClient">The service used to perform <see cref="HttpRequestMessage"/>s</param>
    public CloudEventPublisher(ILoggerFactory loggerFactory, IJsonSerializer serializer, ICloudEventBus cloudEventBus, IOptions<CloudEventPublishOptions> cloudEventDispatchOptions, HttpClient httpClient)
    {
        this.Logger = loggerFactory.CreateLogger(this.GetType());
        this.Serializer = serializer;
        this.CloudEventBus = cloudEventBus;
        this.CloudEventDispatchOptions = cloudEventDispatchOptions.Value;
        this.HttpClient = httpClient;
    }

    /// <summary>
    /// Gets the service used to perform logging
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Gets the service used to serialize <see cref="CloudEvent"/>s to JSON
    /// </summary>
    protected IJsonSerializer Serializer { get; }

    /// <summary>
    /// Gets the service used to manage outgoing and incoming <see cref="CloudEvent"/>s
    /// </summary>
    protected ICloudEventBus CloudEventBus { get; }

    /// <summary>
    /// Gets the options used to configure the application's <see cref="CloudEvent"/>s
    /// </summary>
    protected CloudEventPublishOptions CloudEventDispatchOptions { get; }

    /// <summary>
    /// Gets the service used to perform <see cref="HttpRequestMessage"/>s
    /// </summary>
    protected HttpClient HttpClient { get; }

    /// <summary>
    /// Gets the <see cref="CloudEventPublisher"/>'s <see cref="System.Threading.CancellationTokenSource"/>
    /// </summary>
    protected CancellationTokenSource CancellationTokenSource { get; } = new();

    /// <inheritdoc/>
    public virtual Task StartAsync(CancellationToken cancellationToken)
    {
        this.CloudEventBus.OutputStream.SubscribeAsync(e => this.PublishAsync(e, this.CancellationTokenSource.Token), this.CancellationTokenSource.Token);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Publishes the specified <see cref="CloudEvent"/>
    /// </summary>
    /// <param name="e">The <see cref="CloudEvent"/> to publish</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    public virtual async Task PublishAsync(CloudEvent e, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(e);

        var exceptionPredicate = (HttpRequestException ex) => this.CloudEventDispatchOptions.Retry.StatusCodes == null || this.CloudEventDispatchOptions.Retry.StatusCodes.Count == 0 || (ex.StatusCode.HasValue && ex.StatusCode.HasValue && this.CloudEventDispatchOptions.Retry.StatusCodes.Contains((int)ex.StatusCode.Value));
        AsyncCircuitBreakerPolicy? circuitBreakerPolicy = this.CloudEventDispatchOptions.Retry.CircuitBreaker == null ? null : Policy.Handle(exceptionPredicate)
                .CircuitBreakerAsync(this.CloudEventDispatchOptions.Retry.CircuitBreaker.BreakAfter, this.CloudEventDispatchOptions.Retry.CircuitBreaker.BreakDuration);

        AsyncPolicy retryPolicy = this.CloudEventDispatchOptions.Retry.MaxAttempts.HasValue ?
            Policy.Handle(exceptionPredicate)
                .WaitAndRetryAsync(this.CloudEventDispatchOptions.Retry.MaxAttempts.Value, this.CloudEventDispatchOptions.Retry.BackoffDuration.ForAttempt)
            : Policy.Handle(exceptionPredicate)
                .WaitAndRetryForeverAsync(this.CloudEventDispatchOptions.Retry.BackoffDuration.ForAttempt);

        retryPolicy = circuitBreakerPolicy == null ? retryPolicy : retryPolicy.WrapAsync(circuitBreakerPolicy);
        await retryPolicy.ExecuteAsync(token => this.SendCloudEventAsync(e, token), cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public virtual Task StopAsync(CancellationToken cancellationToken) => Task.Run(this.CancellationTokenSource.Cancel, cancellationToken);

    /// <summary>
    /// Sends the specified <see cref="CloudEvent"/> to the configured sink <see cref="Uri"/>
    /// </summary>
    /// <param name="e">The <see cref="CloudEvent"/> to send</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual async Task SendCloudEventAsync(CloudEvent e, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(e);

        var uri = this.CloudEventDispatchOptions.Sink;
        var json = this.Serializer.SerializeToText(e);
        using var requestContent = new StringContent(json, Encoding.UTF8, new MediaTypeHeaderValue(CloudEventContentType.Json));
        using var request = new HttpRequestMessage(HttpMethod.Post, uri) { Content = requestContent };
        using var response = await this.HttpClient.SendAsync(request, this.CancellationTokenSource.Token).ConfigureAwait(false);
        var responseContent = await response.Content.ReadAsStringAsync(this.CancellationTokenSource.Token).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode) this.Logger.LogError("An error occured while publishing cloud event with id '{cloudEventId}': the remote server responded with a non-success status code '{statusCode}'./r/nDetails: {details}", e.Id, response.StatusCode, responseContent);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Disposes of the <see cref="CloudEventPublisher"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the object is being disposed of</param>
    protected virtual void Dispose(bool disposing)
    {
        if (this._disposed) return;
        if (disposing) this.CancellationTokenSource.Dispose();
        this._disposed = true;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

}
