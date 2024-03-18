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

namespace Neuroglia.Data.Infrastructure.ResourceOriented;

/// <summary>
/// Defines the fundamentals of an object which's status is described by a dedicated object
/// </summary>
public interface IStatus
    : ISubResource<IStatus>
{

    /// <summary>
    /// Gets the object's status
    /// </summary>
    [Required]
    [DataMember(Order = 2, Name = "status", IsRequired = true), JsonPropertyOrder(2), JsonPropertyName("status"), YamlMember(Order = 2, Alias = "status")]
    object? Status { get; }

}

/// <summary>
/// Defines the fundamentals of an object which's status is described by a dedicated object
/// </summary>
/// <typeparam name="TStatus"></typeparam>
public interface IStatus<TStatus>
    : IStatus, ISubResource<IStatus, TStatus>
    where TStatus : class, new()
{

    /// <summary>
    /// Gets the object's status
    /// </summary>
    new TStatus? Status { get; }

}
