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

using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Neuroglia.Data;

/// <summary>
/// Defines the fundamentals of an entity
/// </summary>
public interface IEntity
    : IIdentifiable, IVersionedState
{

    /// <summary>
    /// Gets the date and time at which the entity has been created
    /// </summary>
    DateTimeOffset CreatedAt { get; }

    /// <summary>
    /// Gets the date and time, if any, at which the entity has last been modified
    /// </summary>
    DateTimeOffset? LastModified { get; }

}

/// <summary>
/// Defines the fundamentals of an entity
/// </summary>
public interface IEntity<TKey>
    : IEntity, IIdentifiable<TKey>
    where TKey : IEquatable<TKey>
{



}