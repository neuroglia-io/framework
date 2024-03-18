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
/// Represents the object used to configure the conversion of related <see cref="IResource"/>s to different <see cref="IResourceDefinition"/> versions
/// </summary>
[DataContract]
public record ResourceConversion
{

    /// <summary>
    /// Initializes a new <see cref="ResourceConversion"/>
    /// </summary>
    public ResourceConversion() { }

    /// <summary>
    /// Initializes a new <see cref="ResourceConversion"/>
    /// </summary>
    /// <param name="webhook">The object used to configure the webhook to invoke</param>
    public ResourceConversion(WebhookResourceConversion webhook)
    {
        this.Strategy = ConversionStrategy.Webhook;
        this.Webhook = webhook ?? throw new ArgumentNullException(nameof(webhook));
    }

    /// <summary>
    /// Gets the conversion strategy to use
    /// </summary>
    [DataMember(Name = "strategy", Order = 1), JsonPropertyOrder(1), JsonPropertyName("strategy"), YamlMember(Order = 1, Alias = "strategy")]
    public virtual string? Strategy { get; set; }

    /// <summary>
    /// Gets the object used to configure the webhook to invoke when strategy has been set to 'webhook'
    /// </summary>
    [DataMember(Name = "webhook", Order = 2), JsonPropertyOrder(2), JsonPropertyName("webhook"), YamlMember(Order = 2, Alias = "webhook")]
    public virtual WebhookResourceConversion? Webhook { get; set; }

}
