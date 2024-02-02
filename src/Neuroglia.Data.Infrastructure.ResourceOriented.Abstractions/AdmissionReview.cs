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
/// Represents a review of an operation requested on a specific resource to determine whether or not to allow it
/// </summary>
[DataContract]
public record AdmissionReview
    : IObject
{

    /// <summary>
    /// Gets the resource API group
    /// </summary>
    public static string ResourceGroup { get; set; } = Resource.DefaultResourceGroup;
    /// <summary>
    /// Gets the resource API version
    /// </summary>
    public static string ResourceVersion { get; set; } = "v1";
    /// <summary>
    /// Gets the resource kind
    /// </summary>
    public static string ResourceKind { get; set; } = "AdmissionReview";

    /// <summary>
    /// Initializes a new <see cref="AdmissionReview"/>
    /// </summary>
    public AdmissionReview() { }

    /// <summary>
    /// Initializes a new <see cref="AdmissionReview"/>
    /// </summary>
    /// <param name="request">An object that describes the resource admission review request</param>
    public AdmissionReview(AdmissionReviewRequest request)
    {
        this.Request = request ?? throw new ArgumentNullException(nameof(request));
    }

    /// <summary>
    /// Initializes a new <see cref="AdmissionReview"/>
    /// </summary>
    /// <param name="response">An object that describes the resource admission review response</param>
    public AdmissionReview(AdmissionReviewResponse response)
    {
        this.Response = response ?? throw new ArgumentNullException(nameof(response));
    }

    /// <inheritdoc/>
    [Required]
    [DataMember(Order = 1, Name = "apiVersion", IsRequired = true), JsonPropertyOrder(1), JsonPropertyName("apiVersion"), YamlMember(Order = 1, Alias = "apiVersion")]
    public virtual string ApiVersion { get; set; } = Resource.BuildApiVersion(ResourceGroup, ResourceVersion);

    /// <inheritdoc/>
    [Required]
    [DataMember(Order = 2, Name = "kind", IsRequired = true), JsonPropertyOrder(2), JsonPropertyName("kind"), YamlMember(Order = 2, Alias = "kind")]
    public virtual string Kind { get; set; } = ResourceKind;

    /// <summary>
    /// Gets/sets an object that describes the resource admission review request
    /// </summary>
    [DataMember(Order = 3, Name = "request"), JsonPropertyOrder(3), JsonPropertyName("request"), YamlMember(Order = 3, Alias = "request")]
    public virtual AdmissionReviewRequest? Request { get; set; }

    /// <summary>
    /// Gets/sets an object that describes the resource admission review response
    /// </summary>
    [DataMember(Order = 4, Name = "response"), JsonPropertyOrder(4), JsonPropertyName("response"), YamlMember(Order = 4, Alias = "response")]
    public virtual AdmissionReviewResponse? Response { get; set; }

    /// <inheritdoc/>
    [DataMember(Order = 999, Name = "extensionData"), JsonExtensionData]
    public virtual IDictionary<string, object>? ExtensionData { get; set; }

}
