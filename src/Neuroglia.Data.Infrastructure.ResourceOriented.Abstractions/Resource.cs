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
/// Represents the base class of all cloud stream resources
/// </summary>
[DataContract]
public record Resource
    : IResource
{

    /// <summary>
    /// Gets the default resource group
    /// </summary>
    public const string DefaultResourceGroup = "neuroglia.io";

    /// <summary>
    /// Initializes a new <see cref="Resource"/>
    /// </summary>
    public Resource() { }

    /// <summary>
    /// Initializes a new <see cref="Resource"/>
    /// </summary>
    /// <param name="definition">An object used to describe the <see cref="Resource"/>'s definition</param>
    public Resource(ResourceDefinitionInfo definition) 
    {
        this.Definition = definition ?? throw new ArgumentNullException(nameof(definition));
        this.ApiVersion = this.Definition.GetApiVersion();
        this.Kind = this.Definition.Kind;
    }

    /// <summary>
    /// Initializes a new <see cref="Resource"/>
    /// </summary>
    /// <param name="definition">An object used to describe the <see cref="Resource"/>'s definition</param>
    /// <param name="metadata">The object that describes the resource</param>
    public Resource(ResourceDefinitionInfo definition, ResourceMetadata metadata)
        : this(definition)
    {
        this.Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
    }

    /// <summary>
    /// Gets the resource's API version
    /// </summary>
    [Required]
    [DataMember(Order = 1, Name = "apiVersion", IsRequired = true), JsonPropertyOrder(1), JsonPropertyName("apiVersion"), YamlMember(Order = 1, Alias = "apiVersion")]
    public virtual string ApiVersion { get; set; } = null!;

    /// <summary>
    /// Gets the resource's kind
    /// </summary>
    [Required]
    [DataMember(Order = 2, Name = "kind"), JsonPropertyOrder(2), JsonPropertyName("kind"), YamlMember(Order = 2, Alias = "kind")]
    public virtual string Kind { get; set; } = null!;

    /// <summary>
    /// Gets/sets the object that describes the resource
    /// </summary>
    [Required]
    [DataMember(Order = 3, Name = "metadata", IsRequired = true), JsonPropertyOrder(3), JsonPropertyName("metadata"), YamlMember(Order = 3, Alias = "metadata")]
    public virtual ResourceMetadata Metadata { get; set; } = null!;

    object IMetadata.Metadata => this.Metadata;

    /// <inheritdoc/>
    [IgnoreDataMember, JsonIgnore, YamlIgnore]
    public virtual ResourceDefinitionInfo Definition { get; } = null!;

    /// <inheritdoc/>
    [DataMember(Order = 999, Name = "extensionData"), JsonExtensionData]
    public virtual IDictionary<string, object>? ExtensionData { get; set; }

    /// <inheritdoc/>
    public override string ToString() => this.IsNamespaced() ? $"{this.GetNamespace()}/{this.GetName()}" : this.GetName();

    /// <summary>
    /// Builds a new API version
    /// </summary>
    /// <param name="group">The API group</param>
    /// <param name="version">The API version</param>
    /// <returns>A new API version</returns>
    public static string BuildApiVersion(string group, string version) => $"{group}/{version}";

}

/// <summary>
/// Represents the base class of all cloud stream resources
/// </summary>
/// <typeparam name="TSpec">The type of the resource's spec</typeparam>
[DataContract]
public record Resource<TSpec>
    : Resource, IResource<TSpec>
    where TSpec : class, new()
{

    /// <summary>
    /// Initializes a new <see cref="Resource"/>
    /// </summary>
    public Resource() { }

    /// <summary>
    /// Initializes a new <see cref="Resource"/>
    /// </summary>
    /// <param name="definition">An object used to describe the <see cref="Resource"/>'s definition</param>
    public Resource(ResourceDefinitionInfo definition) : base(definition) { }

    /// <summary>
    /// Initializes a new <see cref="Resource"/>
    /// </summary>
    /// <param name="definition">An object used to describe the resource's definition</param>
    /// <param name="metadata">The object that describes the resource</param>
    /// <param name="spec">The resource's spec</param>
    public Resource(ResourceDefinitionInfo definition, ResourceMetadata metadata, TSpec spec)
        : base(definition, metadata)
    {
        this.Spec = spec ?? throw new ArgumentNullException(nameof(spec));
    }

    /// <summary>
    /// Gets/sets the object used to define and configure the resource
    /// </summary>
    [DataMember(Order = 4, Name = "spec"), JsonPropertyOrder(4), JsonPropertyName("spec"), YamlMember(Order = 4, Alias = "spec")]
    public virtual TSpec Spec { get; set; } = null!;

    object ISpec.Spec => this.Spec;

}

/// <summary>
/// Represents the base class of all cloud stream resources
/// </summary>
/// <typeparam name="TSpec">The type of the resource's spec</typeparam>
/// <typeparam name="TStatus">The type of the resource's status</typeparam>
[DataContract]
public record Resource<TSpec, TStatus>
    : Resource<TSpec>, IResource<TSpec, TStatus>
    where TSpec : class, new()
    where TStatus : class, new()
{

    /// <summary>
    /// Initializes a new <see cref="Resource"/>
    /// </summary>
    public Resource() { }

    /// <summary>
    /// Initializes a new <see cref="Resource"/>
    /// </summary>
    /// <param name="definition">An object used to describe the <see cref="Resource"/>'s type</param>
    public Resource(ResourceDefinitionInfo definition) : base(definition) { }

    /// <summary>
    /// Initializes a new <see cref="Resource"/>
    /// </summary>
    /// <param name="definition">An object used to describe the resource's type</param>
    /// <param name="metadata">The object that describes the resource</param>
    /// <param name="spec">The resource's spec</param>
    /// <param name="status">An object that describes the resource's status</param>
    public Resource(ResourceDefinitionInfo definition, ResourceMetadata metadata, TSpec spec, TStatus? status = null)
        : base(definition, metadata, spec)
    {
        this.Status = status;
    }

    /// <summary>
    /// Gets/sets an object that describes the resource's status, if any
    /// </summary>
    [DataMember(Order = 5, Name = "status"), JsonPropertyOrder(5), JsonPropertyName("status"), YamlMember(Order = 5, Alias = "status")]
    public virtual TStatus? Status { get; set; }

    object? IStatus.Status => this.Status;

    object? ISubResource<IStatus>.SubResource => this.Status;

    TStatus? ISubResource<IStatus, TStatus>.SubResource => this.Status;

}
