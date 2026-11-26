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

using k8s.Autorest;
using k8s.Models;
using Microsoft.Extensions.Options;
using Neuroglia.Data.Infrastructure.ResourceOriented.Configuration;
using Neuroglia.Data.Infrastructure.ResourceOriented.Properties;
using System.Diagnostics;
using System.Net;
using System.Reactive.Subjects;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Neuroglia.Data.Infrastructure.ResourceOriented.Services;

/// <summary>
/// Represents a Kubernetes implementation of the <see cref="IDatabase"/> interface.
/// </summary>
/// <remarks>
/// <para>Resources are stored as Kubernetes custom objects. The custom resource definitions (CRDs) of all the resource types to store, including the one of <see cref="ResourceDefinition"/>s if the <see cref="IResourceRepository"/> is used, must be installed beforehand: the <see cref="KubernetesDatabase"/> never installs CRDs, since applications should not hold that right.</para>
/// <para><see cref="Namespace"/>s are mapped to Kubernetes namespaces, in which namespaced resources are stored.</para>
/// <para>Because Kubernetes ignores the status of a custom resource on creation when its status sub-resource is enabled, the status of a resource being created, if any, is persisted with a subsequent status update, which produces an additional <see cref="ResourceWatchEventType.Updated"/> watch event.</para>
/// </remarks>
/// <param name="logger">The service used to perform logging</param>
/// <param name="kubernetes">The service used to interact with the Kubernetes API</param>
/// <param name="jsonSerializer">The service used to serialize/deserialize data to/from JSON</param>
/// <param name="options">The service used to access the current <see cref="KubernetesDatabaseOptions"/>, if any</param>
public class KubernetesDatabase(ILogger<KubernetesDatabase> logger, IKubernetes kubernetes, IJsonSerializer jsonSerializer, IOptions<KubernetesDatabaseOptions>? options = null)
    : IDatabase
{

    /// <summary>
    /// Gets the name of the only sub-resource supported by the <see cref="KubernetesDatabase"/>
    /// </summary>
    public const string StatusSubResource = "status";
    /// <summary>
    /// Gets the <see cref="Uri"/> that references a problem returned by the Kubernetes API that has no resource-oriented equivalent
    /// </summary>
    public static readonly Uri KubernetesApiErrorProblemType = new("https://neuroglia.io/docs/problems/resources/kubernetes-api-error");
    const string DryRunAll = "All";
    const string ResourceVersionPath = "/metadata/resourceVersion";

    bool _disposed;

    /// <summary>
    /// Gets the current <see cref="ILogger"/>.
    /// </summary>
    protected ILogger Logger { get; } = logger;

    /// <summary>
    /// Gets the service used to interact with the Kubernetes API.
    /// </summary>
    protected IKubernetes Kubernetes { get; } = kubernetes;

    /// <summary>
    /// Gets the service used to serialize/deserialize data to/from JSON.
    /// </summary>
    protected IJsonSerializer JsonSerializer { get; } = jsonSerializer;

    /// <summary>
    /// Gets the options used to configure the <see cref="KubernetesDatabase"/>
    /// </summary>
    protected KubernetesDatabaseOptions Options { get; } = options?.Value ?? new();

    /// <summary>
    /// Gets the <see cref="KubernetesDatabase"/>'s <see cref="System.Threading.CancellationTokenSource"/>, which is canceled when the <see cref="KubernetesDatabase"/> is disposed of, stopping all pending watches
    /// </summary>
    protected CancellationTokenSource CancellationTokenSource { get; } = new();

    /// <inheritdoc/>
    /// <remarks>
    /// <para>Verifies that the Kubernetes API server is reachable and that the configured credentials are accepted, so that a misconfiguration fails the application at startup rather than at the first operation. Never installs CRDs.</para>
    /// <para>As does the Redis implementation, it then ensures that the <see cref="NamespaceDefinition"/> and a <see cref="Namespace"/> exist (Kubernetes always has a 'default' namespace), provided that the CRD of resource definitions is installed, and returns true if any of them had to be created, which instructs the <see cref="IDatabaseInitializer"/> to seed the database. If that CRD is not installed, it logs a warning and returns false.</para>
    /// </remarks>
    public virtual async Task<bool> InitializeAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var versionInfo = await this.Kubernetes.Version.GetCodeAsync(cancellationToken).ConfigureAwait(false);
            this.Logger.LogDebug("Connected to Kubernetes API server version {version}", versionInfo.GitVersion);
        }
        catch (HttpOperationException ex)
        {
            throw await this.CreateExceptionAsync(ex, new(NamespaceDefinition.ResourceGroup, NamespaceDefinition.ResourceVersion, NamespaceDefinition.ResourcePlural), cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        var initialized = false;
        try
        {
            if (await this.GetDefinitionAsync<Namespace>(cancellationToken).ConfigureAwait(false) == null)
            {
                initialized = await this.TryCreateAsync(new NamespaceDefinition(), cancellationToken).ConfigureAwait(false) || initialized;
            }
            if ((await this.ListResourcesAsync<Namespace>(maxResults: 1, cancellationToken: cancellationToken).ConfigureAwait(false)).Items?.Count is null or 0)
            {
                initialized = await this.TryCreateAsync(new Namespace(Namespace.DefaultNamespaceName), cancellationToken).ConfigureAwait(false) || initialized;
            }
        }
        catch (ProblemDetailsException ex) when (ex.Problem.Type == ProblemTypes.DefinitionNotFound)
        {
            this.Logger.LogWarning("The CRD of resource definitions is not installed on the Kubernetes cluster: the namespace definition has not been created. Install it if resources are to be managed with an IResourceRepository. Details: {details}", ex.Problem.Detail);
            return false;
        }
        return initialized;
    }

    /// <inheritdoc/>
    public virtual async Task<IResource> CreateResourceAsync(IResource resource, string group, string version, string plural, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(resource);
        ArgumentException.ThrowIfNullOrWhiteSpace(group);
        ArgumentException.ThrowIfNullOrWhiteSpace(version);
        ArgumentException.ThrowIfNullOrWhiteSpace(plural);
        var definition = new ResourceDefinitionReference(group, version, plural);
        var node = this.SerializeResource(resource);
        if (node["metadata"] is JsonObject metadata && metadata["name"]?.GetValue<string>()?.Trim() is string name && name.EndsWith('-'))
        {
            metadata.Remove("name");
            metadata["generateName"] = name;
        }
        var status = node[StatusSubResource]?.DeepClone();
        JsonObject result;
        try
        {
            result = await this.CreateObjectAsync(node, group, version, plural, @namespace, dryRun, cancellationToken).ConfigureAwait(false);
        }
        catch (HttpOperationException ex)
        {
            throw await this.CreateExceptionAsync(ex, definition, resource.Metadata?.Name, @namespace, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        if (status != null && !this.IsNamespace(group, plural) && !JsonNode.DeepEquals(status, result[StatusSubResource]))
        {
            result[StatusSubResource] = status;
            if (!dryRun)
            {
                var createdName = result["metadata"]?["name"]?.GetValue<string>()!;
                try
                {
                    result = await this.ReplaceObjectAsync(result, group, version, plural, createdName, StatusSubResource, @namespace, false, cancellationToken).ConfigureAwait(false);
                }
                catch (HttpOperationException ex) when (ex.Response?.StatusCode == HttpStatusCode.NotFound)
                {
                    this.Logger.LogDebug("The status of the resource '{resource}' could not be persisted because the status sub-resource of '{plural}.{group}/{version}' is not enabled", createdName, plural, group, version);
                    result.Remove(StatusSubResource);
                }
                catch (HttpOperationException ex)
                {
                    throw await this.CreateExceptionAsync(ex, definition, createdName, @namespace, StatusSubResource, cancellationToken: cancellationToken).ConfigureAwait(false);
                }
            }
        }
        return (IResource)this.JsonSerializer.Deserialize(result, resource.GetType())!;
    }

    /// <inheritdoc/>
    /// <remarks>As does the Redis implementation, returns null when the resource, or its type, does not exist</remarks>
    public virtual async Task<IResource?> GetResourceAsync(string group, string version, string plural, string name, string? @namespace = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(group);
        ArgumentException.ThrowIfNullOrWhiteSpace(version);
        ArgumentException.ThrowIfNullOrWhiteSpace(plural);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        try
        {
            return this.JsonSerializer.Deserialize<Resource>(await this.GetObjectAsync(group, version, plural, name, @namespace, cancellationToken).ConfigureAwait(false));
        }
        catch (HttpOperationException ex) when (ex.Response?.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        catch (HttpOperationException ex)
        {
            throw await this.CreateExceptionAsync(ex, new(group, version, plural), name, @namespace, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public virtual async Task<ICollection> ListResourcesAsync(string group, string version, string plural, string? @namespace = null, IEnumerable<LabelSelector>? labelSelectors = null, ulong? maxResults = null, string? continuationToken = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(group);
        ArgumentException.ThrowIfNullOrWhiteSpace(version);
        ArgumentException.ThrowIfNullOrWhiteSpace(plural);
        var limit = maxResults is null or 0 ? (int?)null : (int)Math.Min(maxResults.Value, int.MaxValue);
        return await this.ListPageAsync(group, version, plural, @namespace, this.BuildLabelSelector(labelSelectors), limit, string.IsNullOrWhiteSpace(continuationToken) ? null : continuationToken, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    /// <remarks>Resources are fetched page by page, using the configured <see cref="KubernetesDatabaseOptions.PageSize"/></remarks>
    public virtual async IAsyncEnumerable<IResource> GetResourcesAsync(string group, string version, string plural, string? @namespace = null, IEnumerable<LabelSelector>? labelSelectors = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(group);
        ArgumentException.ThrowIfNullOrWhiteSpace(version);
        ArgumentException.ThrowIfNullOrWhiteSpace(plural);
        var labelSelector = this.BuildLabelSelector(labelSelectors);
        string? continuationToken = null;
        do
        {
            var page = await this.ListPageAsync(group, version, plural, @namespace, labelSelector, this.GetPageSize(), continuationToken, cancellationToken).ConfigureAwait(false);
            if (page.Items != null) foreach (var item in page.Items.OfType<IResource>()) yield return item;
            continuationToken = page.Metadata?.Continue;
        }
        while (!string.IsNullOrWhiteSpace(continuationToken));
    }

    /// <inheritdoc/>
    /// <remarks>
    /// <para>The returned watch only produces events for changes occurring after it has been created, as does the Redis implementation. It lists the matching resources, then watches them from the resource version of that list.</para>
    /// <para>The watch survives the API server closing the connection, which it does every few minutes, and transient errors: it resumes from the last resource version it has seen, and, when that version is too old (410 Gone), lists the resources again and produces the events required to reflect the changes it missed. It completes only once disposed of.</para>
    /// </remarks>
    public virtual async Task<IResourceWatch> WatchResourcesAsync(string group, string version, string plural, string? @namespace = null, IEnumerable<LabelSelector>? labelSelectors = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(group);
        ArgumentException.ThrowIfNullOrWhiteSpace(version);
        ArgumentException.ThrowIfNullOrWhiteSpace(plural);
        ObjectDisposedException.ThrowIf(this._disposed, this);
        var labelSelector = this.BuildLabelSelector(labelSelectors);
        var state = new Dictionary<string, Resource>();
        var resourceVersion = await this.SynchronizeWatchStateAsync(group, version, plural, @namespace, labelSelector, state, null, cancellationToken).ConfigureAwait(false);
        var subject = new Subject<IResourceWatchEvent>();
        var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(this.CancellationTokenSource.Token);
        var task = Task.Run(() => this.RunWatchAsync(group, version, plural, @namespace, labelSelector, resourceVersion, state, subject, cancellationTokenSource.Token), CancellationToken.None);
        return new ResourceWatch(new KubernetesResourceWatchObservable(subject, cancellationTokenSource, task), true);
    }

    /// <inheritdoc/>
    /// <remarks>If a resource version is specified, the patch is only applied if it matches the resource's current version, otherwise a conflict is reported</remarks>
    public virtual Task<IResource> PatchResourceAsync(Patch patch, string group, string version, string plural, string name, string? @namespace = null, string? resourceVersion = null, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        return this.PatchAsync(patch, group, version, plural, name, null, @namespace, resourceVersion, dryRun, cancellationToken);
    }

    /// <inheritdoc/>
    /// <remarks>The resource's '.metadata.resourceVersion' must match its current version, otherwise a conflict is reported</remarks>
    public virtual Task<IResource> ReplaceResourceAsync(IResource resource, string group, string version, string plural, string name, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        return this.ReplaceAsync(resource, group, version, plural, name, null, @namespace, dryRun, cancellationToken);
    }

    /// <inheritdoc/>
    /// <remarks>Only the 'status' sub-resource is supported. If a resource version is specified, the patch is only applied if it matches the resource's current version, otherwise a conflict is reported</remarks>
    public virtual Task<IResource> PatchSubResourceAsync(Patch patch, string group, string version, string plural, string name, string subResource, string? @namespace = null, string? resourceVersion = null, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        this.EnsureSubResourceIsSupported(group, version, plural, name, subResource, @namespace);
        return this.PatchAsync(patch, group, version, plural, name, StatusSubResource, @namespace, resourceVersion, dryRun, cancellationToken);
    }

    /// <inheritdoc/>
    /// <remarks>Only the 'status' sub-resource is supported. The resource's '.metadata.resourceVersion' must match its current version, otherwise a conflict is reported</remarks>
    public virtual Task<IResource> ReplaceSubResourceAsync(IResource resource, string group, string version, string plural, string name, string subResource, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        this.EnsureSubResourceIsSupported(group, version, plural, name, subResource, @namespace);
        return this.ReplaceAsync(resource, group, version, plural, name, StatusSubResource, @namespace, dryRun, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async Task<IResource> DeleteResourceAsync(string group, string version, string plural, string name, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(group);
        ArgumentException.ThrowIfNullOrWhiteSpace(version);
        ArgumentException.ThrowIfNullOrWhiteSpace(plural);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        try
        {
            return this.JsonSerializer.Deserialize<Resource>(await this.DeleteObjectAsync(group, version, plural, name, @namespace, dryRun, cancellationToken).ConfigureAwait(false))!;
        }
        catch (HttpOperationException ex)
        {
            throw await this.CreateExceptionAsync(ex, new(group, version, plural), name, @namespace, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Patches the specified resource or its status
    /// </summary>
    /// <param name="patch">The patch to apply</param>
    /// <param name="group">The API group the resource to patch belongs to</param>
    /// <param name="version">The version of the type of resource to patch</param>
    /// <param name="plural">The plural name of the type of resource to patch</param>
    /// <param name="name">The name of the resource to patch</param>
    /// <param name="subResource">The sub-resource to patch, if any. Only 'status' is supported</param>
    /// <param name="namespace">The namespace the resource to patch belongs to, if any</param>
    /// <param name="resourceVersion">The expected resource version, if any</param>
    /// <param name="dryRun">A boolean indicating whether or not to persist the changes resulting from the operation</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The patched resource</returns>
    protected virtual async Task<IResource> PatchAsync(Patch patch, string group, string version, string plural, string name, string? subResource, string? @namespace, string? resourceVersion, bool dryRun, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(patch);
        ArgumentException.ThrowIfNullOrWhiteSpace(group);
        ArgumentException.ThrowIfNullOrWhiteSpace(version);
        ArgumentException.ThrowIfNullOrWhiteSpace(plural);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        var definition = new ResourceDefinitionReference(group, version, plural);
        resourceVersion = string.IsNullOrWhiteSpace(resourceVersion) ? null : resourceVersion;
        for (var attempt = 1; ; attempt++)
        {
            var (body, patchType, expectedResourceVersion) = await this.BuildPatchAsync(patch, definition, name, subResource, @namespace, resourceVersion, cancellationToken).ConfigureAwait(false);
            try
            {
                var result = await this.PatchObjectAsync(new V1Patch(body.ToJsonString(), patchType), group, version, plural, name, subResource, @namespace, dryRun, cancellationToken).ConfigureAwait(false);
                return this.JsonSerializer.Deserialize<Resource>(result)!;
            }
            catch (HttpOperationException ex) when (ex.Response?.StatusCode == HttpStatusCode.Conflict && resourceVersion == null && expectedResourceVersion != null && attempt < this.Options.MaxPatchAttempts)
            {
                this.Logger.LogDebug("The resource '{name}' has been modified while being patched; retrying (attempt {attempt})", name, attempt);
            }
            catch (HttpOperationException ex)
            {
                throw await this.CreateExceptionAsync(ex, definition, name, @namespace, subResource, expectedResourceVersion, cancellationToken).ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Builds the body of the request used to apply the specified <see cref="Patch"/>
    /// </summary>
    /// <param name="patch">The <see cref="Patch"/> to apply</param>
    /// <param name="definition">A reference to the definition of the resource to patch</param>
    /// <param name="name">The name of the resource to patch</param>
    /// <param name="subResource">The sub-resource to patch, if any</param>
    /// <param name="namespace">The namespace the resource to patch belongs to, if any</param>
    /// <param name="resourceVersion">The expected resource version, if any</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new object that describes the request body, the Kubernetes patch type and the resource version, if any, that the API server must check before applying the patch</returns>
    /// <remarks>
    /// <para>JSON patches and JSON merge patches are sent as is. When a resource version is specified, the patch also sets '.metadata.resourceVersion' to it, which the API server enforces as a precondition, failing with a conflict on mismatch.</para>
    /// <para>Kubernetes does not support strategic merge patches on custom resources: such patches are applied to the current state of the resource, and the resulting changes are sent as a JSON patch that is conditioned by the resource version they are based on.</para>
    /// </remarks>
    protected virtual async Task<(JsonNode Body, V1Patch.PatchType Type, string? ResourceVersion)> BuildPatchAsync(Patch patch, ResourceDefinitionReference definition, string name, string? subResource, string? @namespace, string? resourceVersion, CancellationToken cancellationToken)
    {
        var reference = this.BuildReference(definition, name, @namespace, subResource);
        var document = this.ToJsonNode(patch.Document) ?? throw new ProblemDetailsException(ResourceProblemDetails.InvalidResourcePatch(reference));
        switch (patch.Type)
        {
            case PatchType.JsonPatch:
                if (document is not JsonArray operations) throw new ProblemDetailsException(ResourceProblemDetails.InvalidResourcePatch(reference));
                if (resourceVersion != null) operations.Add(this.BuildResourceVersionOperation(resourceVersion));
                return (operations, V1Patch.PatchType.JsonPatch, resourceVersion);
            case PatchType.JsonMergePatch:
                if (document is not JsonObject mergePatch) throw new ProblemDetailsException(ResourceProblemDetails.InvalidResourcePatch(reference));
                if (resourceVersion != null)
                {
                    if (mergePatch["metadata"] is not JsonObject metadata)
                    {
                        metadata = [];
                        mergePatch["metadata"] = metadata;
                    }
                    metadata["resourceVersion"] = resourceVersion;
                }
                return (mergePatch, V1Patch.PatchType.MergePatch, resourceVersion);
            case PatchType.JsonStrategicMergePatch:
                var original = await this.GetResourceAsync(definition.Group, definition.Version, definition.Plural, name, @namespace, cancellationToken).ConfigureAwait(false) ?? throw new ProblemDetailsException(ResourceProblemDetails.ResourceNotFound(reference));
                var currentResourceVersion = original.Metadata.ResourceVersion!;
                if (resourceVersion != null && resourceVersion != currentResourceVersion) throw new ProblemDetailsException(ResourceProblemDetails.ResourceOptimisticConcurrencyCheckFailed(reference, resourceVersion, currentResourceVersion));
                var originalNode = this.JsonSerializer.SerializeToNode<object>(original)!;
                var patchedNode = JsonStrategicMergePatch.ApplyPatch(originalNode.DeepClone(), document);
                var jsonPatch = this.ToJsonNode(JsonPatchUtility.CreateJsonPatchFromDiff(originalNode, patchedNode)) as JsonArray ?? [];
                jsonPatch.Add(this.BuildResourceVersionOperation(currentResourceVersion));
                return (jsonPatch, V1Patch.PatchType.JsonPatch, currentResourceVersion);
            default:
                throw new ProblemDetailsException(ResourceProblemDetails.InvalidResourcePatch(reference));
        }
    }

    /// <summary>
    /// Replaces the specified resource or its status
    /// </summary>
    /// <param name="resource">The state to replace the resource with</param>
    /// <param name="group">The API group the resource to replace belongs to</param>
    /// <param name="version">The version of the type of resource to replace</param>
    /// <param name="plural">The plural name of the type of resource to replace</param>
    /// <param name="name">The name of the resource to replace</param>
    /// <param name="subResource">The sub-resource to replace, if any. Only 'status' is supported</param>
    /// <param name="namespace">The namespace the resource to replace belongs to, if any</param>
    /// <param name="dryRun">A boolean indicating whether or not to persist the changes resulting from the operation</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The replaced resource</returns>
    protected virtual async Task<IResource> ReplaceAsync(IResource resource, string group, string version, string plural, string name, string? subResource, string? @namespace, bool dryRun, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(resource);
        ArgumentException.ThrowIfNullOrWhiteSpace(group);
        ArgumentException.ThrowIfNullOrWhiteSpace(version);
        ArgumentException.ThrowIfNullOrWhiteSpace(plural);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        var definition = new ResourceDefinitionReference(group, version, plural);
        var reference = this.BuildReference(definition, name, @namespace, subResource);
        var resourceVersion = resource.Metadata?.ResourceVersion;
        if (string.IsNullOrWhiteSpace(resourceVersion))
        {
            var current = await this.GetResourceAsync(group, version, plural, name, @namespace, cancellationToken).ConfigureAwait(false) ?? throw new ProblemDetailsException(ResourceProblemDetails.ResourceNotFound(reference));
            throw new ProblemDetailsException(ResourceProblemDetails.ResourceOptimisticConcurrencyCheckFailed(reference, string.Empty, current.Metadata.ResourceVersion!));
        }
        var node = this.SerializeResource(resource);
        try
        {
            return this.JsonSerializer.Deserialize<Resource>(await this.ReplaceObjectAsync(node, group, version, plural, name, subResource, @namespace, dryRun, cancellationToken).ConfigureAwait(false))!;
        }
        catch (HttpOperationException ex)
        {
            throw await this.CreateExceptionAsync(ex, definition, name, @namespace, subResource, resourceVersion, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Lists a page of resources
    /// </summary>
    /// <param name="group">The API group the resources to list belong to</param>
    /// <param name="version">The version of the type of resources to list</param>
    /// <param name="plural">The plural name of the type of resources to list</param>
    /// <param name="namespace">The namespace the resources to list belong to, if any. If not set, lists resources across all namespaces</param>
    /// <param name="labelSelector">The Kubernetes label selector used to filter resources, if any</param>
    /// <param name="limit">The maximum amount of resources to list, if any</param>
    /// <param name="continuationToken">The token used to continue a paged list, if any</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="Collection"/> that contains the listed resources</returns>
    protected virtual async Task<Collection> ListPageAsync(string group, string version, string plural, string? @namespace, string? labelSelector, int? limit, string? continuationToken, CancellationToken cancellationToken)
    {
        JsonObject node;
        try
        {
            node = await this.ListObjectsAsync(group, version, plural, @namespace, labelSelector, limit, continuationToken, cancellationToken).ConfigureAwait(false);
        }
        catch (HttpOperationException ex)
        {
            throw await this.CreateExceptionAsync(ex, new(group, version, plural), cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        var metadata = node["metadata"] is JsonObject metadataNode ? this.JsonSerializer.Deserialize<CollectionMetadata>(metadataNode)! : new CollectionMetadata();
        if (string.IsNullOrWhiteSpace(metadata.Continue)) metadata.Continue = null;
        var items = node["items"] is JsonArray itemsNode ? itemsNode.Where(i => i != null).Select(i => (object)this.JsonSerializer.Deserialize<Resource>(i!)!).ToList() : [];
        return new Collection(node["apiVersion"]?.GetValue<string>() ?? Resource.BuildApiVersion(group, version), node["kind"]?.GetValue<string>() ?? string.Empty, metadata, items);
    }

    /// <summary>
    /// Lists all the resources to watch and updates the specified watch state accordingly
    /// </summary>
    /// <param name="group">The API group the resources to watch belong to</param>
    /// <param name="version">The version of the type of resources to watch</param>
    /// <param name="plural">The plural name of the type of resources to watch</param>
    /// <param name="namespace">The namespace the resources to watch belong to, if any</param>
    /// <param name="labelSelector">The Kubernetes label selector used to filter resources, if any</param>
    /// <param name="state">A name/resource mapping of the last known state of the watched resources, which only retains their identifying metadata</param>
    /// <param name="observer">The <see cref="IObserver{T}"/> to notify about the differences between the last known and the current state, if any</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The resource version from which to watch resources</returns>
    protected virtual async Task<string> SynchronizeWatchStateAsync(string group, string version, string plural, string? @namespace, string? labelSelector, IDictionary<string, Resource> state, IObserver<IResourceWatchEvent>? observer, CancellationToken cancellationToken)
    {
        var current = new Dictionary<string, Resource>();
        string? resourceVersion = null;
        string? continuationToken = null;
        do
        {
            var page = await this.ListPageAsync(group, version, plural, @namespace, labelSelector, this.GetPageSize(), continuationToken, cancellationToken).ConfigureAwait(false);
            resourceVersion ??= page.Metadata?.ResourceVersion;
            if (page.Items != null)
            {
                foreach (var resource in page.Items.OfType<Resource>())
                {
                    var key = this.BuildWatchStateKey(resource);
                    current[key] = resource;
                    if (observer == null) continue;
                    if (!state.TryGetValue(key, out var known)) observer.OnNext(new ResourceWatchEvent(ResourceWatchEventType.Created, resource));
                    else if (known.Metadata.ResourceVersion != resource.Metadata.ResourceVersion) observer.OnNext(new ResourceWatchEvent(ResourceWatchEventType.Updated, resource));
                }
            }
            continuationToken = page.Metadata?.Continue;
        }
        while (!string.IsNullOrWhiteSpace(continuationToken));
        if (observer != null) foreach (var deleted in state.Where(e => !current.ContainsKey(e.Key)).Select(e => e.Value).ToList()) observer.OnNext(new ResourceWatchEvent(ResourceWatchEventType.Deleted, deleted));
        state.Clear();
        foreach (var entry in current) state[entry.Key] = this.BuildWatchStateEntry(entry.Value);
        return resourceVersion ?? throw new InvalidOperationException($"The Kubernetes API server did not return the resource version of the list of '{plural}.{group}/{version}'");
    }

    /// <summary>
    /// Runs the loop that watches resources until canceled, reconnecting whenever the API server closes the watch or a transient error occurs
    /// </summary>
    /// <param name="group">The API group the resources to watch belong to</param>
    /// <param name="version">The version of the type of resources to watch</param>
    /// <param name="plural">The plural name of the type of resources to watch</param>
    /// <param name="namespace">The namespace the resources to watch belong to, if any</param>
    /// <param name="labelSelector">The Kubernetes label selector used to filter resources, if any</param>
    /// <param name="resourceVersion">The resource version from which to start watching</param>
    /// <param name="state">A name/resource mapping of the last known state of the watched resources</param>
    /// <param name="subject">The <see cref="ISubject{T}"/> used to stream watch events</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to stop the watch</param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual async Task RunWatchAsync(string group, string version, string plural, string? @namespace, string? labelSelector, string resourceVersion, IDictionary<string, Resource> state, ISubject<IResourceWatchEvent> subject, CancellationToken cancellationToken)
    {
        var delay = this.Options.WatchMinReconnectDelay;
        var relist = false;
        var timeoutSeconds = this.Options.WatchTimeout.HasValue ? (int?)Math.Max(1, (int)Math.Ceiling(this.Options.WatchTimeout.Value.TotalSeconds)) : null;
        while (!cancellationToken.IsCancellationRequested)
        {
            var stopwatch = Stopwatch.StartNew();
            var eventCount = 0;
            Exception? watchError = null;
            try
            {
                if (relist)
                {
                    resourceVersion = await this.SynchronizeWatchStateAsync(group, version, plural, @namespace, labelSelector, state, subject, cancellationToken).ConfigureAwait(false);
                    relist = false;
                }
                await foreach (var (type, element) in this.WatchObjectsAsync(group, version, plural, @namespace, labelSelector, resourceVersion, timeoutSeconds, ex => watchError = ex, cancellationToken).ConfigureAwait(false))
                {
                    eventCount++;
                    var eventResourceVersion = element.TryGetProperty("metadata", out var metadata) && metadata.TryGetProperty("resourceVersion", out var value) ? value.GetString() : null;
                    if (!string.IsNullOrWhiteSpace(eventResourceVersion)) resourceVersion = eventResourceVersion;
                    var eventType = type switch
                    {
                        WatchEventType.Added => ResourceWatchEventType.Created,
                        WatchEventType.Modified => ResourceWatchEventType.Updated,
                        WatchEventType.Deleted => ResourceWatchEventType.Deleted,
                        _ => (ResourceWatchEventType?)null
                    };
                    if (eventType == null) continue;
                    var resource = this.JsonSerializer.Deserialize<Resource>(this.NormalizeObject(this.ToJsonObject(element), group, version, plural))!;
                    var key = this.BuildWatchStateKey(resource);
                    if (eventType == ResourceWatchEventType.Deleted) state.Remove(key);
                    else state[key] = this.BuildWatchStateEntry(resource);
                    subject.OnNext(new ResourceWatchEvent(eventType.Value, resource));
                    delay = this.Options.WatchMinReconnectDelay;
                }
                if (watchError is KubernetesException { Status.Code: (int)HttpStatusCode.Gone })
                {
                    this.Logger.LogDebug("The resource version '{resourceVersion}' of the watch of '{plural}.{group}/{version}' is too old: listing resources again", resourceVersion, plural, group, version);
                    relist = true;
                    continue;
                }
                if (watchError == null)
                {
                    this.Logger.LogTrace("The Kubernetes API server closed the watch of '{plural}.{group}/{version}': resuming from resource version '{resourceVersion}'", plural, group, version, resourceVersion);
                    if (eventCount > 0 || stopwatch.Elapsed >= this.Options.WatchMinReconnectDelay) continue;
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            catch (HttpOperationException ex) when (ex.Response?.StatusCode == HttpStatusCode.Gone)
            {
                relist = true;
                continue;
            }
            catch (JsonException) when (eventCount == 0 && stopwatch.Elapsed >= this.Options.WatchMinReconnectDelay)
            {
                //the API server closed the watch before sending any event, which the Kubernetes client fails to deserialize
                continue;
            }
            catch (Exception ex)
            {
                watchError = ex;
            }
            if (watchError != null) this.Logger.LogWarning("An error occurred while watching '{plural}.{group}/{version}', reconnecting in {delay}: {ex}", plural, group, version, delay, watchError);
            try
            {
                await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            if (watchError != null) delay = TimeSpan.FromTicks(Math.Min(delay.Ticks * 2, this.Options.WatchMaxReconnectDelay.Ticks));
        }
        subject.OnCompleted();
    }

    /// <summary>
    /// Determines whether the specified type of resource is the <see cref="Namespace"/> type, which the <see cref="KubernetesDatabase"/> maps to Kubernetes namespaces
    /// </summary>
    /// <param name="group">The API group of the type of resource to check</param>
    /// <param name="plural">The plural name of the type of resource to check</param>
    /// <returns>A boolean indicating whether or not the specified type of resource is the <see cref="Namespace"/> type</returns>
    /// <remarks>Kubernetes cannot serve a custom resource named 'namespaces' by name, since it parses '/apis/{group}/{version}/namespaces/{name}' as a namespaced request. Besides, namespaced resources can only be stored in existing Kubernetes namespaces</remarks>
    protected virtual bool IsNamespace(string group, string plural) => group == NamespaceDefinition.ResourceGroup && plural == NamespaceDefinition.ResourcePlural;

    /// <summary>
    /// Creates the specified object on the Kubernetes API
    /// </summary>
    /// <param name="body">The object to create</param>
    /// <param name="group">The API group of the object to create</param>
    /// <param name="version">The version of the type of the object to create</param>
    /// <param name="plural">The plural name of the type of the object to create</param>
    /// <param name="namespace">The namespace the object to create belongs to, if any</param>
    /// <param name="dryRun">A boolean indicating whether or not to persist the changes resulting from the operation</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The created object</returns>
    protected virtual async Task<JsonObject> CreateObjectAsync(JsonObject body, string group, string version, string plural, string? @namespace, bool dryRun, CancellationToken cancellationToken)
    {
        var dryRunValue = dryRun ? DryRunAll : null;
        if (this.IsNamespace(group, plural)) return this.FromKubernetesNamespace(await this.Kubernetes.CoreV1.CreateNamespaceAsync(this.ToKubernetesNamespace(body), dryRunValue, cancellationToken: cancellationToken).ConfigureAwait(false), group, version);
        return this.ToJsonObject(string.IsNullOrWhiteSpace(@namespace)
            ? await this.Kubernetes.CustomObjects.CreateClusterCustomObjectAsync(body, group, version, plural, dryRunValue, cancellationToken: cancellationToken).ConfigureAwait(false)
            : await this.Kubernetes.CustomObjects.CreateNamespacedCustomObjectAsync(body, group, version, @namespace, plural, dryRunValue, cancellationToken: cancellationToken).ConfigureAwait(false));
    }

    /// <summary>
    /// Gets the specified object from the Kubernetes API
    /// </summary>
    /// <param name="group">The API group of the object to get</param>
    /// <param name="version">The version of the type of the object to get</param>
    /// <param name="plural">The plural name of the type of the object to get</param>
    /// <param name="name">The name of the object to get</param>
    /// <param name="namespace">The namespace the object to get belongs to, if any</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The specified object</returns>
    protected virtual async Task<JsonObject> GetObjectAsync(string group, string version, string plural, string name, string? @namespace, CancellationToken cancellationToken)
    {
        if (this.IsNamespace(group, plural)) return this.FromKubernetesNamespace(await this.Kubernetes.CoreV1.ReadNamespaceAsync(name, cancellationToken: cancellationToken).ConfigureAwait(false), group, version);
        return this.ToJsonObject(string.IsNullOrWhiteSpace(@namespace)
            ? await this.Kubernetes.CustomObjects.GetClusterCustomObjectAsync(group, version, plural, name, cancellationToken: cancellationToken).ConfigureAwait(false)
            : await this.Kubernetes.CustomObjects.GetNamespacedCustomObjectAsync(group, version, @namespace, plural, name, cancellationToken: cancellationToken).ConfigureAwait(false));
    }

    /// <summary>
    /// Lists objects on the Kubernetes API
    /// </summary>
    /// <param name="group">The API group of the objects to list</param>
    /// <param name="version">The version of the type of the objects to list</param>
    /// <param name="plural">The plural name of the type of the objects to list</param>
    /// <param name="namespace">The namespace the objects to list belong to, if any</param>
    /// <param name="labelSelector">The Kubernetes label selector used to filter objects, if any</param>
    /// <param name="limit">The maximum amount of objects to list, if any</param>
    /// <param name="continuationToken">The token used to continue a paged list, if any</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The list of matching objects</returns>
    protected virtual async Task<JsonObject> ListObjectsAsync(string group, string version, string plural, string? @namespace, string? labelSelector, int? limit, string? continuationToken, CancellationToken cancellationToken)
    {
        if (this.IsNamespace(group, plural))
        {
            var namespaces = await this.Kubernetes.CoreV1.ListNamespaceAsync(continueParameter: continuationToken, labelSelector: labelSelector, limit: limit, cancellationToken: cancellationToken).ConfigureAwait(false);
            var list = this.ToJsonObject(KubernetesJson.Serialize(namespaces));
            if (list["items"] is JsonArray items) foreach (var item in items.OfType<JsonObject>()) this.NormalizeObject(item, group, version, plural);
            return list;
        }
        return this.ToJsonObject(string.IsNullOrWhiteSpace(@namespace)
            ? await this.Kubernetes.CustomObjects.ListClusterCustomObjectAsync(group, version, plural, continueParameter: continuationToken, labelSelector: labelSelector, limit: limit, cancellationToken: cancellationToken).ConfigureAwait(false)
            : await this.Kubernetes.CustomObjects.ListNamespacedCustomObjectAsync(group, version, @namespace, plural, continueParameter: continuationToken, labelSelector: labelSelector, limit: limit, cancellationToken: cancellationToken).ConfigureAwait(false));
    }

    /// <summary>
    /// Watches objects on the Kubernetes API, until the API server closes the watch
    /// </summary>
    /// <param name="group">The API group of the objects to watch</param>
    /// <param name="version">The version of the type of the objects to watch</param>
    /// <param name="plural">The plural name of the type of the objects to watch</param>
    /// <param name="namespace">The namespace the objects to watch belong to, if any</param>
    /// <param name="labelSelector">The Kubernetes label selector used to filter objects, if any</param>
    /// <param name="resourceVersion">The resource version from which to watch objects</param>
    /// <param name="timeoutSeconds">The duration, in seconds, after which the API server should close the watch, if any</param>
    /// <param name="onError">An <see cref="Action{T}"/> invoked when the API server reports an error, such as an expired resource version</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IAsyncEnumerable{T}"/> used to enumerate the watch events</returns>
    protected virtual IAsyncEnumerable<(WatchEventType Type, JsonElement Object)> WatchObjectsAsync(string group, string version, string plural, string? @namespace, string? labelSelector, string resourceVersion, int? timeoutSeconds, Action<Exception> onError, CancellationToken cancellationToken)
    {
        if (this.IsNamespace(group, plural)) return this.Kubernetes.CoreV1.ListNamespaceWithHttpMessagesAsync(allowWatchBookmarks: true, labelSelector: labelSelector, resourceVersion: resourceVersion, timeoutSeconds: timeoutSeconds, watch: true, cancellationToken: cancellationToken).WatchAsync<JsonElement, V1NamespaceList>(onError, cancellationToken);
        var responseTask = string.IsNullOrWhiteSpace(@namespace)
            ? this.Kubernetes.CustomObjects.ListClusterCustomObjectWithHttpMessagesAsync(group, version, plural, allowWatchBookmarks: true, labelSelector: labelSelector, resourceVersion: resourceVersion, timeoutSeconds: timeoutSeconds, watch: true, cancellationToken: cancellationToken)
            : this.Kubernetes.CustomObjects.ListNamespacedCustomObjectWithHttpMessagesAsync(group, version, @namespace, plural, allowWatchBookmarks: true, labelSelector: labelSelector, resourceVersion: resourceVersion, timeoutSeconds: timeoutSeconds, watch: true, cancellationToken: cancellationToken);
        return responseTask.WatchAsync<JsonElement, object>(onError, cancellationToken);
    }

    /// <summary>
    /// Patches the specified object, or its status, on the Kubernetes API
    /// </summary>
    /// <param name="patch">The patch to apply</param>
    /// <param name="group">The API group of the object to patch</param>
    /// <param name="version">The version of the type of the object to patch</param>
    /// <param name="plural">The plural name of the type of the object to patch</param>
    /// <param name="name">The name of the object to patch</param>
    /// <param name="subResource">The sub-resource to patch, if any</param>
    /// <param name="namespace">The namespace the object to patch belongs to, if any</param>
    /// <param name="dryRun">A boolean indicating whether or not to persist the changes resulting from the operation</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The patched object</returns>
    protected virtual async Task<JsonObject> PatchObjectAsync(V1Patch patch, string group, string version, string plural, string name, string? subResource, string? @namespace, bool dryRun, CancellationToken cancellationToken)
    {
        var dryRunValue = dryRun ? DryRunAll : null;
        if (this.IsNamespace(group, plural)) return this.FromKubernetesNamespace(subResource == null
            ? await this.Kubernetes.CoreV1.PatchNamespaceAsync(patch, name, dryRunValue, cancellationToken: cancellationToken).ConfigureAwait(false)
            : await this.Kubernetes.CoreV1.PatchNamespaceStatusAsync(patch, name, dryRunValue, cancellationToken: cancellationToken).ConfigureAwait(false), group, version);
        return this.ToJsonObject(subResource == null
            ? string.IsNullOrWhiteSpace(@namespace)
                ? await this.Kubernetes.CustomObjects.PatchClusterCustomObjectAsync(patch, group, version, plural, name, dryRunValue, cancellationToken: cancellationToken).ConfigureAwait(false)
                : await this.Kubernetes.CustomObjects.PatchNamespacedCustomObjectAsync(patch, group, version, @namespace, plural, name, dryRunValue, cancellationToken: cancellationToken).ConfigureAwait(false)
            : string.IsNullOrWhiteSpace(@namespace)
                ? await this.Kubernetes.CustomObjects.PatchClusterCustomObjectStatusAsync(patch, group, version, plural, name, dryRunValue, cancellationToken: cancellationToken).ConfigureAwait(false)
                : await this.Kubernetes.CustomObjects.PatchNamespacedCustomObjectStatusAsync(patch, group, version, @namespace, plural, name, dryRunValue, cancellationToken: cancellationToken).ConfigureAwait(false));
    }

    /// <summary>
    /// Replaces the specified object, or its status, on the Kubernetes API
    /// </summary>
    /// <param name="body">The state to replace the object with</param>
    /// <param name="group">The API group of the object to replace</param>
    /// <param name="version">The version of the type of the object to replace</param>
    /// <param name="plural">The plural name of the type of the object to replace</param>
    /// <param name="name">The name of the object to replace</param>
    /// <param name="subResource">The sub-resource to replace, if any</param>
    /// <param name="namespace">The namespace the object to replace belongs to, if any</param>
    /// <param name="dryRun">A boolean indicating whether or not to persist the changes resulting from the operation</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The replaced object</returns>
    protected virtual async Task<JsonObject> ReplaceObjectAsync(JsonObject body, string group, string version, string plural, string name, string? subResource, string? @namespace, bool dryRun, CancellationToken cancellationToken)
    {
        var dryRunValue = dryRun ? DryRunAll : null;
        if (this.IsNamespace(group, plural)) return this.FromKubernetesNamespace(subResource == null
            ? await this.Kubernetes.CoreV1.ReplaceNamespaceAsync(this.ToKubernetesNamespace(body), name, dryRunValue, cancellationToken: cancellationToken).ConfigureAwait(false)
            : await this.Kubernetes.CoreV1.ReplaceNamespaceStatusAsync(this.ToKubernetesNamespace(body), name, dryRunValue, cancellationToken: cancellationToken).ConfigureAwait(false), group, version);
        return this.ToJsonObject(subResource == null
            ? string.IsNullOrWhiteSpace(@namespace)
                ? await this.Kubernetes.CustomObjects.ReplaceClusterCustomObjectAsync(body, group, version, plural, name, dryRunValue, cancellationToken: cancellationToken).ConfigureAwait(false)
                : await this.Kubernetes.CustomObjects.ReplaceNamespacedCustomObjectAsync(body, group, version, @namespace, plural, name, dryRunValue, cancellationToken: cancellationToken).ConfigureAwait(false)
            : string.IsNullOrWhiteSpace(@namespace)
                ? await this.Kubernetes.CustomObjects.ReplaceClusterCustomObjectStatusAsync(body, group, version, plural, name, dryRunValue, cancellationToken: cancellationToken).ConfigureAwait(false)
                : await this.Kubernetes.CustomObjects.ReplaceNamespacedCustomObjectStatusAsync(body, group, version, @namespace, plural, name, dryRunValue, cancellationToken: cancellationToken).ConfigureAwait(false));
    }

    /// <summary>
    /// Deletes the specified object from the Kubernetes API
    /// </summary>
    /// <param name="group">The API group of the object to delete</param>
    /// <param name="version">The version of the type of the object to delete</param>
    /// <param name="plural">The plural name of the type of the object to delete</param>
    /// <param name="name">The name of the object to delete</param>
    /// <param name="namespace">The namespace the object to delete belongs to, if any</param>
    /// <param name="dryRun">A boolean indicating whether or not to persist the changes resulting from the operation</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The deleted object</returns>
    protected virtual async Task<JsonObject> DeleteObjectAsync(string group, string version, string plural, string name, string? @namespace, bool dryRun, CancellationToken cancellationToken)
    {
        var dryRunValue = dryRun ? DryRunAll : null;
        if (this.IsNamespace(group, plural))
        {
            var deleted = await this.GetObjectAsync(group, version, plural, name, @namespace, cancellationToken).ConfigureAwait(false);
            await this.Kubernetes.CoreV1.DeleteNamespaceAsync(name, dryRun: dryRunValue, cancellationToken: cancellationToken).ConfigureAwait(false);
            return deleted;
        }
        return this.ToJsonObject(string.IsNullOrWhiteSpace(@namespace)
            ? await this.Kubernetes.CustomObjects.DeleteClusterCustomObjectAsync(group, version, plural, name, dryRun: dryRunValue, cancellationToken: cancellationToken).ConfigureAwait(false)
            : await this.Kubernetes.CustomObjects.DeleteNamespacedCustomObjectAsync(group, version, @namespace, plural, name, dryRun: dryRunValue, cancellationToken: cancellationToken).ConfigureAwait(false));
    }

    /// <summary>
    /// Normalizes an object returned by the Kubernetes API, so that it describes a resource of the specified type
    /// </summary>
    /// <param name="node">The object to normalize</param>
    /// <param name="group">The API group of the object to normalize</param>
    /// <param name="version">The version of the type of the object to normalize</param>
    /// <param name="plural">The plural name of the type of the object to normalize</param>
    /// <returns>The normalized object</returns>
    protected virtual JsonObject NormalizeObject(JsonObject node, string group, string version, string plural)
    {
        if (!this.IsNamespace(group, plural)) return node;
        node["apiVersion"] = Resource.BuildApiVersion(group, version);
        node["kind"] = NamespaceDefinition.ResourceKind;
        return node;
    }

    /// <summary>
    /// Converts the specified <see cref="Namespace"/> into a new Kubernetes <see cref="V1Namespace"/>
    /// </summary>
    /// <param name="node">The <see cref="Namespace"/> to convert</param>
    /// <returns>A new <see cref="V1Namespace"/></returns>
    protected virtual V1Namespace ToKubernetesNamespace(JsonObject node)
    {
        var clone = node.DeepClone().AsObject();
        clone["apiVersion"] = V1Namespace.KubeApiVersion;
        clone["kind"] = V1Namespace.KubeKind;
        return KubernetesJson.Deserialize<V1Namespace>(clone.ToJsonString());
    }

    /// <summary>
    /// Converts the specified Kubernetes <see cref="V1Namespace"/> into a new <see cref="Namespace"/>
    /// </summary>
    /// <param name="value">The <see cref="V1Namespace"/> to convert</param>
    /// <param name="group">The API group of the <see cref="Namespace"/> type</param>
    /// <param name="version">The version of the <see cref="Namespace"/> type</param>
    /// <returns>A new <see cref="JsonObject"/> that describes the converted <see cref="Namespace"/></returns>
    protected virtual JsonObject FromKubernetesNamespace(V1Namespace value, string group, string version) => this.NormalizeObject(this.ToJsonObject(KubernetesJson.Serialize(value)), group, version, NamespaceDefinition.ResourcePlural);

    /// <summary>
    /// Creates a new <see cref="Exception"/> that describes the specified error returned by the Kubernetes API, using the same problem details as the ones produced by other <see cref="IDatabase"/> implementations
    /// </summary>
    /// <param name="ex">The <see cref="HttpOperationException"/> to translate</param>
    /// <param name="definition">A reference to the definition of the resource(s) the failed operation applied to</param>
    /// <param name="name">The name of the resource the failed operation applied to, if any</param>
    /// <param name="namespace">The namespace of the resource(s) the failed operation applied to, if any</param>
    /// <param name="subResource">The sub-resource the failed operation applied to, if any</param>
    /// <param name="targetResourceVersion">The resource version expected by the failed operation, if any</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="ProblemDetailsException"/></returns>
    protected virtual async Task<Exception> CreateExceptionAsync(HttpOperationException ex, ResourceDefinitionReference definition, string? name = null, string? @namespace = null, string? subResource = null, string? targetResourceVersion = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(ex);
        var status = this.ReadStatus(ex);
        var statusCode = (int?)ex.Response?.StatusCode ?? status?.Code ?? (int)HttpStatusCode.InternalServerError;
        var reference = string.IsNullOrWhiteSpace(name) ? null : this.BuildReference(definition, name, @namespace, subResource);
        var detail = status?.Message ?? ex.Message;
        ProblemDetails problem;
        switch (statusCode)
        {
            case (int)HttpStatusCode.NotFound:
                if (string.IsNullOrWhiteSpace(status?.Details?.Name)) problem = ResourceProblemDetails.ResourceDefinitionNotFound(definition);
                else if (reference != null && status.Details.Name == name) problem = ResourceProblemDetails.ResourceNotFound(reference);
                else problem = new(ProblemTypes.NotFound, ProblemTitles.NotFound, statusCode, detail);
                break;
            case (int)HttpStatusCode.Conflict:
                if (status?.Reason == "AlreadyExists" && reference != null) problem = ResourceProblemDetails.ResourceAlreadyExists(reference);
                else if (reference != null)
                {
                    var exists = true;
                    string? actualResourceVersion = null;
                    try
                    {
                        var actualResource = await this.GetResourceAsync(definition.Group, definition.Version, definition.Plural, name!, @namespace, cancellationToken).ConfigureAwait(false);
                        exists = actualResource != null;
                        actualResourceVersion = actualResource?.Metadata.ResourceVersion;
                    }
                    catch (Exception getException)
                    {
                        this.Logger.LogDebug("Failed to get the current version of resource '{resource}': {ex}", reference, getException);
                    }
                    //the API server reports a conflict rather than a 404 when replacing a missing resource with a resource version
                    problem = exists
                        ? ResourceProblemDetails.ResourceOptimisticConcurrencyCheckFailed(reference, targetResourceVersion ?? string.Empty, actualResourceVersion ?? string.Empty)
                        : ResourceProblemDetails.ResourceNotFound(reference);
                }
                else problem = new(ProblemTypes.Conflict, ProblemTitles.Conflict, statusCode, detail);
                break;
            case (int)HttpStatusCode.UnprocessableEntity:
            case (int)HttpStatusCode.BadRequest:
                var errors = status?.Details?.Causes?
                    .GroupBy(c => string.IsNullOrWhiteSpace(c.Field) ? "resource" : c.Field)
                    .ToDictionary(g => g.Key, g => g.Select(c => c.Message).ToArray());
                problem = new(ProblemTypes.SchemaValidationFailed, ProblemTitles.ValidationFailed, (int)HttpStatusCode.BadRequest, detail, null, errors?.Count > 0 ? errors : null);
                break;
            default:
                problem = new(KubernetesApiErrorProblemType, status?.Reason ?? ex.Response?.ReasonPhrase ?? "Kubernetes API Error", statusCode, detail);
                break;
        }
        this.Logger.LogDebug("The Kubernetes API returned an error: {ex}", ex);
        return new ProblemDetailsException(problem);
    }

    /// <summary>
    /// Reads the <see cref="V1Status"/> returned by the Kubernetes API in the response described by the specified <see cref="HttpOperationException"/>, if any
    /// </summary>
    /// <param name="ex">The <see cref="HttpOperationException"/> to read the <see cref="V1Status"/> of</param>
    /// <returns>The <see cref="V1Status"/> returned by the Kubernetes API, if any</returns>
    protected virtual V1Status? ReadStatus(HttpOperationException ex)
    {
        var content = ex.Response?.Content;
        if (string.IsNullOrWhiteSpace(content) || !content.TrimStart().StartsWith('{')) return null;
        try
        {
            return KubernetesJson.Deserialize<V1Status>(content);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    /// <summary>
    /// Serializes the specified <see cref="IResource"/> into a new <see cref="JsonObject"/>
    /// </summary>
    /// <param name="resource">The <see cref="IResource"/> to serialize</param>
    /// <returns>A new <see cref="JsonObject"/></returns>
    protected virtual JsonObject SerializeResource(IResource resource) => this.JsonSerializer.SerializeToNode<object>(resource)?.AsObject() ?? throw new ArgumentException("The specified resource could not be serialized to a JSON object", nameof(resource));

    /// <summary>
    /// Converts an object returned by the Kubernetes client into a new <see cref="JsonObject"/>
    /// </summary>
    /// <param name="value">The object to convert</param>
    /// <returns>A new <see cref="JsonObject"/></returns>
    protected virtual JsonObject ToJsonObject(object value) => this.ToJsonNode(value)?.AsObject() ?? throw new InvalidOperationException("The Kubernetes API returned an unexpected response");

    /// <summary>
    /// Converts the specified value into a new <see cref="JsonNode"/>
    /// </summary>
    /// <param name="value">The value to convert</param>
    /// <returns>A new <see cref="JsonNode"/></returns>
    protected virtual JsonNode? ToJsonNode(object? value) => value switch
    {
        null => null,
        JsonNode node => node.DeepClone(),
        JsonElement element => JsonNode.Parse(element.GetRawText()),
        string json => JsonNode.Parse(json),
        _ => this.JsonSerializer.SerializeToNode(value)
    };

    /// <summary>
    /// Builds a Kubernetes label selector based on the specified <see cref="LabelSelector"/>s
    /// </summary>
    /// <param name="labelSelectors">The <see cref="LabelSelector"/>s to build a Kubernetes label selector for</param>
    /// <returns>A new Kubernetes label selector, if any</returns>
    protected virtual string? BuildLabelSelector(IEnumerable<LabelSelector>? labelSelectors) => labelSelectors?.Any() == true ? string.Join(',', labelSelectors.Select(s => s.ToString())) : null;

    /// <summary>
    /// Builds a new <see cref="IResourceReference"/> to the specified resource or sub-resource
    /// </summary>
    /// <param name="definition">A reference to the resource's definition</param>
    /// <param name="name">The resource's name</param>
    /// <param name="namespace">The resource's namespace, if any</param>
    /// <param name="subResource">The sub-resource, if any</param>
    /// <returns>A new <see cref="IResourceReference"/></returns>
    protected virtual IResourceReference BuildReference(ResourceDefinitionReference definition, string name, string? @namespace, string? subResource) => subResource == null
        ? new ResourceReference(definition, name, @namespace)
        : new SubResourceReference(definition, name, subResource, @namespace);

    /// <summary>
    /// Ensures that the specified sub-resource is supported
    /// </summary>
    /// <param name="group">The API group the resource belongs to</param>
    /// <param name="version">The version of the type of resource</param>
    /// <param name="plural">The plural name of the type of resource</param>
    /// <param name="name">The name of the resource</param>
    /// <param name="subResource">The sub-resource to check</param>
    /// <param name="namespace">The namespace the resource belongs to, if any</param>
    protected virtual void EnsureSubResourceIsSupported(string group, string version, string plural, string name, string subResource, string? @namespace)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(subResource);
        if (!subResource.Equals(StatusSubResource, StringComparison.OrdinalIgnoreCase)) throw new ProblemDetailsException(ResourceProblemDetails.UnsupportedSubResource(new SubResourceReference(new(group, version, plural), name, subResource, @namespace)));
    }

    int GetPageSize() => this.Options.PageSize > 0 ? this.Options.PageSize : KubernetesDatabaseOptions.DefaultPageSize;

    JsonObject BuildResourceVersionOperation(string resourceVersion) => new()
    {
        ["op"] = "replace",
        ["path"] = ResourceVersionPath,
        ["value"] = resourceVersion
    };

    string BuildWatchStateKey(IResource resource) => string.IsNullOrWhiteSpace(resource.Metadata.Namespace) ? resource.Metadata.Name! : $"{resource.Metadata.Namespace}/{resource.Metadata.Name}";

    Resource BuildWatchStateEntry(Resource resource) => new()
    {
        ApiVersion = resource.ApiVersion,
        Kind = resource.Kind,
        Metadata = new()
        {
            Name = resource.Metadata.Name,
            Namespace = resource.Metadata.Namespace,
            Labels = resource.Metadata.Labels,
            ResourceVersion = resource.Metadata.ResourceVersion
        }
    };

    async Task<bool> TryCreateAsync<TResource>(TResource resource, CancellationToken cancellationToken)
        where TResource : class, IResource, new()
    {
        try
        {
            await this.CreateResourceAsync(resource, false, cancellationToken).ConfigureAwait(false);
            return true;
        }
        catch (ProblemDetailsException ex) when (ex.Problem.Type == ProblemTypes.Conflict)
        {
            return false;
        }
    }

    /// <summary>
    /// Disposes of the <see cref="KubernetesDatabase"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not to dispose of the <see cref="KubernetesDatabase"/></param>
    /// <returns>A new awaitable <see cref="ValueTask"/></returns>
    protected virtual ValueTask DisposeAsync(bool disposing)
    {
        this.Dispose(disposing);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        await this.DisposeAsync(true).ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes of the <see cref="KubernetesDatabase"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="KubernetesDatabase"/> is being disposed of</param>
    protected virtual void Dispose(bool disposing)
    {
        if (this._disposed) return;
        if (disposing)
        {
            this.CancellationTokenSource.Cancel();
            this.CancellationTokenSource.Dispose();
        }
        this._disposed = true;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Represents the observable that streams the events of a Kubernetes watch, which it stops when disposed of
    /// </summary>
    /// <param name="subject">The <see cref="Subject{T}"/> used to stream watch events</param>
    /// <param name="cancellationTokenSource">The <see cref="System.Threading.CancellationTokenSource"/> used to stop the watch</param>
    /// <param name="task">The <see cref="Task"/> that runs the watch</param>
    sealed class KubernetesResourceWatchObservable(Subject<IResourceWatchEvent> subject, CancellationTokenSource cancellationTokenSource, Task task)
        : IObservable<IResourceWatchEvent>, IDisposable, IAsyncDisposable
    {

        int _disposed;

        public IDisposable Subscribe(IObserver<IResourceWatchEvent> observer) => subject.Subscribe(observer);

        public async ValueTask DisposeAsync()
        {
            if (Interlocked.Exchange(ref this._disposed, 1) == 1) return;
            cancellationTokenSource.Cancel();
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (Exception) { }
            cancellationTokenSource.Dispose();
            subject.Dispose();
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref this._disposed, 1) == 1) return;
            cancellationTokenSource.Cancel();
            task.ContinueWith(_ =>
            {
                cancellationTokenSource.Dispose();
                subject.Dispose();
            }, TaskScheduler.Default);
        }

    }

}
