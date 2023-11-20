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

using System.Text.Json.Serialization;

namespace Neuroglia.Data;

/// <summary>
/// Represents the default implementation of the <see cref="IAggregateState{TKey}"/> interface
/// </summary>
/// <typeparam name="TKey">The type of key used to identify the <see cref="IAggregateRoot"/></typeparam>
public abstract record AggregateState<TKey>
    : IEntity<TKey>, IAggregateState<TKey>
    where TKey : IEquatable<TKey>
{

    /// <summary>
    /// Initializes a new <see cref="AggregateState{TKey}"/>
    /// </summary>
    protected AggregateState() { }

    /// <summary>
    /// Initializes a new <see cref="AggregateState{TKey}"/>
    /// </summary>
    /// <param name="id">The id used to authenticate the aggregate the state belongs to</param>
    protected AggregateState(TKey id) => this.Id = id;

    /// <inheritdoc/>
    [JsonInclude]
    public virtual TKey Id { get; protected set; } = default!;

    object IIdentifiable.Id => this.Id;

    /// <inheritdoc/>
    [JsonInclude]
    public virtual ulong StateVersion { get; set; }

    /// <inheritdoc/>
    [JsonInclude]
    public virtual DateTimeOffset CreatedAt { get; protected set; }

    /// <inheritdoc/>
    [JsonInclude]
    public virtual DateTimeOffset? LastModified { get; protected set; }

    /// <inheritdoc/>
    public virtual bool Equals(IIdentifiable<TKey>? other) => other?.Id?.Equals(this.Id) == true;

}