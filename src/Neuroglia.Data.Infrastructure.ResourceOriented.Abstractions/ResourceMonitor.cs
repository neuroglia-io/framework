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

using Neuroglia.Data.Infrastructure.ResourceOriented;
using System.Reactive.Linq;

namespace Neuroglia.Data.Infrastructure.ResourceOriented.Services;

/// <summary>
/// Represents the default implementation of the <see cref="IResourceMonitor"/> interface
/// </summary>
public class ResourceMonitor
    : IResourceMonitor, IDisposable, IAsyncDisposable
{

    bool _disposed;

    /// <summary>
    /// Initializes a new <see cref="ResourceMonitor"/>
    /// </summary>
    /// <param name="resourceWatch">The service used to watch events produced by the monitored <see cref="IResource"/></param>
    /// <param name="resource">The current state of the monitored <see cref="IResource"/></param>
    /// <param name="leaveOpen">A boolean indicating whether or not to leave the <see cref="IResourceWatch"/> open when the <see cref="ResourceMonitor"/> is being disposed of</param>
    public ResourceMonitor(IResourceWatch resourceWatch, IResource resource, bool leaveOpen)
    {
        this.ResourceWatch = resourceWatch;
        this.Resource = resource;
        this.LeaveOpen = leaveOpen;
        this.CancellationTokenSource = new();
        this.ResourceEvents = this.ResourceWatch.Where(r => r.Resource.GetName() == this.Resource.GetName() && r.Resource.GetNamespace() == this.Resource.GetNamespace());
        this.ResourceEvents.Where(e => e.Type == ResourceWatchEventType.Updated).Select(e => e.Resource).Subscribe(this.OnResourceUpdated, this.CancellationTokenSource.Token);
        this.ResourceEvents.Where(e => e.Type == ResourceWatchEventType.Deleted).Select(e => e.Resource).Subscribe(this.OnResourceDeleted, this.CancellationTokenSource.Token);
    }

    /// <summary>
    /// Gets the service used to watch events produced by the monitored <see cref="IResource"/>
    /// </summary>
    protected IResourceWatch ResourceWatch { get; }

    /// <summary>
    /// Gets a boolean indicating whether or not to leave the <see cref="IResourceWatch"/> open when the <see cref="ResourceMonitor"/> is being disposed of
    /// </summary>
    protected bool LeaveOpen { get; }

    /// <summary>
    /// Gets the <see cref="ResourceMonitor"/>'s <see cref="System.Threading.CancellationTokenSource"/>
    /// </summary>
    protected CancellationTokenSource CancellationTokenSource { get; }

    /// <summary>
    /// Gets the <see cref="IObservable{T}"/> used to watch the <see cref="IResourceWatchEvent"/>s produced by the monitored <see cref="IResource"/>
    /// </summary>
    protected IObservable<IResourceWatchEvent> ResourceEvents { get; }

    /// <summary>
    /// Gets the current state of the monitored <see cref="IResource"/>
    /// </summary>
    public IResource Resource { get; private set; }

    /// <inheritdoc/>
    public virtual IDisposable Subscribe(IObserver<IResourceWatchEvent> observer)
    {
        return this.ResourceWatch.Where(r => r.Resource.GetName() == this.Resource.GetName() && r.Resource.GetNamespace() == this.Resource.GetNamespace()).Subscribe(observer);
    }

    /// <summary>
    /// Handles updates to the monitored <see cref="IResource"/>
    /// </summary>
    /// <param name="resource">The updated <see cref="IResource"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual void OnResourceUpdated(IResource resource)
    {
        this.Resource = resource;
    }

    /// <summary>
    /// Handles the deletion of the monitored <see cref="IResource"/>
    /// </summary>
    /// <param name="resource">The deleted <see cref="IResource"/></param>
    protected virtual void OnResourceDeleted(IResource resource)
    {
        this.CancellationTokenSource.Cancel();
        this.Resource = null!;
    }

    /// <summary>
    /// Disposes of the <see cref="ResourceMonitor"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="ResourceMonitor"/> is being disposed of</param>
    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (this._disposed || !disposing) return;
        this.CancellationTokenSource?.Dispose();
        if (!this.LeaveOpen && this.ResourceWatch != null) await this.ResourceWatch.DisposeAsync().ConfigureAwait(false);
        this._disposed = true;
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        await this.DisposeAsync(true).ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes of the <see cref="ResourceMonitor"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="ResourceMonitor"/> is being disposed of</param>
    protected virtual void Dispose(bool disposing)
    {
        if (this._disposed || !disposing) return;
        this.CancellationTokenSource?.Dispose();
        if (!this.LeaveOpen) this.ResourceWatch?.Dispose();
        this._disposed = true;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

}

/// <summary>
/// Represents the default implementation of the <see cref="IResourceMonitor"/> interface
/// </summary>
/// <typeparam name="TResource">The type of <see cref="IResource"/> to monitor</typeparam>
public class ResourceMonitor<TResource>
    :  IResourceMonitor<TResource>, IDisposable, IAsyncDisposable
    where TResource : class, IResource, new()
{

    bool _disposed;

    /// <summary>
    /// Initializes a new <see cref="ResourceMonitor{TResource}"/>
    /// </summary>
    /// <param name="resourceWatch">The service used to watch events produced by the monitored <see cref="IResource"/></param>
    /// <param name="resource">The current state of the monitored <see cref="IResource"/></param>
    /// <param name="leaveOpen">A boolean indicating whether or not to leave the <see cref="IResourceWatch"/> open when the <see cref="ResourceMonitor{TResource}"/> is being disposed of</param>
    public ResourceMonitor(IResourceWatch<TResource> resourceWatch, TResource resource, bool leaveOpen)
    {
        this.ResourceWatch = resourceWatch;
        this.Resource = resource;
        this.LeaveOpen = leaveOpen;
        this.CancellationTokenSource = new();
        this.ResourceEvents = this.ResourceWatch.Where(r => r.Resource.GetName() == this.Resource.GetName() && r.Resource.GetNamespace() == this.Resource.GetNamespace());
        this.ResourceEvents.Where(e => e.Type == ResourceWatchEventType.Updated).Select(e => e.Resource).Subscribe(this.OnResourceUpdated, this.CancellationTokenSource.Token);
        this.ResourceEvents.Where(e => e.Type == ResourceWatchEventType.Deleted).Select(e => e.Resource).Subscribe(this.OnResourceDeleted, this.CancellationTokenSource.Token);
    }

    /// <summary>
    /// Gets the service used to watch events produced by the monitored <see cref="IResource"/>
    /// </summary>
    protected IResourceWatch<TResource> ResourceWatch { get; }

    /// <summary>
    /// Gets a boolean indicating whether or not to leave the <see cref="IResourceWatch"/> open when the <see cref="ResourceMonitor{TResource}"/> is being disposed of
    /// </summary>
    protected bool LeaveOpen { get; }

    /// <summary>
    /// Gets the <see cref="ResourceMonitor"/>'s <see cref="System.Threading.CancellationTokenSource"/>
    /// </summary>
    protected CancellationTokenSource CancellationTokenSource { get; }

    /// <summary>
    /// Gets the <see cref="IObservable{T}"/> used to watch the <see cref="IResourceWatchEvent"/>s produced by the monitored <see cref="IResource"/>
    /// </summary>
    protected IObservable<IResourceWatchEvent<TResource>> ResourceEvents { get; }

    /// <summary>
    /// Gets the current state of the monitored <see cref="IResource"/>
    /// </summary>
    public TResource Resource { get; private set; } = null!;

    /// <inheritdoc/>
    public virtual IDisposable Subscribe(IObserver<IResourceWatchEvent<TResource>> observer)
    {
        return this.ResourceWatch.Where(r => r.Resource.GetName() == this.Resource.GetName() && r.Resource.GetNamespace() == this.Resource.GetNamespace()).Subscribe(observer);
    }

    /// <summary>
    /// Handles updates to the monitored <see cref="IResource"/>
    /// </summary>
    /// <param name="resource">The updated <see cref="IResource"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual void OnResourceUpdated(TResource resource)
    {
        this.Resource = resource;
    }

    /// <summary>
    /// Handles the deletion of the monitored <see cref="IResource"/>
    /// </summary>
    /// <param name="resource">The deleted <see cref="IResource"/></param>
    protected virtual void OnResourceDeleted(TResource resource)
    {
        this.CancellationTokenSource.Cancel();
        this.Resource = null!;
    }

    /// <summary>
    /// Disposes of the <see cref="ResourceMonitor"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="ResourceMonitor"/> is being disposed of</param>
    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (this._disposed || !disposing) return;
        this.CancellationTokenSource?.Dispose();
        if (!this.LeaveOpen && this.ResourceWatch != null) await this.ResourceWatch.DisposeAsync().ConfigureAwait(false);
        this._disposed = true;
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        await this.DisposeAsync(true).ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes of the <see cref="ResourceMonitor"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="ResourceMonitor"/> is being disposed of</param>
    protected virtual void Dispose(bool disposing)
    {
        if (this._disposed || !disposing) return;
        this.CancellationTokenSource?.Dispose();
        if (!this.LeaveOpen) this.ResourceWatch?.Dispose();
        this._disposed = true;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

}