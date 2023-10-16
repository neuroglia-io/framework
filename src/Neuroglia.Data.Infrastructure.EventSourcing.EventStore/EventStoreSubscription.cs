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

using EventStore.Client;
using Neuroglia.Data.Infrastructure.EventSourcing.EventStore.Subscriptions;

namespace Neuroglia.Data.Infrastructure.EventSourcing.EventStore;

/// <summary>
/// Represents the default implementation of the <see cref="IEventStoreSubscription"/> interface
/// </summary>
public abstract class EventStoreSubscription
    : IEventStoreSubscription
{

    /// <summary>
    /// Represents the event fired whenever the <see cref="EventStoreSubscription"/> has been disposed of
    /// </summary>
    public event EventHandler? Disposed;

    /// <summary>
    /// Initializes a new <see cref="EventStoreSubscription"/>
    /// </summary>
    /// <param name="id">The <see cref="EventStoreSubscription"/>'s id</param>
    /// <param name="source">The <see cref="EventStoreSubscription"/>'s source</param>
    protected EventStoreSubscription(string id, object source)
    {
        this.Id = id.ToString();
        this.Source = source;
    }

    /// <inheritdoc/>
    public string Id { get; protected set; }

    /// <summary>
    /// Gets the <see cref="EventStoreSubscription"/>'s source
    /// </summary>
    public object Source { get; protected set; }

    /// <summary>
    /// Sets the <see cref="EventStoreSubscription"/>'s source
    /// </summary>
    /// <param name="source"></param>
    public void SetSource(object source) { this.Source = source ?? throw new ArgumentNullException(nameof(source)); }


    /// <summary>
    /// Creates a new <see cref="EventStoreSubscription"/>
    /// </summary>
    /// <param name="subscriptionId">The id of the <see cref="EventStoreSubscription"/> to create</param>
    /// <param name="source">The <see cref="StreamSubscription"/> to create a new <see cref="EventStoreSubscription"/> for</param>
    /// <returns>A new <see cref="EventStoreSubscription"/></returns>
    public static EventStoreSubscription CreateFor(string subscriptionId, StreamSubscription source) => new StandardSubscription(subscriptionId, source);

    private bool _Disposed;
    /// <summary>
    /// Disposes of the <see cref="EventStoreSubscription"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="EventStoreSubscription"/> is being disposed of</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!this._Disposed)
        {
            if (disposing) this.Disposed?.Invoke(this, new EventArgs());
            this._Disposed = true;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

}
