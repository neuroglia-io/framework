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
/// Defines the fundamentals of a resource
/// </summary>
public interface IResource
    : IObject, IMetadata<ResourceMetadata>
{

    /// <summary>
    /// Gets an object used to describe the resource's type
    /// </summary>
    [IgnoreDataMember, JsonIgnore, YamlIgnore]
    ResourceDefinitionInfo Definition { get; }

}

/// <summary>
/// Defines the fundamentals of a resource
/// </summary>
/// <typeparam name="TSpec">The type of the <see cref="IResource"/>'s spec</typeparam>
public interface IResource<TSpec>
    : IResource, ISpec<TSpec>
    where TSpec : class, new()
{



}

/// <summary>
/// Defines the fundamentals of a resource
/// </summary>
/// <typeparam name="TSpec">The type of the <see cref="IResource"/>'s spec</typeparam>
/// <typeparam name="TStatus">The type of the <see cref="IResource"/>'s status</typeparam>
public interface IResource<TSpec, TStatus>
    : IResource<TSpec>, IStatus<TStatus>
    where TSpec : class, new()
    where TStatus : class, new()
{



}
