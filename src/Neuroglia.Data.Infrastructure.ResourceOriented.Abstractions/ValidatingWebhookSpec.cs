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
/// Represents the specification of a <see cref="ValidatingWebhook"/>'s 
/// </summary>
[DataContract]
public record ValidatingWebhookSpec
{

    /// <summary>
    /// Initializes a new <see cref="ValidatingWebhookSpec"/>
    /// </summary>
    public ValidatingWebhookSpec() { }

    /// <summary>
    /// Initializes a new <see cref="ValidatingWebhookSpec"/>
    /// </summary>
    /// <param name="client">The <see cref="WebhookClientConfiguration"/> used to configure the webhook to call</param>
    /// <param name="resources">An <see cref="IEnumerable{T}"/> containing the filters used to configure when to call the <see cref="ValidatingWebhook"/></param>
    /// <param name="priority">The <see cref="ValidatingWebhook"/>'s priority</param>
    public ValidatingWebhookSpec(WebhookClientConfiguration client, IEnumerable<RuleWithOperation>? resources = null, long? priority = null)
    {
        this.Client = client ?? throw new ArgumentNullException(nameof(client));
        this.Resources = resources?.WithValueSemantics();
        this.Priority = priority;
    }

    /// <summary>
    /// Gets the <see cref="WebhookClientConfiguration"/> used to configure the webhook to call
    /// </summary>
    [Required]
    [DataMember(Name = "client", Order = 1), JsonPropertyOrder(1), JsonPropertyName("client"), YamlMember(Order = 1, Alias = "client")]
    public virtual WebhookClientConfiguration Client { get; set; } = null!;

    /// <summary>
    /// Gets a <see cref="List{T}"/> containing the filters used to configure when to call the <see cref="ValidatingWebhook"/>
    /// </summary>
    [DataMember(Name = "resources", Order = 2), JsonPropertyOrder(2), JsonPropertyName("resources"), YamlMember(Order = 2, Alias = "resources")]
    public virtual EquatableList<RuleWithOperation>? Resources { get; set; }

    /// <summary>
    /// Gets the <see cref="ValidatingWebhook"/>'s priority
    /// </summary>
    [DataMember(Name = "priority", Order = 3), JsonPropertyOrder(3), JsonPropertyName("priority"), YamlMember(Order = 3, Alias = "priority")]
    public virtual long? Priority { get; set; }

}
