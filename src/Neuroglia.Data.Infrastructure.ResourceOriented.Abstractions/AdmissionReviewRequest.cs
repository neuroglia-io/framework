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

using Neuroglia.Security;

namespace Neuroglia.Data.Infrastructure.ResourceOriented;

/// <summary>
/// Represents a request to perform a review to determine whether or not an operation can be performed on a specific resource
/// </summary>
[DataContract]
public record AdmissionReviewRequest
    : IExtensible
{

    /// <summary>
    /// Initializes a new <see cref="AdmissionReviewRequest"/>
    /// </summary>
    public AdmissionReviewRequest() { }

    /// <summary>
    /// Initializes a new <see cref="AdmissionReviewRequest"/>
    /// </summary>
    /// <param name="uid">A string that uniquely and globally identifies the resource admission review request</param>
    /// <param name="operation">The operation to perform on the specified resource</param>
    /// <param name="resourceReference">A reference to the resource to perform the admission review for</param>
    /// <param name="subResource">The sub resource the operation to review applies to, if any</param>
    /// <param name="patch">The patch to apply to the (sub)resource being admitted. Null if operation is not 'patch'</param>
    /// <param name="updatedState">The updated state of the (sub)resource being admitted. Null if operation has been set to 'patch' or 'deleted'</param>
    /// <param name="originalState">The original state of the (sub)resource being admitted. Null if operation has been set to 'create'</param>
    /// <param name="user">The information about the authenticated user that has performed the operation that is being admitted</param>
    /// <param name="dryRun">A boolean indicating whether or not admission side effects should be actuated</param>
    public AdmissionReviewRequest(string uid, Operation operation, ResourceReference resourceReference, string? subResource = null, Patch? patch = null, IResource? updatedState = null, IResource? originalState = null, UserInfo? user = null, bool dryRun = false)
    {
        if (string.IsNullOrWhiteSpace(uid)) throw new ArgumentNullException(nameof(uid));
        this.Uid = uid;
        this.Operation = operation;
        this.Resource = resourceReference ?? throw new ArgumentNullException(nameof(resourceReference));
        this.SubResource = subResource;
        this.Patch = patch ?? (operation == Operation.Patch ? throw new ArgumentNullException(nameof(patch)) : null);
        this.UpdatedState = updatedState ?? (operation == Operation.Create || operation == Operation.Replace ? throw new ArgumentNullException(nameof(updatedState)) : null);
        this.OriginalState = originalState ?? (operation != Operation.Create ? throw new ArgumentNullException(nameof(originalState)) : null);
        this.User = user;
        this.DryRun = dryRun;
        DryRun = dryRun;
    }

    /// <summary>
    /// Gets/sets a string that uniquely and globally identifies the resource admission review request
    /// </summary>
    [Required]
    [DataMember(Order = 1, Name = "uid", IsRequired = true), JsonPropertyOrder(1), JsonPropertyName("uid"), YamlMember(Order = 1, Alias = "uid")]
    public virtual string Uid { get; set; } = null!;

    /// <summary>
    /// Gets/sets the operation to perform on the specified resource
    /// </summary>
    [Required]
    [DataMember(Order = 2, Name = "operation", IsRequired = true), JsonPropertyOrder(2), JsonPropertyName("operation"), YamlMember(Order = 2, Alias = "operation")]
    public virtual Operation Operation { get; set; }

    /// <summary>
    /// Gets/sets a reference to the resource to perform the admission review for
    /// </summary>
    [Required]
    [DataMember(Order = 3, Name = "resource", IsRequired = true), JsonPropertyOrder(3), JsonPropertyName("resource"), YamlMember(Order = 3, Alias = "resource")]
    public virtual ResourceReference Resource { get; set; } = null!;

    /// <summary>
    /// Gets/sets the sub resource the operation to review applies to, if any
    /// </summary>
    [DataMember(Order = 4, Name = "subResource"), JsonPropertyOrder(4), JsonPropertyName("subResource"), YamlMember(Order = 4, Alias = "subResource")]
    public virtual string? SubResource { get; set; }

    /// <summary>
    /// Gets/sets the patch to apply to the (sub)resource being admitted. Null if operation is not 'patch'
    /// </summary>
    [DataMember(Order = 5, Name = "patch"), JsonPropertyOrder(5), JsonPropertyName("patch"), YamlMember(Order = 5, Alias = "patch")]
    public virtual Patch? Patch { get; set; }

    /// <summary>
    /// Gets/sets the updated state of the (sub)resource being admitted. Null if operation has been set to 'patch' or 'delete'
    /// </summary>
    [DataMember(Order = 6, Name = "updatedState"), JsonPropertyOrder(6), JsonPropertyName("updatedState"), YamlMember(Order = 6, Alias = "updatedState")]
    public virtual object? UpdatedState { get; set; }

    /// <summary>
    /// Gets/sets the original, unmodified state of the (sub)resource being admitted. Null if operation has been set to 'create'
    /// </summary>
    [DataMember(Order = 7, Name = "originalState"), JsonPropertyOrder(7), JsonPropertyName("originalState"), YamlMember(Order = 7, Alias = "originalState")]
    public virtual IResource? OriginalState { get; set; }

    /// <summary>
    /// Gets/sets information about the authenticated user that has performed the operation that is being admitted
    /// </summary>
    [DataMember(Order = 8, Name = "user"), JsonPropertyOrder(8), JsonPropertyName("user"), YamlMember(Order = 8, Alias = "user")]
    public virtual UserInfo? User { get; set; }

    /// <summary>
    /// Gets/sets a boolean indicating whether or not admission side effects should be actuated
    /// </summary>
    [DataMember(Order = 9, Name = "dryRun"), JsonPropertyOrder(9), JsonPropertyName("dryRun"), YamlMember(Order = 9, Alias = "dryRun")]
    public virtual bool DryRun { get; set; }

    /// <inheritdoc/>
    [DataMember(Order = 999, Name = "extensionData"), JsonExtensionData]
    public virtual IDictionary<string, object>? ExtensionData { get; set; }

}
