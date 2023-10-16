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

namespace Neuroglia.Data.Infrastructure.EventSourcing;

/// <summary>
/// Represents the snapshot envelope of an <see cref="IAggregateRoot"/>
/// </summary>
/// <typeparam name="TAggregate">The type of the snapshot <see cref="IAggregateRoot"/></typeparam>
public class Snapshot<TAggregate>
    : ISnapshot<TAggregate>
    where TAggregate : class, IAggregateRoot
{

    /// <summary>
    /// Initializes a new <see cref="Snapshot{TKey}"/>
    /// </summary>
    protected Snapshot() { }

    /// <summary>
    /// Initializes a new <see cref="Snapshot{TKey}"/>
    /// </summary>
    /// <param name="data">The <see cref="IAggregateRoot"/> to snapshot</param>
    /// <param name="metadata">The metadata, if any, of the <see cref="Snapshot"/> to create</param>
    public Snapshot(TAggregate data, IDictionary<string, object>? metadata)
    {
        this.Data = data ?? throw new ArgumentNullException(nameof(data));
        this.Version = data.StateVersion;
        this.Metadata = metadata;
    }

    /// <summary>
    /// Initializes a new <see cref="Snapshot{TKey}"/>
    /// </summary>
    /// <param name="data">The <see cref="IAggregateRoot"/> to snapshot</param>
    public Snapshot(TAggregate data) : this(data, null) { }

    /// <inheritdoc/>
    public virtual TAggregate Data { get; protected set; } = null!;

    IAggregateRoot ISnapshot.Data => this.Data;

    /// <inheritdoc/>
    public virtual ulong Version { get; protected set; }

    /// <inheritdoc/>
    public virtual IDictionary<string, object>? Metadata { get; protected set; }

}

/// <summary>
/// Defines helpers methods to handle <see cref="ISnapshot"/>s
/// </summary>
public static class Snapshot
{

    /// <summary>
    /// Creates a new <see cref="Snapshot{TAggregate}"/> for the specified <see cref="IAggregateRoot"/>
    /// </summary>
    /// <param name="aggregate">The <see cref="IAggregateRoot"/> to create a new <see cref="Snapshot{TAggregate}"/> for</param>
    /// <returns>A new <see cref="Snapshot{TAggregate}"/> of the specified <see cref="IAggregateRoot"/></returns>
    public static Snapshot<TAggregate> CreateFor<TAggregate>(TAggregate aggregate)
        where TAggregate : class, IAggregateRoot
    {
        return new(aggregate);
    }

}
