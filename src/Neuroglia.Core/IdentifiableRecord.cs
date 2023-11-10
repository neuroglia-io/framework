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

namespace Neuroglia;

/// <summary>
/// Represents the default abstract implementation of the <see cref="IIdentifiable{TKey}"/> interface
/// </summary>
/// <typeparam name="TKey">The type of key used to uniquely identify the object</typeparam>
public abstract class IdentifiableRecord<TKey>
    : IIdentifiable<TKey>
    where TKey : IEquatable<TKey>
{

    /// <summary>
    /// Initializes a new <see cref="IdentifiableRecord{TKey}"/>
    /// </summary>
    protected IdentifiableRecord() { }

    /// <summary>
    /// Initializes a new <see cref="IdentifiableRecord{TKey}"/>
    /// </summary>
    /// <param name="id">The key used to identify the object</param>
    protected IdentifiableRecord(TKey id)
    {
        this.Id = id;
    }

    /// <inheritdoc/>
    public virtual TKey Id { get; protected set; } = default!;

    object IIdentifiable.Id => this.Id;

    /// <inheritdoc/>
    public virtual bool Equals(IIdentifiable<TKey>? other) => other != null && this.Id.Equals(other.Id);

}