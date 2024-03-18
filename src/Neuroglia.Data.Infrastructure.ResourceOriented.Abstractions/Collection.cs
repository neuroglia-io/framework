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
/// Represents a collection of objects
/// </summary>
[DataContract]
public record Collection
    : ICollection
{

    /// <summary>
    /// Gets/sets the Kind suffix of all collections
    /// </summary>
    public static string KindSuffix { get; set; } = "Collection";

    /// <summary>
    /// Initializes a new <see cref="Collection"/>
    /// </summary>
    public Collection() { }

    /// <summary>
    /// Initializes a new <see cref="Collection"/>
    /// </summary>
    /// <param name="apiVersion">The collection's API version</param>
    /// <param name="kind">The collection's kind</param>
    /// <param name="metadata">The object that describes the collection</param>
    /// <param name="items">An <see cref="IEnumerable{T}"/> containing the items the collection is made out of</param>
    public Collection(string apiVersion, string kind, CollectionMetadata metadata, IEnumerable<object>? items = null)
    {
        if(string.IsNullOrWhiteSpace(apiVersion)) throw new ArgumentNullException(nameof(apiVersion));
        if(string.IsNullOrWhiteSpace(kind)) throw new ArgumentNullException(nameof(kind));
        this.ApiVersion = apiVersion;
        this.Kind = $"{kind}{KindSuffix}";
        this.Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
        this.Items = items?.WithValueSemantics();
    }

    /// <summary>
    /// Gets the collection's API version
    /// </summary>
    [Required]
    [DataMember(Order = 1, Name = "apiVersion", IsRequired = true), JsonPropertyOrder(1), JsonPropertyName("apiVersion"), YamlMember(Order = 1, Alias = "apiVersion")]
    public virtual string ApiVersion { get; set; } = null!;

    /// <summary>
    /// Gets the collection's kind
    /// </summary>
    [Required]
    [DataMember(Order = 2, Name = "kind"), JsonPropertyOrder(2), JsonPropertyName("kind"), YamlMember(Order = 2, Alias = "kind")]
    public virtual string Kind { get; set; } = null!;

    /// <summary>
    /// Gets/sets the object that describes the collection
    /// </summary>
    [Required]
    [DataMember(Order = 3, Name = "metadata", IsRequired = true), JsonPropertyOrder(3), JsonPropertyName("metadata"), YamlMember(Order = 3, Alias = "metadata")]
    public virtual CollectionMetadata Metadata { get; set; } = null!;

    CollectionMetadata IMetadata<CollectionMetadata>.Metadata => this.Metadata;

    object IMetadata.Metadata => this.Metadata;

    /// <inheritdoc/>
    [DataMember(Order = 4, Name = "items", IsRequired = true), JsonPropertyOrder(4), JsonPropertyName("items"), YamlMember(Order = 4, Alias = "items")]
    public virtual EquatableList<object>? Items { get; set; }

    /// <inheritdoc/>
    [DataMember(Order = 999, Name = "extensionData"), JsonExtensionData]
    public virtual IDictionary<string, object>? ExtensionData { get; set; }

}

/// <summary>
/// Represents a collection of objects
/// </summary>
[DataContract]
public class Collection<TObject>
    : ICollection<TObject>
    where TObject : class, IObject, new()
{

    /// <summary>
    /// Initializes a new <see cref="Collection{TObject}"/>
    /// </summary>
    public Collection() { }

    /// <summary>
    /// Initializes a new <see cref="Collection{TObject}"/>
    /// </summary>
    /// <param name="metadata">The object that describes the collection</param>
    /// <param name="items">An <see cref="IEnumerable{T}"/> containing the items the collection is made out of</param>
    public Collection(CollectionMetadata metadata, IEnumerable<TObject>? items)
    {
        var obj = new TObject();
        this.ApiVersion = obj.ApiVersion;
        this.Kind = $"{obj.Kind}{Collection.KindSuffix}";
        this.Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
        this.Items = items?.WithValueSemantics();
    }

    /// <summary>
    /// Gets the collection's API version
    /// </summary>
    [Required]
    [DataMember(Order = 1, Name = "apiVersion", IsRequired = true), JsonPropertyOrder(1), JsonPropertyName("apiVersion"), YamlMember(Order = 1, Alias = "apiVersion")]
    public virtual string ApiVersion { get; set; } = null!;

    /// <summary>
    /// Gets the collection's kind
    /// </summary>
    [Required]
    [DataMember(Order = 2, Name = "kind"), JsonPropertyOrder(2), JsonPropertyName("kind"), YamlMember(Order = 2, Alias = "kind")]
    public virtual string Kind { get; set; } = null!;

    /// <summary>
    /// Gets/sets the object that describes the collection
    /// </summary>
    [Required]
    [DataMember(Order = 3, Name = "metadata", IsRequired = true), JsonPropertyOrder(3), JsonPropertyName("metadata"), YamlMember(Order = 3, Alias = "metadata")]
    public virtual CollectionMetadata Metadata { get; set; } = null!;

    CollectionMetadata IMetadata<CollectionMetadata>.Metadata => this.Metadata;

    object IMetadata.Metadata => this.Metadata;

    /// <inheritdoc/>
    [DataMember(Order = 4, Name = "items", IsRequired = true), JsonPropertyOrder(4), JsonPropertyName("items"), YamlMember(Order = 4, Alias = "items")]
    public virtual EquatableList<TObject>? Items { get; set; }

    EquatableList<object>? ICollection.Items => this.Items?.OfType<object>().WithValueSemantics();

    /// <inheritdoc/>
    [DataMember(Order = 999, Name = "extensionData"), JsonExtensionData]
    public virtual IDictionary<string, object>? ExtensionData { get; set; }

}
