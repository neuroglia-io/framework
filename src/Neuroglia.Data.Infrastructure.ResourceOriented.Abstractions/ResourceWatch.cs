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

using System.Reactive.Linq;

namespace Neuroglia.Data.Infrastructure.ResourceOriented;


/// <summary>
/// Represents the default implementation of a service used to watch events of <see cref="IResource"/>s of the specified type
/// </summary>
/// <remarks>
/// Initializes a new <see cref="ResourceWatch{TResource}"/>
/// </remarks>
/// <param name="observable">The service used to observe the <see cref="IResourceWatchEvent{TResource}"/>s emitted by <see cref="IResource"/>s of the specified type</param>
/// <param name="disposeObservable">A boolean used to configure whether or not to dispose of the <see cref="System.Reactive.Linq.Observable"/> when disposing of the <see cref="ResourceWatch"/></param>
public class ResourceWatch(IObservable<IResourceWatchEvent> observable, bool disposeObservable)
    : IResourceWatch
{

    private bool _disposed;

    /// <summary>
    /// Gets the service used to observe the <see cref="IResourceWatchEvent{TResource}"/>s emitted by <see cref="IResource"/>s of the specified type
    /// </summary>
    protected IObservable<IResourceWatchEvent> Observable { get; } = observable;

    /// <summary>
    /// Gets a boolean used to configure whether or not to dispose of the <see cref="System.Reactive.Linq.Observable"/> when disposing of the <see cref="ResourceWatch"/>
    /// </summary>
    internal protected bool DisposeObservable { get; } = disposeObservable;

    /// <inheritdoc/>
    public virtual IDisposable Subscribe(IObserver<IResourceWatchEvent> observer) => this.Observable.Subscribe(observer);

    /// <summary>
    /// Disposes of the <see cref="ResourceWatch{TResource}"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="ResourceWatch{TResource}"/> is being disposed of</param>
    /// <returns>A new awaitable <see cref="ValueTask"/></returns>
    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (!disposing || this._disposed) return;
        if (this.DisposeObservable)
        {
            switch (this.Observable)
            {
                case IAsyncDisposable asyncDisposable:
                    await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                    break;
                case IDisposable disposable:
                    disposable.Dispose();
                    break;
            }
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
    /// Disposes of the <see cref="ResourceWatch{TResource}"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="ResourceWatch{TResource}"/> is being disposed of</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing || this._disposed) return;
        if (this.DisposeObservable && this.Observable is IDisposable disposable) disposable.Dispose();
        this._disposed = true;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

}

/// <summary>
/// Represents the default implementation of a service used to watch events of <see cref="IResource"/>s of the specified type
/// </summary>
/// <typeparam name="TResource">The type of <see cref="IResource"/> to watch the events of</typeparam>
/// <remarks>
/// Initializes a new <see cref="ResourceWatch{TResource}"/>
/// </remarks>
/// <param name="observable">The service used to observe the <see cref="IResourceWatchEvent{TResource}"/>s emitted by <see cref="IResource"/>s of the specified type</param>
/// <param name="disposeObservable">A boolean used to configure whether or not to dispose of the <see cref="System.Reactive.Linq.Observable"/> when disposing of the <see cref="ResourceWatch"/></param>
public class ResourceWatch<TResource>(IObservable<IResourceWatchEvent<TResource>> observable, bool disposeObservable)
    : IResourceWatch<TResource>
    where TResource : class, IResource, new()
{

    private bool _disposed;

    /// <summary>
    /// Initializes a new <see cref="ResourceWatch{TResource}"/>
    /// </summary>
    /// <param name="observable">The service used to observe the <see cref="IResourceWatchEvent"/>s emitted by <see cref="IResource"/>s of the specified type</param>
    /// <param name="disposeObservable">A boolean used to configure whether or not to dispose of the <see cref="System.Reactive.Linq.Observable"/> when disposing of the <see cref="ResourceWatch"/></param>
    public ResourceWatch(IObservable<IResourceWatchEvent> observable, bool disposeObservable) : this(observable.Select(e => e.OfType<TResource>()), disposeObservable) { }

    /// <summary>
    /// Gets the service used to observe the <see cref="IResourceWatchEvent{TResource}"/>s emitted by <see cref="IResource"/>s of the specified type
    /// </summary>
    protected IObservable<IResourceWatchEvent<TResource>> Observable { get; } = observable;

    /// <summary>
    /// Gets a boolean used to configure whether or not to dispose of the <see cref="System.Reactive.Linq.Observable"/> when disposing of the <see cref="ResourceWatch"/>
    /// </summary>
    internal protected bool DisposeObservable { get; } = disposeObservable;

    /// <inheritdoc/>
    public virtual IDisposable Subscribe(IObserver<IResourceWatchEvent<TResource>> observer) => this.Observable.Subscribe(observer);

    /// <summary>
    /// Disposes of the <see cref="ResourceWatch{TResource}"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="ResourceWatch{TResource}"/> is being disposed of</param>
    /// <returns>A new awaitable <see cref="ValueTask"/></returns>
    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (!disposing || this._disposed) return;

        switch (this.Observable)
        {
            case IAsyncDisposable asyncDisposable:
                await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                break;
            case IDisposable disposable:
                disposable.Dispose();
                break;
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
    /// Disposes of the <see cref="ResourceWatch{TResource}"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="ResourceWatch{TResource}"/> is being disposed of</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing || this._disposed) return;
        if (this.DisposeObservable && this.Observable is IDisposable disposable) disposable.Dispose();
        this._disposed = true;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

}