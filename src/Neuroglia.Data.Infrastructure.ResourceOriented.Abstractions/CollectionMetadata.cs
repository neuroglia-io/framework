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
/// Represents an object used to describe a resource collection
/// </summary>
[DataContract]
public record CollectionMetadata
    : IExtensible
{

    /// <summary>
    /// Initializes a new <see cref="CollectionMetadata"/>
    /// </summary>
    public CollectionMetadata() { }

    /// <summary>
    /// Gets/sets a value used to continue paging resources, in the context of a paging request
    /// </summary>
    [DataMember(Order = 1, Name = "continue"), JsonPropertyOrder(1), JsonPropertyName("continue"), YamlMember(Order = 1, Alias = "continue")]
    public virtual string? Continue { get; set; }

    /// <summary>
    /// Gets/sets the count of remaining items that are not included in the collection
    /// </summary>
    [DataMember(Order = 2, Name = "remainingItemCount"), JsonPropertyOrder(2), JsonPropertyName("remainingItemCount"), YamlMember(Order = 2, Alias = "remainingItemCount")]
    public virtual long? RemainingItemCount { get; set; }

    /// <summary>
    /// Gets/sets a value that references the collection's version and that can be used to determine when the collection has changed
    /// </summary>
    [DataMember(Order = 3, Name = "resourceVersion"), JsonPropertyOrder(3), JsonPropertyName("resourceVersion"), YamlMember(Order = 3, Alias = "resourceVersion")]
    public virtual string ResourceVersion { get; set; } = null!;

    /// <inheritdoc/>
    [DataMember(Order = 999, Name = "extensionData"), JsonExtensionData]
    public IDictionary<string, object>? ExtensionData { get; set; }

}