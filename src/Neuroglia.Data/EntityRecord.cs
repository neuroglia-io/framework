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

namespace Neuroglia.Data;

/// <summary>
/// Represents the default abstract implementation of the <see cref="IEntity{TKey}"/> interface
/// </summary>
/// <typeparam name="TKey">The type of key used to uniquely identify the <see cref="IEntity{TKey}"/></typeparam>
public abstract record EntityRecord<TKey>
    : IdentifiableRecord<TKey>, IEntity<TKey>
    where TKey : IEquatable<TKey>
{

    /// <summary>
    /// Initializes a new <see cref="EntityRecord{TKey}"/>
    /// </summary>
    protected EntityRecord() { }

    /// <summary>
    /// Initializes a new <see cref="EntityRecord{TKey}"/>
    /// </summary>
    /// <param name="id">The <see cref="IEntity"/>'s unique key</param>
    protected EntityRecord(TKey id)
        : base(id)
    {
        this.CreatedAt = DateTimeOffset.UtcNow;
    }

    /// <inheritdoc/>
    public virtual DateTimeOffset CreatedAt { get; protected set; }

    /// <inheritdoc/>
    public virtual DateTimeOffset? LastModified { get; protected set; }

    /// <inheritdoc/>
    public virtual ulong StateVersion { get; set; }

}
