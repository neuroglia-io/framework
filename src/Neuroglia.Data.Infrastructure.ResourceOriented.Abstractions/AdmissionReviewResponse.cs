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
/// Represents an object used to describe the result of a resource admission review request
/// </summary>
[DataContract]
public record AdmissionReviewResponse
    : IExtensible
{

    /// <summary>
    /// Initializes a new <see cref="AdmissionReviewResponse"/>
    /// </summary>
    public AdmissionReviewResponse() { }

    /// <summary>
    /// Initializes a new <see cref="AdmissionReviewResponse"/>
    /// </summary>
    /// <param name="uid">A string that uniquely and globally identifies the resource admission review request to respond to</param>
    /// <param name="allowed">A boolean indicating whether or not the requested operation is allowed on the specified resource</param>
    /// <param name="patch">The patch to apply, in case the resource is being mutated</param>
    /// <param name="problem">An object used to describe the problem, if any, that has occured during the resource admission</param>
    public AdmissionReviewResponse(string uid, bool allowed, Patch? patch = null, ProblemDetails? problem = null)
    {
        if (string.IsNullOrWhiteSpace(uid)) throw new ArgumentNullException(nameof(uid));
        this.Uid = uid;
        this.Allowed = allowed;
        this.Patch = patch;
        this.Problem = problem;
    }

    /// <summary>
    /// Gets/sets a string that uniquely and globally identifies the resource admission review request to respond to
    /// </summary>
    [Required]
    [DataMember(Order = 1, Name = "uid", IsRequired = true), JsonPropertyOrder(1), JsonPropertyName("uid"), YamlMember(Order = 1, Alias = "uid")]
    public virtual string Uid { get; set; } = null!;

    /// <summary>
    /// Gets/sets a boolean indicating whether or not the requested operation is allowed on the specified resource
    /// </summary>
    [Required]
    [DataMember(Order = 2, Name = "allowed", IsRequired = true), JsonPropertyOrder(2), JsonPropertyName("allowed"), YamlMember(Order = 2, Alias = "allowed")]
    public virtual bool Allowed { get; set; }

    /// <summary>
    /// Gets/sets the patch to apply, in case the resource is being mutated
    /// </summary>
    [DataMember(Order = 3, Name = "patch", IsRequired = true), JsonPropertyOrder(3), JsonPropertyName("patch"), YamlMember(Order = 3, Alias = "patch")]
    public virtual Patch? Patch { get; set; }

    /// <summary>
    /// Gets/sets an object used to describe the problem, if any, that has occured during the resource admission
    /// </summary>
    [DataMember(Order = 4, Name = "problem"), JsonPropertyOrder(4), JsonPropertyName("problem"), YamlMember(Order = 4, Alias = "problem")]
    public virtual ProblemDetails? Problem { get; set; }

    /// <inheritdoc/>
    [DataMember(Order = 999, Name = "extensionData"), JsonExtensionData]
    public virtual IDictionary<string, object>? ExtensionData { get; set; }

}