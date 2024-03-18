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
/// Represents an object used to describe a resource
/// </summary>
[DataContract]
public record ResourceMetadata
    : IExtensible
{

    /// <summary>
    /// Initializes a new <see cref="ResourceMetadata"/>
    /// </summary>
    public ResourceMetadata() { }

    /// <summary>
    /// Initializes a new <see cref="ResourceMetadata"/>
    /// </summary>
    /// <param name="name">The described resource's name</param>
    /// <param name="namespace">The namespace the described resource belongs to</param>
    /// <param name="labels">A key/value mappings of the described resource's labels, if any</param>
    public ResourceMetadata(string name, string? @namespace = null, IDictionary<string, string>? labels = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        if (name.Split('.').Length == 1 || !name.Split('.').All(ObjectNamingConvention.Current.IsValidResourceName)) ObjectNamingConvention.Current.EnsureIsValidResourceName(name);
        if (@namespace != null) ObjectNamingConvention.Current.EnsureIsValidResourceName(@namespace);
        labels?.Keys.ToList().ForEach(ObjectNamingConvention.Current.EnsureIsValidLabelName);
        this.Name = name;
        this.Namespace = @namespace;
        this.CreationTimestamp = DateTimeOffset.Now;
        this.Generation = 0;
        this.Labels = labels;
    }

    /// <summary>
    /// Gets/sets the described resource's name
    /// </summary>
    [DataMember(Order = 1, Name = "name"), JsonPropertyOrder(1), JsonPropertyName("name"), YamlMember(Order = 1, Alias = "name")]
    public virtual string? Name { get; set; }

    /// <summary>
    /// Gets/sets the described resource's namespace
    /// </summary>
    [DataMember(Order = 2, Name = "namespace"), JsonPropertyOrder(2), JsonPropertyName("namespace"), YamlMember(Order = 2, Alias = "namespace")]
    public virtual string? Namespace { get; set; }

    /// <summary>
    /// Gets/sets a key/value mappings of the described resource's labels, if any
    /// </summary>
    [DataMember(Order = 3, Name = "labels"), JsonPropertyOrder(3), JsonPropertyName("labels"), YamlMember(Order = 3, Alias = "labels")]
    public virtual IDictionary<string, string>? Labels { get; set; }

    /// <summary>
    /// Gets/sets a key/value mappings of the described resource's annotations, if any
    /// </summary>
    [DataMember(Order = 4, Name = "annotations"), JsonPropertyOrder(4), JsonPropertyName("annotations"), YamlMember(Order = 4, Alias = "annotations")]
    public virtual IDictionary<string, string>? Annotations { get; set; }

    /// <summary>
    /// Gets/sets the date and time at which the described resource has been created
    /// </summary>
    [DataMember(Order = 5, Name = "creationTimestamp"), JsonPropertyOrder(5), JsonPropertyName("creationTimestamp"), YamlMember(Order = 5, Alias = "creationTimestamp")]
    public virtual DateTimeOffset? CreationTimestamp { get; set; }

    /// <summary>
    /// Gets/sets a value that represents the resource's spec version
    /// </summary>
    [DataMember(Order = 6, Name = "generation"), JsonPropertyOrder(6), JsonPropertyName("generation"), YamlMember(Order = 6, Alias = "generation")]
    public virtual ulong Generation { get; set; }

    /// <summary>
    /// Gets/sets a value that represents the resource's spec version
    /// </summary>
    [DataMember(Order = 7, Name = "resourceVersion"), JsonPropertyOrder(7), JsonPropertyName("resourceVersion"), YamlMember(Order = 7, Alias = "resourceVersion")]
    public virtual string? ResourceVersion { get; set; }

    /// <inheritdoc/>
    [DataMember(Order = 999, Name = "extensionData"), JsonExtensionData]
    public IDictionary<string, object>? ExtensionData { get; set; }

    /// <inheritdoc/>
    public override string? ToString() => string.IsNullOrWhiteSpace(this.Name) ? base.ToString() : string.IsNullOrWhiteSpace(this.Namespace) ? this.Name : $"{this.Namespace}.{this.Name}";

}
