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

using Neuroglia.Serialization.Yaml;

namespace Neuroglia.Data.Infrastructure.ResourceOriented;

/// <summary>
/// Represents the definition of validating webhooks
/// </summary>
[DataContract]
public record ValidatingWebhookDefinition
    : ResourceDefinition
{

    static ValidatingWebhookDefinition()
    {
        using var stream = typeof(ResourceDefinition).Assembly.GetManifestResourceStream($"{typeof(ResourceDefinition).Namespace}.validating-webhook.yaml")!;
        using var streamReader = new StreamReader(stream);
        Instance = YamlSerializer.Default.Deserialize<ResourceDefinition>(streamReader.ReadToEnd())!;
    }

    /// <summary>
    /// Gets/sets the group namespaces belong to
    /// </summary>
    public new static string ResourceGroup { get; set; } = DefaultResourceGroup;
    /// <summary>
    /// Gets/sets the resource version of namespaces
    /// </summary>
    public new static string ResourceVersion { get; set; } = "v1";
    /// <summary>
    /// Gets/sets the singular name of namespaces
    /// </summary>
    public static string ResourceSingular { get; set; } = "validating-webhook";
    /// <summary>
    /// Gets/sets the plural name of namespaces
    /// </summary>
    public new static string ResourcePlural { get; set; } = "validating-webhooks";
    /// <summary>
    /// Gets/sets the kind of namespaces
    /// </summary>
    public new static string ResourceKind { get; set; } = "ValidatingWebhook";
    /// <summary>
    /// Gets/sets the short names of validating webhooks
    /// </summary>
    public static HashSet<string> ResourceShortNames { get; set; } = ["vwh"];

    /// <summary>
    /// Gets the definition of <see cref="ValidatingWebhookDefinition"/>s
    /// </summary>
    public static new ResourceDefinition Instance { get; set; }

    /// <summary>
    /// Initializes a new <see cref="ValidatingWebhookDefinition"/>
    /// </summary>
    public ValidatingWebhookDefinition() : base(Instance.Spec) { }

}
