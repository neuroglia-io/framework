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
/// Represents a webhook used to validate <see cref="Resource"/>s
/// </summary>
[DataContract]
public record MutatingWebhook
    : Resource<MutatingWebhookSpec>
{

    /// <summary>
    /// Gets/sets the group mutating webhooks belong to
    /// </summary>
    public static string ResourceGroup => MutatingWebhookDefinition.ResourceGroup;
    /// <summary>
    /// Gets/sets the resource version of mutating webhooks
    /// </summary>
    public static string ResourceVersion => MutatingWebhookDefinition.ResourceVersion;
    /// <summary>
    /// Gets/sets the plural name of mutating webhooks
    /// </summary>
    public static string ResourcePlural => MutatingWebhookDefinition.ResourcePlural;
    /// <summary>
    /// Gets/sets the kind of mutating webhooks
    /// </summary>
    public static string ResourceKind => MutatingWebhookDefinition.ResourceKind;

    /// <summary>
    /// Gets the <see cref="MutatingWebhook"/>'s resource type.
    /// </summary>
    public static ResourceDefinition ResourceDefinition { get; set; } = new MutatingWebhookDefinition();

    /// <summary>
    /// Initializes a new <see cref="MutatingWebhook"/>
    /// </summary>
    public MutatingWebhook() : base(ResourceDefinition!) { }

    /// <summary>
    /// Initializes a new <see cref="MutatingWebhook"/>
    /// </summary>
    /// <param name="metadata">The <see cref="MutatingWebhook"/>'s metadata</param>
    /// <param name="spec">The <see cref="MutatingWebhook"/>'s <see cref="MutatingWebhookSpec"/></param>
    public MutatingWebhook(ResourceMetadata metadata, MutatingWebhookSpec spec)
        : base(ResourceDefinition!, metadata, spec)
    {

    }

}
