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

namespace Neuroglia.Data.Infrastructure.ResourceOriented.Services;

/// <summary>
/// Represents the service used to create <see cref="RedisDatabase"/>s
/// </summary>
public class RedisDatabaseFactory
    : IFactory<RedisDatabase>, IDisposable, IAsyncDisposable
{

    private bool _disposed;

    /// <summary>
    /// Initializes a new <see cref="RedisDatabaseFactory"/>
    /// </summary>
    /// <param name="applicationServices">The current application's services</param>
    public RedisDatabaseFactory(IServiceProvider applicationServices)
    {
        this.ApplicationServices = applicationServices;
    }

    /// <summary>
    /// Gets the current application's services
    /// </summary>
    protected IServiceProvider ApplicationServices { get; }

    /// <summary>
    /// Gets the plugin's services
    /// </summary>
    protected IServiceProvider? PluginServices { get; set; }

    /// <inheritdoc/>
    public virtual RedisDatabase Create()
    {
        if (this.PluginServices == null) return this.ApplicationServices.GetRequiredService<RedisDatabase>();
        else return this.PluginServices.GetRequiredService<RedisDatabase>();
    }

    object IFactory.Create() => this.Create();

    /// <summary>
    /// Disposes of the <see cref="RedisDatabaseFactory"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="RedisDatabaseFactory"/> is being disposed of</param>
    /// <returns>A new awaitable <see cref="ValueTask"/></returns>
    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (!disposing || this._disposed) return;
        switch (this.PluginServices)
        {
            case IAsyncDisposable asyncDisposable: await asyncDisposable.DisposeAsync().ConfigureAwait(false); break;
            case IDisposable disposable: disposable.Dispose(); break;
        }
        this._disposed = true;
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        await this.DisposeAsync(true).ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes of the <see cref="RedisDatabaseFactory"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="RedisDatabaseFactory"/> is being disposed of</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing || this._disposed) return;
        if (this.PluginServices is IDisposable disposable) disposable.Dispose();
        this._disposed = true;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }


}