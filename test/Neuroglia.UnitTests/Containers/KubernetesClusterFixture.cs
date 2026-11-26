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

using k8s;
using k8s.Autorest;
using k8s.Models;
using Neuroglia.Data.Infrastructure.ResourceOriented;
using Neuroglia.UnitTests.Cases.Data.Infrastructure.ResourceOriented;
using System.Net;
using System.Text;
using Testcontainers.K3s;

namespace Neuroglia.UnitTests.Containers;

/// <summary>
/// Runs a single-node k3s cluster in a container, and installs the CRDs required to test the Kubernetes database.
/// Only the kubeconfig of the started container is ever used: the machine's kubeconfig and contexts are never read.
/// </summary>
public sealed class KubernetesClusterFixture
    : IAsyncLifetime
{

    /// <summary>
    /// Gets the name of the Kubernetes namespace in which namespaced test resources are created
    /// </summary>
    internal const string NamespacedResourceNamespace = DatabaseTestsBase.FakeNamespaceName;

    readonly K3sContainer _container = new K3sBuilder()
        .WithName($"k3s-{Guid.NewGuid():N}")
        .Build();
    KubernetesClientConfiguration _configuration = null!;

    /// <summary>
    /// Gets the definitions of the custom resources installed on the cluster
    /// </summary>
    internal static IReadOnlyList<(string Group, string Version, string Plural, bool Namespaced)> CustomResources { get; } =
    [
        (ResourceDefinition.ResourceGroup, ResourceDefinition.ResourceVersion, ResourceDefinition.ResourcePlural, false),
        (FakeNamespacedResourceDefinition.ResourceGroup, FakeNamespacedResourceDefinition.ResourceVersion, FakeNamespacedResourceDefinition.ResourcePlural, true),
        (FakeClusterResourceDefinition.ResourceGroup, FakeClusterResourceDefinition.ResourceVersion, FakeClusterResourceDefinition.ResourcePlural, false)
    ];

    /// <summary>
    /// Creates a new <see cref="IKubernetes"/> client connected to the containerized cluster
    /// </summary>
    /// <param name="handlers">The <see cref="DelegatingHandler"/>s, if any, to insert in the client's HTTP pipeline</param>
    /// <returns>A new <see cref="IKubernetes"/> client</returns>
    public IKubernetes CreateClient(params DelegatingHandler[] handlers) => new Kubernetes(this._configuration, handlers);

    /// <inheritdoc/>
    public async Task InitializeAsync()
    {
        await this._container.StartAsync().ConfigureAwait(false);
        var kubeconfig = await this._container.GetKubeconfigAsync().ConfigureAwait(false);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(kubeconfig));
        this._configuration = KubernetesClientConfiguration.BuildConfigFromConfigFile(stream);
        using var client = this.CreateClient();
        await client.CoreV1.CreateNamespaceAsync(new V1Namespace { Metadata = new() { Name = NamespacedResourceNamespace } }).ConfigureAwait(false);
        await this.InstallCustomResourceDefinitionAsync(client, ResourceDefinition.ResourceGroup, ResourceDefinition.ResourceVersion, "resource-definition", ResourceDefinition.ResourcePlural, ResourceDefinition.ResourceKind, false, false, PreserveUnknownFields()).ConfigureAwait(false);
        await this.InstallCustomResourceDefinitionAsync(client, FakeNamespacedResourceDefinition.ResourceGroup, FakeNamespacedResourceDefinition.ResourceVersion, FakeNamespacedResourceDefinition.ResourceSingular, FakeNamespacedResourceDefinition.ResourcePlural, FakeNamespacedResourceDefinition.ResourceKind, true, true, FakeResourceSchema()).ConfigureAwait(false);
        await this.InstallCustomResourceDefinitionAsync(client, FakeClusterResourceDefinition.ResourceGroup, FakeClusterResourceDefinition.ResourceVersion, FakeClusterResourceDefinition.ResourceSingular, FakeClusterResourceDefinition.ResourcePlural, FakeClusterResourceDefinition.ResourceKind, false, true, FakeResourceSchema()).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes all the custom objects stored in the cluster
    /// </summary>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    public async Task ResetAsync()
    {
        using var client = this.CreateClient();
        foreach (var (group, version, plural, namespaced) in CustomResources)
        {
            if (namespaced)
            {
                foreach (var @namespace in new[] { NamespacedResourceNamespace, "default" }) await client.CustomObjects.DeleteCollectionNamespacedCustomObjectAsync(group, version, @namespace, plural).ConfigureAwait(false);
            }
            else await client.CustomObjects.DeleteCollectionClusterCustomObjectAsync(group, version, plural).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public async Task DisposeAsync()
    {
        await this._container.StopAsync().ConfigureAwait(false);
        await this._container.DisposeAsync().ConfigureAwait(false);
    }

    async Task InstallCustomResourceDefinitionAsync(IKubernetes client, string group, string version, string singular, string plural, string kind, bool namespaced, bool statusSubResource, V1JSONSchemaProps schema)
    {
        var name = $"{plural}.{group}";
        await client.ApiextensionsV1.CreateCustomResourceDefinitionAsync(new V1CustomResourceDefinition
        {
            Metadata = new() { Name = name },
            Spec = new()
            {
                Group = group,
                Scope = namespaced ? "Namespaced" : "Cluster",
                Names = new() { Singular = singular, Plural = plural, Kind = kind, ListKind = $"{kind}List" },
                Versions =
                [
                    new()
                    {
                        Name = version,
                        Served = true,
                        Storage = true,
                        Schema = new() { OpenAPIV3Schema = schema },
                        Subresources = statusSubResource ? new() { Status = new { } } : null
                    }
                ]
            }
        }).ConfigureAwait(false);
        var timeout = DateTimeOffset.UtcNow.AddMinutes(1);
        while (true)
        {
            var crd = await client.ApiextensionsV1.ReadCustomResourceDefinitionAsync(name).ConfigureAwait(false);
            if (crd.Status?.Conditions?.Any(c => c.Type == "Established" && c.Status == "True") == true)
            {
                try
                {
                    await client.CustomObjects.ListClusterCustomObjectAsync(group, version, plural).ConfigureAwait(false);
                    return;
                }
                catch (HttpOperationException ex) when (ex.Response.StatusCode == HttpStatusCode.NotFound) { }
            }
            if (DateTimeOffset.UtcNow > timeout) throw new TimeoutException($"The CRD '{name}' has not been established in time");
            await Task.Delay(250).ConfigureAwait(false);
        }
    }

    static V1JSONSchemaProps PreserveUnknownFields() => new() { Type = "object", XKubernetesPreserveUnknownFields = true };

    static V1JSONSchemaProps FakeResourceSchema()
    {
        V1JSONSchemaProps Object() => new()
        {
            Type = "object",
            Properties = new Dictionary<string, V1JSONSchemaProps>
            {
                ["fakeProperty1"] = new() { Type = "string" },
                ["fakeProperty2"] = new() { Type = "number" }
            }
        };
        return new()
        {
            Type = "object",
            Properties = new Dictionary<string, V1JSONSchemaProps>
            {
                ["spec"] = Object(),
                ["status"] = Object()
            }
        };
    }

}
