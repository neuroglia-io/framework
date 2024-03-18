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
using Neuroglia.Data.Infrastructure.ResourceOriented.Configuration;
using Neuroglia.Reactive;
using System.Collections.Concurrent;

namespace Neuroglia.Data.Infrastructure.ResourceOriented.Services;

/// <summary>
/// Represents the default implementation of the <see cref="IResourceController{TResource}"/> interface
/// </summary>
/// <typeparam name="TResource">The type of <see cref="IResource"/> to control</typeparam>
public class ResourceController<TResource>
    : IHostedService, IResourceController<TResource>
    where TResource : class, IResource, new()
{

    bool _disposed;

    /// <summary>
    /// Initializes a new <see cref="ResourceController{TResource}"/>
    /// </summary>
    /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
    /// <param name="controllerOptions">The service used to access the current <see cref="IOptions{TOptions}"/></param>
    /// <param name="repository">The service used to manage <see cref="IResource"/>s</param>
    public ResourceController(ILoggerFactory loggerFactory, IOptions<ResourceControllerOptions<TResource>> controllerOptions, IRepository repository)
    {
        this.Logger = loggerFactory.CreateLogger(this.GetType());
        this.Options = controllerOptions.Value;
        this.Repository = repository;
    }

    /// <summary>
    /// Gets the service used to perform logging
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Gets the options used to configure the <see cref="ResourceController{TResource}"/>
    /// </summary>
    protected ResourceControllerOptions<TResource> Options { get; }

    /// <summary>
    /// Gets the service used to manage <see cref="IResource"/>s
    /// </summary>
    protected IRepository Repository { get; }

    /// <summary>
    /// Gets the service used to watch changes on <see cref="IResource"/>s to control
    /// </summary>
    protected IResourceWatch<TResource> Watch { get; private set; } = null!;

    /// <summary>
    /// Gets the <see cref="Timer"/> used by the reconciliation loop
    /// </summary>
    protected Timer? ReconciliationTimer { get; private set; }

    /// <summary>
    /// Gets a <see cref="ConcurrentDictionary{TKey, TValue}"/> containing all controlled <see cref="IResource"/>s
    /// </summary>
    protected ConcurrentDictionary<string, TResource> Resources { get; } = [];

    /// <summary>
    /// Gets the <see cref="ResourceController{TResource}"/>'s <see cref="System.Threading.CancellationTokenSource"/>
    /// </summary>
    protected CancellationTokenSource CancellationTokenSource { get; private set; } = null!;

    List<TResource> IResourceController<TResource>.Resources => this.Resources.Values.ToList();

    /// <inheritdoc/>
    public virtual async Task StartAsync(CancellationToken cancellationToken)
    {
        this.CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        await this.ReconcileAsync(this.CancellationTokenSource.Token).ConfigureAwait(false);
        this.Watch = await this.Repository.WatchAsync<TResource>(this.Options.ResourceNamespace, cancellationToken: this.CancellationTokenSource.Token).ConfigureAwait(false);
        this.Watch.SubscribeAsync(this.OnResourceWatchEventAsync);
        this.ReconciliationTimer = new(async _ => await this.ReconcileAsync(this.CancellationTokenSource.Token).ConfigureAwait(false), null, this.Options.Reconciliation.Interval, this.Options.Reconciliation.Interval);
    }

    /// <inheritdoc/>
    public virtual Task StopAsync(CancellationToken cancellationToken)
    {
        this.CancellationTokenSource?.Cancel();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Reconciles the state of controlled resources with their actual state, as advertised by the server
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual async Task ReconcileAsync(CancellationToken cancellationToken = default)
    {
        var existingResourceKeys = new HashSet<string>();
        await foreach (var resource in this.Repository.GetAllAsync<TResource>(this.Options.ResourceNamespace, this.Options.LabelSelectors, cancellationToken).ConfigureAwait(false))
        {
            var cacheKey = this.GetResourceCacheKey(resource.GetName(), resource.GetNamespace());
            if (this.Resources.TryGetValue(cacheKey, out var cachedState) && cachedState != null)
            {
                if (cachedState.Metadata.ResourceVersion != resource.Metadata.ResourceVersion) await this.OnResourceUpdatedAsync(cachedState, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                await this.OnResourceCreatedAsync(resource, cancellationToken).ConfigureAwait(false);
            }
            existingResourceKeys.Add(cacheKey);
        }
        foreach (var resource in this.Resources.ToList().Where(kvp => !existingResourceKeys.Contains(kvp.Key)).Select(kvp => kvp.Value))
        {
            await this.OnResourceDeletedAsync(resource, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Builds a new cache key for the specified resource
    /// </summary>
    /// <param name="name">The name of the resource to build a new cache key for</param>
    /// <param name="namespace">The namespace the resource to build a new cache key for belongs to</param>
    /// <returns>A new cache key</returns>
    protected virtual string GetResourceCacheKey(string name, string? @namespace) => string.IsNullOrWhiteSpace(@namespace) ? name : $"{@namespace}.{name}";

    /// <inheritdoc/>
    public virtual IDisposable Subscribe(IObserver<IResourceWatchEvent<TResource>> observer) => this.Watch.Subscribe(observer);

    /// <summary>
    /// Handles the specified <see cref="IResource"/>'s change
    /// </summary>
    /// <param name="e">An <see cref="IResourceWatchEvent{TResource}"/> that describes the change that has occurred</param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    private Task OnResourceWatchEventAsync(IResourceWatchEvent<TResource> e)
    {
        return e.Type switch
        {
            ResourceWatchEventType.Created => this.OnResourceCreatedAsync(e.Resource, this.CancellationTokenSource.Token),
            ResourceWatchEventType.Updated => this.OnResourceUpdatedAsync(e.Resource, this.CancellationTokenSource.Token),
            ResourceWatchEventType.Deleted => this.OnResourceDeletedAsync(e.Resource, this.CancellationTokenSource.Token),
            _ => Task.CompletedTask
        };
    }

    /// <summary>
    /// Handles the creation of a new <see cref="IResource"/>
    /// </summary>
    /// <param name="resource">The newly created <see cref="IResource"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual Task OnResourceCreatedAsync(TResource resource, CancellationToken cancellationToken = default)
    {
        var cacheKey = this.GetResourceCacheKey(resource.GetName(), resource.GetNamespace());
        resource = this.Resources.AddOrUpdate(cacheKey, resource, (key, existing) => resource);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the update of a <see cref="IResource"/>
    /// </summary>
    /// <param name="resource">The updated <see cref="IResource"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual Task OnResourceUpdatedAsync(TResource resource, CancellationToken cancellationToken = default)
    {
        var cacheKey = this.GetResourceCacheKey(resource.GetName(), resource.GetNamespace());
        resource = this.Resources.AddOrUpdate(cacheKey, resource, (key, existing) => resource);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the deletion of a <see cref="IResource"/>
    /// </summary>
    /// <param name="resource">The deleted <see cref="IResource"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual Task OnResourceDeletedAsync(TResource resource, CancellationToken cancellationToken = default)
    {
        var cacheKey = this.GetResourceCacheKey(resource.GetName(), resource.GetNamespace());
        this.Resources.Remove(cacheKey, out _);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Disposes of the <see cref="ResourceController{TResource}"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="ResourceController{TResource}"/> is being disposed of</param>
    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (this._disposed || !disposing) return;
        this.CancellationTokenSource?.Dispose();
        if (this.Watch != null) await this.Watch.DisposeAsync().ConfigureAwait(false);
        this._disposed = true;
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        await this.DisposeAsync(true).ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes of the <see cref="ResourceController{TResource}"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="ResourceController{TResource}"/> is being disposed of</param>
    protected virtual void Dispose(bool disposing)
    {
        if (this._disposed || !disposing) return;
        this.CancellationTokenSource?.Dispose();
        this.Watch?.Dispose();
        this._disposed = true;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

}
