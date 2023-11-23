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

namespace Neuroglia;

/// <summary>
/// Represents the base class for Data Transfer Objects used to describe an entity
/// </summary>
/// <typeparam name="TKey">The type of key used to identify the described entity</typeparam>
[DataContract]
public abstract record EntityDataTransferObject<TKey>
    : DataTransferObject, IIdentifiable<TKey>
    where TKey : IEquatable<TKey>
{

    /// <summary>
    /// Gets/sets the id of the described entity
    /// </summary>
    [DataMember]
    public virtual TKey Id { get; set; } = default!;

    /// <summary>
    /// Gets/sets the date and time the described entity has been created at
    /// </summary>
    [DataMember]
    public virtual DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets/sets the date and time the described entity has last been modified
    /// </summary>
    [DataMember]
    public virtual DateTimeOffset? LastModified { get; set; }

    object IIdentifiable.Id => this.Id;

    /// <inheritdoc/>
    public virtual bool Equals(IIdentifiable<TKey>? other) => other != null && other.GetType() == this.GetType() && this.Id.Equals(this.Id);

}
