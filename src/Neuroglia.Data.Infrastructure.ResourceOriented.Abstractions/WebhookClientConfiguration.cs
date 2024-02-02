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
/// Represents the object used to configure a webhook client
/// </summary>
[DataContract]
public record WebhookClientConfiguration
{

    /// <summary>
    /// Initializes a new <see cref="WebhookClientConfiguration"/>
    /// </summary>
    public WebhookClientConfiguration() { }

    /// <summary>
    /// Initializes a new <see cref="WebhookClientConfiguration"/>
    /// </summary>
    /// <param name="uri">The <see cref="System.Uri"/> of the remote service to call</param>
    public WebhookClientConfiguration(Uri uri)
    {
        this.Uri = uri;
    }

    /// <summary>
    /// Gets the <see cref="System.Uri"/> of the remote service to call
    /// </summary>
    [Required]
    [DataMember(Name = "uri", Order = 1), JsonPropertyOrder(1), JsonPropertyName("uri"), YamlMember(Order = 1, Alias = "uri")]
    public virtual Uri Uri { get; set; } = null!;

}