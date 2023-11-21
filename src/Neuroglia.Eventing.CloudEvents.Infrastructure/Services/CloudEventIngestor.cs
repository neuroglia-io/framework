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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neuroglia.Eventing.CloudEvents.Infrastructure.Configuration;
using Neuroglia.Mediation;
using Neuroglia.Reactive;
using Neuroglia.Serialization;
using System.Text;

namespace Neuroglia.Eventing.CloudEvents.Infrastructure.Services;

/// <summary>
/// Represents the service used to ingest incoming <see cref="CloudEvent"/>s
/// </summary>
public class CloudEventIngestor
    : ICloudEventIngestor
{

    bool _disposed;

    /// <summary>
    /// Initializes a new <see cref="CloudEventIngestor"/>
    /// </summary>
    /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
    /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
    /// <param name="serializer">The service used to serialize and deserialize <see cref="CloudEvent"/>s</param>
    /// <param name="cloudEventBus">The service used to manage incoming and outgoing <see cref="CloudEvent"/>s</param>
    /// <param name="cloudEventIngestionOptions">The service used to access the current <see cref="Configuration.CloudEventIngestionOptions"/></param>
    public CloudEventIngestor(IServiceProvider serviceProvider, ILoggerFactory loggerFactory, IJsonSerializer serializer, ICloudEventBus cloudEventBus, IOptions<CloudEventIngestionOptions> cloudEventIngestionOptions)
    {
        this.ServiceProvider = serviceProvider;
        this.Logger = loggerFactory.CreateLogger(this.GetType());
        this.Serializer = serializer;
        this.CloudEventBus = cloudEventBus;
        this.CloudEventIngestionOptions = cloudEventIngestionOptions.Value;
    }

    /// <summary>
    /// Gets the current <see cref="IServiceProvider"/>
    /// </summary>
    protected IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Gets the service used to perform logging
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Gets the service used to serialize and deserialize <see cref="CloudEvent"/>s
    /// </summary>
    protected IJsonSerializer Serializer { get; }

    /// <summary>
    /// Gets the service used to manage incoming and outgoing <see cref="CloudEvent"/>s
    /// </summary>
    protected ICloudEventBus CloudEventBus { get; }

    /// <summary>
    /// Gets the options used to configure how to ingest incoming <see cref="CloudEvent"/>s
    /// </summary>
    protected CloudEventIngestionOptions CloudEventIngestionOptions { get; }

    /// <summary>
    /// Gets the <see cref="CloudEventIngestor"/>'s <see cref="System.Threading.CancellationTokenSource"/>
    /// </summary>
    protected CancellationTokenSource CancellationTokenSource { get; } = new();

    /// <inheritdoc/>
    public virtual Task StartAsync(CancellationToken cancellationToken) 
    {
        this.CloudEventBus.InputStream.SubscribeAsync(e => this.IngestAsync(e, this.CancellationTokenSource.Token), this.CancellationTokenSource.Token);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual async Task IngestAsync(CloudEvent e, CancellationToken cancellationToken = default)
    {
        var ingestionConfiguration = this.CloudEventIngestionOptions.Events.FirstOrDefault(c => c.Type.Equals(e.Type, StringComparison.OrdinalIgnoreCase));
        if (ingestionConfiguration == null)
        {
            this.Logger.LogDebug("No ingestion configuration found for cloud event of type '{cloudEventType}'. The cloud event will not be ingested but will be cked.", e.Type);
            return;
        }
        try
        {
            await using var scope = this.ServiceProvider.CreateAsyncScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var notification = string.IsNullOrWhiteSpace(e.DataBase64) ? e.Data : this.Serializer.Deserialize(Encoding.UTF8.GetString(Convert.FromBase64String(e.DataBase64)), ingestionConfiguration.DataType);
            if (notification == null)
            {
                this.Logger.LogWarning("The cloud event of type '{cloudEventType}' with id '{cloudEventId}' does not have data, and will therefore not be ingested, but will be acked.", e.Type, e.Id);
                return;
            }
            else if (ingestionConfiguration.DataType != notification.GetType()) notification = this.Serializer.Convert(notification, ingestionConfiguration.DataType);
            await mediator.PublishAsync((dynamic)notification!, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this.Logger.LogError("Failed to ingest the cloud event of type '{cloudEventType}' with id '{cloudEventId}': {ex}", e.Type, e.Id, ex);
        }
    }

    /// <inheritdoc/>
    public virtual Task StopAsync(CancellationToken cancellationToken) => Task.Run(this.CancellationTokenSource.Cancel, cancellationToken);

    /// <summary>
    /// Disposes of the <see cref="CloudEventIngestor"/>
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
