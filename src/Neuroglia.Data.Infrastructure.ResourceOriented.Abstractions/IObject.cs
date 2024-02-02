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
/// Defines the fundamentals of a stored object
/// </summary>
public interface IObject
    : IExtensible
{

    /// <summary>
    /// Gets the API version that defines the versioned group the object belongs to
    /// </summary>
    [Required]
    [DataMember(Order = -999, Name = "apiVersion", IsRequired = true), JsonPropertyOrder(-999), JsonPropertyName("apiVersion"), YamlMember(Order = -999, Alias = "apiVersion")]
    string ApiVersion { get; }

    /// <summary>
    /// Gets the object's kind
    /// </summary>
    [Required]
    [DataMember(Order = -998, Name = "kind", IsRequired = true), JsonPropertyOrder(-998), JsonPropertyName("kind"), YamlMember(Order = -998, Alias = "kind")]
    string Kind { get; }

}