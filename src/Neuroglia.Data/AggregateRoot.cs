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

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Neuroglia.Data;

/// <summary>
/// Represents the default implementation of the <see cref="IAggregateRoot"/> interface
/// </summary>
/// <typeparam name="TKey">The type of key used to uniquely identify the <see cref="IAggregateRoot"/></typeparam>
/// <typeparam name="TState">The type of the <see cref="IAggregateRoot"/>'s state</typeparam>
public abstract class AggregateRoot<TKey, TState>
    : IEntity<TKey>, IAggregateRoot<TKey, TState>
    where TKey : IEquatable<TKey>
    where TState : class, IAggregateState<TKey>, new()
{

    /// <summary>
    /// Initializes a new <see cref="AggregateRoot{TKey}"/>
    /// </summary>
    protected AggregateRoot() { }

    /// <inheritdoc/>
    [IgnoreDataMember, JsonIgnore]
    public virtual TKey Id => this.State.Id;

    object IIdentifiable.Id => this.Id;

    /// <inheritdoc/>
    [IgnoreDataMember, JsonIgnore]
    public virtual DateTimeOffset CreatedAt => this.State.CreatedAt;

    /// <inheritdoc/>
    [IgnoreDataMember, JsonIgnore]
    public virtual DateTimeOffset? LastModified => this.State.LastModified;

    readonly List<IDomainEvent> _pendingEvents = [];
    /// <inheritdoc/>
    [IgnoreDataMember, JsonIgnore]
    public virtual IReadOnlyList<IDomainEvent> PendingEvents => this._pendingEvents.AsReadOnly();

    /// <inheritdoc/>
    [DataMember, JsonInclude]
    public virtual TState State { get; protected set; } = new();

    IAggregateState<TKey> IAggregateRoot<TKey>.State => this.State;

    IAggregateState IAggregateRoot.State => this.State;

    /// <summary>
    /// Registers the specified <see cref="IDomainEvent"/>
    /// </summary>
    /// <typeparam name="TEvent">The type of <see cref="IDomainEvent"/> to register</typeparam>
    /// <param name="e">The <see cref="IDomainEvent"/> to register</param>
    /// <returns>The registered <see cref="IDomainEvent"/></returns>
    protected virtual TEvent RegisterEvent<TEvent>(TEvent e)
        where TEvent : IDomainEvent
    {
        if (e == null) throw new ArgumentNullException(nameof(e));

        if (e is DomainEvent<TKey> domainEvent) domainEvent.AggregateVersion = this.State.StateVersion + (ulong)this.PendingEvents.Count + 1;

        this._pendingEvents.Add(e);
        return e;
    }

    /// <inheritdoc/>
    public virtual void ClearPendingEvents() => this._pendingEvents.Clear();

    /// <inheritdoc/>
    public bool Equals(IIdentifiable<TKey>? other) => other?.Id.Equals(this.Id) == true;

}
