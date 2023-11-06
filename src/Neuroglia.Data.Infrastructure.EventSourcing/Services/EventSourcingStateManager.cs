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
using Microsoft.Extensions.Options;
using Neuroglia.Data.Infrastructure.EventSourcing.Configuration;
using Neuroglia.Serialization;

namespace Neuroglia.Data.Infrastructure.EventSourcing.Services;

/// <summary>
/// Represents the default implementation of the <see cref="IAggregateStateManager{TAggregate, TKey}"/> interface
/// </summary>
/// <typeparam name="TAggregate">The type of the <see cref="IAggregateRoot"/> to manage the state of</typeparam>
/// <typeparam name="TKey">The type of key used to identify the <see cref="IAggregateRoot"/> to manage the state of</typeparam>
public class EventSourcingStateManager<TAggregate, TKey>
    : IAggregateStateManager<TAggregate, TKey>
    where TAggregate : class, IAggregateRoot<TKey>
    where TKey : IEquatable<TKey>
{

    /// <summary>
    /// Initializes a new <see cref="EventSourcingStateManager{TAggregate, TKey}"/>
    /// </summary>
    /// <param name="options">The service used to access the current <see cref="StateManagementOptions{TAggregate, TKey}"/></param>
    /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
    /// <param name="eventStore">The service used to store events</param>
    /// <param name="serializer">The service used to serialize and deserialize <see cref="IEventRecord"/>s</param>
    public EventSourcingStateManager(IOptions<StateManagementOptions<TAggregate, TKey>> options, IServiceProvider serviceProvider, IEventStore eventStore, ISerializer serializer)
    {
        this.Options = options.Value;
        this.ServiceProvider = serviceProvider;
        this.EventStore = eventStore;
        this.Serializer = serializer;
    }

    /// <summary>
    /// Gets the current <see cref="StateManagementOptions{TAggregate, TKey}"/>
    /// </summary>
    protected StateManagementOptions<TAggregate, TKey> Options { get; }

    /// <summary>
    /// Gets the current <see cref="IServiceProvider"/>
    /// </summary>
    protected IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Gets the service used to store events
    /// </summary>
    protected IEventStore EventStore { get; }

    /// <summary>
    /// Gets the service used to serialize and deserialize <see cref="IEventRecord"/>s
    /// </summary>
    protected ISerializer Serializer { get; }

    /// <inheritdoc/>
    public virtual async Task TakeSnapshotAsync(TAggregate aggregate, CancellationToken cancellationToken)
    {
        if (!this.Options.SnapshotFrequency.HasValue) return; //no snapshot desired
        var snapshot = await this.GetSnapshotAsync(aggregate.Id, cancellationToken).ConfigureAwait(false);
        if (snapshot == null) { if (aggregate.State.StateVersion < this.Options.SnapshotFrequency.Value) return; }
        else if (snapshot.StateVersion + this.Options.SnapshotFrequency.Value > aggregate.State.StateVersion) return;

        snapshot = aggregate is ISnapshotable snapshotable ? snapshotable.CreateSnapshot() : new Snapshot(aggregate.State, aggregate.State.StateVersion);
        var snapshotEvent = new EventDescriptor("snapshot", snapshot);
        await this.EventStore.AppendAsync(this.GetStreamIdFor(aggregate.Id), new EventDescriptor[] { snapshotEvent }, null, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public virtual async Task<TAggregate> RestoreStateAsync(TKey aggregateId, CancellationToken cancellationToken)
    {
        var factory = this.Options.AggregateFactory ?? ((IServiceProvider provider) => (TAggregate)Activator.CreateInstance(typeof(TAggregate), true)!);
        var aggregate = factory.Invoke(this.ServiceProvider);

        var snapshot = await this.GetSnapshotAsync(aggregateId, cancellationToken).ConfigureAwait(false);
        if (snapshot != null)
        {
            if (aggregate is IRestorable restorable) restorable.Restore(snapshot);
            else aggregate.GetType().GetProperty(nameof(IAggregateRoot.State))?.GetSetMethod()?.Invoke(aggregate, new object[] { snapshot.State });
        }

        return aggregate;
    }

    /// <summary>
    /// Gets the id of the snapshot stream that corresponds to the specified aggregate key
    /// </summary>
    /// <param name="id">The key of the aggregate to get the snapshot stream id for</param>
    /// <returns>The id of the stream that corresponds to the specified aggregate key</returns>
    protected virtual string GetStreamIdFor(TKey id) => $"{typeof(TAggregate).Name.ToLower()}-snapshots-{id}";

    /// <summary>
    /// Gets the snapshot of the specified <see cref="IAggregateRoot"/>
    /// </summary>
    /// <param name="id">The key of the <see cref="IAggregateRoot"/> to find</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The snapshot of the <see cref="IAggregateRoot"/> with the specified key</returns>
    protected virtual async Task<ISnapshot?> GetSnapshotAsync(TKey id, CancellationToken cancellationToken = default)
    {
        try
        {
            var snapshotEvent = await this.EventStore.ReadAsync(this.GetStreamIdFor(id), StreamReadDirection.Backwards, StreamPosition.EndOfStream, 1, cancellationToken).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            return snapshotEvent == null ? null : (ISnapshot)snapshotEvent.Data!;
        }
        catch (StreamNotFoundException)
        {
            return null;
        }
    }

}
