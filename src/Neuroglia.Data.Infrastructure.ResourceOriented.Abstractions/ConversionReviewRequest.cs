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
/// Represents a request to convert the version of a resource
/// </summary>
[DataContract]
public record ConversionReviewRequest
{

    /// <summary>
    /// Initializes a new <see cref="ConversionReviewRequest"/>
    /// </summary>
    public ConversionReviewRequest() { }

    /// <summary>
    /// Initializes a new <see cref="ConversionReviewRequest"/>
    /// </summary>
    /// <param name="uid">The globally unique identifier of the conversion request</param>
    /// <param name="desiredApiVersion">The version to convert the resource to</param>
    /// <param name="resource">The resource to convert</param>
    public ConversionReviewRequest(string uid, string desiredApiVersion, IResource resource)
    {
        if(string.IsNullOrWhiteSpace(uid)) throw new ArgumentNullException(nameof(uid));
        if (string.IsNullOrWhiteSpace(desiredApiVersion)) throw new ArgumentNullException(nameof(desiredApiVersion));
        if (string.IsNullOrWhiteSpace(uid)) throw new ArgumentNullException(nameof(uid));
        this.Uid = uid;
        this.DesiredApiVersion = desiredApiVersion;
        this.Resource = resource ?? throw new ArgumentNullException(nameof(resource));
    }

    /// <summary>
    /// Gets the globally unique identifier of the conversion request
    /// </summary>
    [Required]
    [DataMember(Name = "uid", Order = 1, IsRequired = true), JsonPropertyOrder(1), JsonPropertyName("uid"), YamlMember(Order = 1, Alias = "uid")]
    public virtual string Uid { get; set; } = null!;

    /// <summary>
    /// Gets the version to convert the resource to
    /// </summary>
    [Required]
    [DataMember(Name = "desiredApiVersion", Order = 2, IsRequired = true), JsonPropertyOrder(2), JsonPropertyName("desiredApiVersion"), YamlMember(Order = 2, Alias = "desiredApiVersion")]
    public virtual string DesiredApiVersion { get; set; } = null!;

    /// <summary>
    /// Gets the resource to convert
    /// </summary>
    [Required]
    [DataMember(Name = "resource", Order = 3, IsRequired = true), JsonPropertyOrder(3), JsonPropertyName("resource"), YamlMember(Order = 3, Alias = "resource")]
    public virtual IResource Resource { get; set; } = null!;

}

