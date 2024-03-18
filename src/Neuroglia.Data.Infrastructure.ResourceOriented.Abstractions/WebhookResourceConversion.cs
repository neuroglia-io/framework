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
/// Represents the object used to configure a webhook-based <see cref="ResourceConversion"/>
/// </summary>
[DataContract]
public record WebhookResourceConversion
{

    /// <summary>
    /// Gets a <see cref="List{T}"/> containing the version supported by the webhook conversion
    /// </summary>
    [Required]
    [DataMember(Name = "supportedVersions", Order = 1), JsonPropertyOrder(1), JsonPropertyName("supportedVersions"), YamlMember(Order = 1, Alias = "supportedVersions")]
    public virtual EquatableList<string>? SupportedVersions { get; set; }

    /// <summary>
    /// Gets the object used to configure the webhook to call
    /// </summary>
    [Required]
    [DataMember(Name = "client", Order = 2), JsonPropertyOrder(2), JsonPropertyName("client"), YamlMember(Order = 2, Alias = "client")]
    public virtual WebhookClientConfiguration Client { get; set; } = null!;

}
