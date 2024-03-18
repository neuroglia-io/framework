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
/// Defines the fundamentals of an object defined by a spec
/// </summary>
public interface ISpec
{

    /// <summary>
    /// Gets the object's spec
    /// </summary>
    [Required]
    [DataMember(Order = 1, Name = "spec", IsRequired = true), JsonPropertyOrder(1), JsonPropertyName("spec"), YamlMember(Order = 1, Alias = "spec")]
    object Spec { get; }

}

/// <summary>
/// Defines the fundamentals of an object defined by a spec
/// </summary>
/// <typeparam name="TSpec">The type of spec</typeparam>
public interface ISpec<TSpec>
    : ISpec
    where TSpec : class, new()
{

    /// <summary>
    /// Gets the object's spec
    /// </summary>
    new TSpec Spec { get; }

}
