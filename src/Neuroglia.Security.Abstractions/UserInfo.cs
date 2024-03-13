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

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Neuroglia.Security;

/// <summary>
/// Represents an object that holds information about a user
/// </summary>
[DataContract]
public record UserInfo
{

    /// <summary>
    /// Initializes a new <see cref="UserInfo"/>
    /// </summary>
    public UserInfo() { }

    /// <summary>
    /// Initializes a new <see cref="UserInfo"/>
    /// </summary>
    /// <param name="name">The name of the user to describe</param>
    /// <param name="authenticationType">The name of the mechanism used to authenticate the user</param>
    /// <param name="claims">A key/comma-separated values mapping of the claims used to describe the authenticated user</param>
    public UserInfo(string name, string authenticationType, IDictionary<string, string>? claims = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        if (string.IsNullOrWhiteSpace(authenticationType)) throw new ArgumentNullException(nameof(authenticationType));
        this.Name = name;
        this.AuthenticationType = authenticationType;
        this.Claims = claims;
    }

    /// <summary>
    /// Gets/sets the name of the user to describe
    /// </summary>
    [Required]
    [DataMember(Order = 1, Name = "name", IsRequired = true), JsonPropertyOrder(1), JsonPropertyName("name")]
    public virtual string Name { get; set; } = null!;

    /// <summary>
    /// Gets/sets the name of the mechanism used to authenticate the user
    /// </summary>
    [Required]
    [DataMember(Order = 2, Name = "authenticationType", IsRequired = true), JsonPropertyOrder(2), JsonPropertyName("authenticationType")]
    public virtual string AuthenticationType { get; set; } = null!;

    /// <summary>
    /// Gets/sets a key/comma-separated values mapping of the claims used to describe the authenticated user
    /// </summary>
    [DataMember(Order = 3, Name = "claims", IsRequired = true), JsonPropertyOrder(3), JsonPropertyName("claims")]
    public virtual IDictionary<string, string>? Claims { get; set; }

    /// <inheritdoc/>
    public override string ToString() => this.Name;

}