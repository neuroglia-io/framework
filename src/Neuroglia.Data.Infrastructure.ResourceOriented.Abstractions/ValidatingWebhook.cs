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
/// Represents a webhook used to validate <see cref="IResource"/>s
/// </summary>
[DataContract]
public record ValidatingWebhook
    : Resource<ValidatingWebhookSpec>
{

    /// <summary>
    /// Gets/sets the group validating webhooks belong to
    /// </summary>
    public static string ResourceGroup => ValidatingWebhookDefinition.ResourceGroup;
    /// <summary>
    /// Gets/sets the resource version of validating webhooks
    /// </summary>
    public static string ResourceVersion => ValidatingWebhookDefinition.ResourceVersion;
    /// <summary>
    /// Gets/sets the plural name of validating webhooks
    /// </summary>
    public static string ResourcePlural => ValidatingWebhookDefinition.ResourcePlural;
    /// <summary>
    /// Gets/sets the kind of validating webhooks
    /// </summary>
    public static string ResourceKind => ValidatingWebhookDefinition.ResourceKind;

    /// <summary>
    /// Gets the <see cref="ValidatingWebhook"/>'s resource type.
    /// </summary>
    public static ResourceDefinition ResourceDefinition { get; set; } = new ValidatingWebhookDefinition();

    /// <summary>
    /// Initializes a new <see cref="ValidatingWebhook"/>
    /// </summary>
    public ValidatingWebhook() : base(new ResourceDefinitionInfo(ResourceGroup, ResourceVersion, ResourcePlural, ResourceKind)) { }

    /// <summary>
    /// Initializes a new <see cref="ValidatingWebhook"/>
    /// </summary>
    /// <param name="metadata">The <see cref="ValidatingWebhook"/>'s metadata</param>
    /// <param name="spec">The <see cref="ValidatingWebhook"/>'s <see cref="ValidatingWebhookSpec"/></param>
    public ValidatingWebhook(ResourceMetadata metadata, ValidatingWebhookSpec spec)
        : this()
    {
        this.Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
        this.Spec = spec ?? throw new ArgumentNullException(nameof(spec));
    }

}
