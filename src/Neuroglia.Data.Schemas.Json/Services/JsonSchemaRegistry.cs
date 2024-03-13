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

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neuroglia.Data.Schemas.Json.Configuration;

namespace Neuroglia.Data.Schemas.Json;

/// <summary>
/// Represents the default implementation of the <see cref="IJsonSchemaRegistry"/>
/// </summary>
public class JsonSchemaRegistry
    : IHostedService, IJsonSchemaRegistry, IDisposable
{
    private bool _disposed;

    /// <summary>
    /// Initializes a new <see cref="JsonSchemaRegistry"/>
    /// </summary>
    /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
    /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
    /// <param name="options">The service used to access the current current <see cref="JsonSchemaRegistryOptions"/></param>
    /// <param name="schemaResolver">The service used to manage <see cref="JsonSchema"/>s</param>
    /// <param name="httpClientFactory">The service used to create <see cref="System.Net.Http.HttpClient"/>s</param>
    public JsonSchemaRegistry(IServiceProvider serviceProvider, ILoggerFactory loggerFactory, IOptions<JsonSchemaRegistryOptions> options, IJsonSchemaResolver schemaResolver, IHttpClientFactory httpClientFactory)
    {
        this.ServiceProvider = serviceProvider;
        this.Logger = loggerFactory.CreateLogger(GetType());
        this.Options = options.Value;
        this.SchemaResolver = schemaResolver;
        this.HttpClient = httpClientFactory.CreateClient();
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
    /// Gets the current <see cref="JsonSchemaRegistryOptions"/>
    /// </summary>
    protected JsonSchemaRegistryOptions Options { get; }

    /// <summary>
    /// Gets the service used to resolve <see cref="JsonSchema"/>s
    /// </summary>
    protected IJsonSchemaResolver SchemaResolver { get; }

    /// <summary>
    /// Gets the service used to perform HTTP requests
    /// </summary>
    protected HttpClient HttpClient { get; private set; }

    /// <summary>
    /// Gets the <see cref="JsonSchemaRegistry"/>'s <see cref="System.Threading.CancellationTokenSource"/>
    /// </summary>
    protected CancellationTokenSource CancellationTokenSource { get; private set; } = null!;

    /// <inheritdoc/>
    public virtual async Task StartAsync(CancellationToken cancellationToken)
    {
        this.CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        if (this.Options.References == null || this.Options.References.Count == 0) return;
        foreach (var reference in this.Options.References)
        {
            try
            {
                var json = await this.HttpClient.GetStringAsync(reference.Uri, cancellationToken).ConfigureAwait(false);
                var schema = await this.SchemaResolver.ResolveSchemaAsync(JsonSchema.FromText(json), cancellationToken).ConfigureAwait(false);
                if(reference.Mutators != null && reference.Mutators.Count != 0)
                {
                    foreach (var mutator in reference.Mutators)
                    {
                        schema = await mutator(this.ServiceProvider, schema).ConfigureAwait(false);
                    }
                }
                SchemaRegistry.Global.Register(reference.Uri, schema);
            }
            catch (Exception ex)
            {
                this.Logger.LogError("An error occurred while retrieving the JSON Schema at '{uri}': {ex}", reference, ex);
            }
        }
        this.HttpClient.Dispose();
        this.HttpClient = null!;
    }

    /// <inheritdoc/>
    public virtual Task StopAsync(CancellationToken cancellationToken) => Task.Run(() => this.CancellationTokenSource?.Cancel(), cancellationToken);

    /// <inheritdoc/>
    public virtual JsonSchema? GetSchema(Uri uri) => (JsonSchema?)SchemaRegistry.Global.Get(uri);

    /// <summary>
    /// Disposes of the <see cref="JsonSchemaRegistry"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="JsonSchemaRegistry"/> has been disposed of</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!this._disposed)
        {
            if (disposing)
            {
                this.CancellationTokenSource?.Dispose();
                this.HttpClient?.Dispose();
            }
            this._disposed = true;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

}
