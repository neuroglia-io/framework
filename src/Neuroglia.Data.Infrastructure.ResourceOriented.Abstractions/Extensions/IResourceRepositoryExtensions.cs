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

using Neuroglia.Data.Infrastructure.ResourceOriented.Services;

namespace Neuroglia.Data.Infrastructure.ResourceOriented;

/// <summary>
/// Defines extensions for <see cref="IResourceRepository"/> implementations
/// </summary>
public static class IResourceRepositoryExtensions
{

    /// <summary>
    /// Gets the resource definition with the specified name
    /// </summary>
    /// <param name="repository">The extended <see cref="IResourceRepository"/></param>
    /// <param name="group">The API group the resource definition to get belongs to</param>
    /// <param name="plural">The plural name of the resource definition to get</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The resource definition with the specified name, if any</returns>
    public static async Task<IResourceDefinition?> GetDefinitionAsync(this IResourceRepository repository, string group, string plural, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        var resource = await repository.GetAsync<ResourceDefinition>(string.IsNullOrWhiteSpace(group) ? plural : $"{plural}.{group}", cancellationToken: cancellationToken).ConfigureAwait(false);
        if (resource == null) return null;
        return resource.ConvertTo<ResourceDefinition>();
    }

    /// <summary>
    /// Gets the definition of the specified resource type
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="IResource"/> to get the definition of</typeparam>
    /// <param name="repository">The extended <see cref="IResourceRepository"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The resource definition with the specified name, if any</returns>
    public static Task<IResourceDefinition?> GetDefinitionAsync<TResource>(this IResourceRepository repository, CancellationToken cancellationToken = default)
        where TResource : class, IResource, new()
    {
        var resource = new TResource();
        return repository.GetDefinitionAsync(resource.GetGroup(), resource.Definition.Plural, cancellationToken);
    }

    /// <summary>
    /// Lists <see cref="IResourceDefinition"/>s
    /// </summary>
    /// <param name="repository">The extended <see cref="IResourceRepository"/></param>
    /// <param name="labelSelectors">A collection of objects used to configure the labels to filter the <see cref="IResourceDefinition"/>s to list by</param>
    /// <param name="maxResults">The maximum amount of results that should be returned</param>
    /// <param name="continuationToken">A value used to continue paging resource definitions, in the context of a paging request</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="ICollection"/> that contains all matching <see cref="IResourceDefinition"/>s</returns>
    public static async Task<ICollection<ResourceDefinition>> ListDefinitionsAsync(this IResourceRepository repository, IEnumerable<LabelSelector>? labelSelectors = null, ulong? maxResults = null, string? continuationToken = null, CancellationToken cancellationToken = default)
    {
        return await repository.ListAsync<ResourceDefinition>(null, labelSelectors, maxResults, continuationToken, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Adds the specified <see cref="IResource"/>
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="IResource"/> to add</typeparam>
    /// <param name="repository">The extended <see cref="IResourceRepository"/></param>
    /// <param name="resource">The <see cref="IResource"/> to add</param>
    /// <param name="dryRun">A boolean indicating whether or not to persist the changes induced by the operation</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The added <see cref="IResource"/></returns>
    public static async Task<TResource> AddAsync<TResource>(this IResourceRepository repository, TResource resource, bool dryRun = false, CancellationToken cancellationToken = default)
        where TResource : class, IResource, new()
    {
        ArgumentNullException.ThrowIfNull(resource);
        var result = await repository.AddAsync(resource, resource.Definition.Group, resource.Definition.Version, resource.Definition.Plural, dryRun, cancellationToken).ConfigureAwait(false);
        return result.ConvertTo<TResource>()!;
    }

    /// <summary>
    /// Adds a new <see cref="Namespace"/>
    /// </summary>
    /// <param name="repository">The extended <see cref="IResourceRepository"/></param>
    /// <param name="name">The name of the <see cref="Namespace"/> to add</param>
    /// <param name="dryRun">A boolean indicating whether or not to persist the changes induced by the operation</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The added <see cref="Namespace"/></returns>
    public static async Task<Namespace> AddNamespaceAsync(this IResourceRepository repository, string name, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        return await repository.AddAsync<Namespace>(new(name), dryRun, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the <see cref="IResource"/> with the specified name, if any
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="IResource"/> to get</typeparam>
    /// <param name="repository">The extended <see cref="IResourceRepository"/></param>
    /// <param name="name">The name of the <see cref="IResource"/> to get</param>
    /// <param name="namespace">The namespace the <see cref="IResource"/> to get belongs to, if any</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The <see cref="IResource"/> with the specified name, if any</returns>
    public static async Task<TResource?> GetAsync<TResource>(this IResourceRepository repository, string name, string? @namespace = null, CancellationToken cancellationToken = default)
        where TResource : class, IResource, new()
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        var resource = new TResource();
        var result = await repository.GetAsync(resource.Definition.Group, resource.Definition.Version, resource.Definition.Plural, name, @namespace, cancellationToken).ConfigureAwait(false);
        return result.ConvertTo<TResource>()!;
    }

    /// <summary>
    /// Lists <see cref="IResource"/>s of the specified type
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="IResource"/>s to list</typeparam>
    /// <param name="repository">The extended <see cref="IResourceRepository"/></param>
    /// <param name="namespace">The namespace the <see cref="IResource"/>s to list belongs to, if any. If not set, lists resources across all namespaces</param>
    /// <param name="labelSelectors">A collection of objects used to configure the labels to filter the <see cref="IResource"/>s to list by</param>
    /// <param name="maxResults">The maximum amount of results that should be returned</param>
    /// <param name="continuationToken">A value used to continue paging resources, in the context of a paging request</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="ICollection"/> that contains all matching <see cref="IResource"/>s of type specified type</returns>
    public static async Task<ICollection<TResource>> ListAsync<TResource>(this IResourceRepository repository, string? @namespace = null, IEnumerable<LabelSelector>? labelSelectors = null, ulong? maxResults = null, string? continuationToken = null, CancellationToken cancellationToken = default)
        where TResource : class, IResource, new()
    {
        var resource = new TResource();
        var collection = await repository.ListAsync(resource.Definition.Group, resource.Definition.Version, resource.Definition.Plural, @namespace, labelSelectors, maxResults, continuationToken, cancellationToken).ConfigureAwait(false);
        return collection.OfType<TResource>();
    }

    /// <summary>
    /// Streams <see cref="IResource"/>s of the specified type
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="IResource"/>s to stream</typeparam>
    /// <param name="repository">The extended <see cref="IResourceRepository"/></param>
    /// <param name="namespace">The namespace the <see cref="IResource"/>s to stream belongs to, if any. If not set, streams resources across all namespaces</param>
    /// <param name="labelSelectors">A collection of objects used to configure the labels to filter the <see cref="IResource"/>s to stream by</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IAsyncEnumerable{T}"/> used to stream all matching <see cref="IResource"/>s of type specified type</returns>
    public static IAsyncEnumerable<TResource> GetAllAsync<TResource>(this IResourceRepository repository, string? @namespace = null, IEnumerable<LabelSelector>? labelSelectors = null, CancellationToken cancellationToken = default)
        where TResource : class, IResource, new()
    {
        var resource = new TResource();
        var stream = repository.GetAllAsync(resource.Definition.Group, resource.Definition.Version, resource.Definition.Plural, @namespace, labelSelectors, cancellationToken);
        return stream.Select(r => r.ConvertTo<TResource>()!);
    }

    /// <summary>
    /// Observes events produced by <see cref="IResource"/>s of the specified type
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="IResource"/>s to observe</typeparam>
    /// <param name="repository">The extended <see cref="IResourceRepository"/></param>
    /// <param name="namespace">The namespace the <see cref="IResource"/>s to stream belongs to, if any. If not set, observes resources across all namespaces</param>
    /// <param name="labelSelectors">A collection of objects used to configure the labels to filter the <see cref="IResource"/>s to observe by</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A <see cref="IResourceWatch{TResource}"/> used to observe events produced by <see cref="IResource"/>s of the specified type</returns>
    public static async Task<IResourceWatch<TResource>> WatchAsync<TResource>(this IResourceRepository repository, string? @namespace = null, IEnumerable<LabelSelector>? labelSelectors = null, CancellationToken cancellationToken = default)
        where TResource : class, IResource, new()
    {
        var resource = new TResource();
        var watch = await repository.WatchAsync(resource.Definition.Group, resource.Definition.Version, resource.Definition.Plural, @namespace, labelSelectors, cancellationToken).ConfigureAwait(false);
        return watch.OfType<TResource>();
    }

    /// <summary>
    /// Monitors changes to the specified <see cref="IResource"/>
    /// </summary>
    /// <param name="repository">The extended <see cref="IResourceRepository"/></param>
    /// <param name="group">The API group the <see cref="IResource"/> to monitor belongs to</param>
    /// <param name="version">The version of the <see cref="IResource"/> to monitor</param>
    /// <param name="plural">The plural form of the type of the <see cref="IResource"/> to monitor</param>
    /// <param name="name">The name of the <see cref="IResource"/> to monitor</param>
    /// <param name="namespace">The namespace the <see cref="IResource"/> to monitor belongs to, if any</param>
    /// <param name="leaveOpen">A boolean indicating whether or not to leave the <see cref="IResourceWatch"/> open when the <see cref="ResourceMonitor"/> is being disposed of</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IResourceMonitor"/></returns>
    public static async Task<IResourceMonitor> MonitorAsync(this IResourceRepository repository, string group, string version, string plural, string name, string? @namespace = null, bool leaveOpen = false, CancellationToken cancellationToken = default)
    {
        var resource = await repository.GetAsync(group, version, plural, name, @namespace, cancellationToken).ConfigureAwait(false) ?? throw new ProblemDetailsException(ResourceProblemDetails.ResourceNotFound(new ResourceReference(new(group, version, plural), name, @namespace)));
        var watch = await repository.WatchAsync(group, version, plural, @namespace, cancellationToken: cancellationToken).ConfigureAwait(false);
        return new ResourceMonitor(watch, resource, leaveOpen);
    }

    /// <summary>
    /// Monitors changes to the specified <see cref="IResource"/>
    /// </summary>
    /// <typeparam name="TResource">The type of the <see cref="IResource"/> to monitor</typeparam>
    /// <param name="repository">The extended <see cref="IResourceRepository"/></param>
    /// <param name="name">The name of the <see cref="IResource"/> to monitor</param>
    /// <param name="namespace">The namespace the <see cref="IResource"/> to monitor belongs to, if any</param>
    /// <param name="leaveOpen">A boolean indicating whether or not to leave the <see cref="IResourceWatch"/> open when the <see cref="ResourceMonitor"/> is being disposed of</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IResourceMonitor"/></returns>
    public static async Task<IResourceMonitor<TResource>> MonitorAsync<TResource>(this IResourceRepository repository, string name, string? @namespace = null, bool leaveOpen = false, CancellationToken cancellationToken = default)
        where TResource : class, IResource, new()
    {
        var resourceReference = new ResourceReference<TResource>(name, @namespace);
        var resource = await repository.GetAsync<TResource>(name, @namespace, cancellationToken).ConfigureAwait(false) ?? throw new ProblemDetailsException(ResourceProblemDetails.ResourceNotFound(resourceReference));
        var watch = await repository.WatchAsync<TResource>(@namespace, cancellationToken: cancellationToken).ConfigureAwait(false);
        return new ResourceMonitor<TResource>(watch, resource, leaveOpen);
    }

    /// <summary>
    /// Patches the specified <see cref="IResource"/>
    /// </summary>
    /// <typeparam name="TResource">The type of the <see cref="IResource"/> to patch</typeparam>
    /// <param name="repository">The extended <see cref="IResourceRepository"/></param>
    /// <param name="patch">The patch to apply</param>
    /// <param name="name">The name of the <see cref="IResource"/> to patch</param>
    /// <param name="namespace">The namespace the <see cref="IResource"/> to patch belongs to, if any</param>
    /// <param name="resourceVersion">The expected resource version, if any, used for optimistic concurrency</param>
    /// <param name="dryRun">A boolean indicating whether or not to persist the changes induced by the operation</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The replaced <see cref="IResource"/></returns>
    public static async Task<TResource> PatchAsync<TResource>(this IResourceRepository repository, Patch patch, string name, string? @namespace = null, string? resourceVersion = null, bool dryRun = false, CancellationToken cancellationToken = default)
        where TResource : class, IResource, new()
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        ArgumentNullException.ThrowIfNull(patch);
        var resource = new TResource();
        var result = await repository.PatchAsync(patch, resource.Definition.Group, resource.Definition.Version, resource.Definition.Plural, name, @namespace, resourceVersion, dryRun, cancellationToken).ConfigureAwait(false);
        return result.ConvertTo<TResource>()!;
    }

    /// <summary>
    /// Replaces the specified <see cref="IResource"/>
    /// </summary>
    /// <typeparam name="TResource">The type of the <see cref="IResource"/> to replace</typeparam>
    /// <param name="repository">The extended <see cref="IResourceRepository"/></param>
    /// <param name="resource">The state to replace the <see cref="IResource"/> with</param>
    /// <param name="dryRun">A boolean indicating whether or not to persist the changes induced by the operation</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The replaced <see cref="IResource"/></returns>
    public static async Task<TResource> ReplaceAsync<TResource>(this IResourceRepository repository, TResource resource, bool dryRun = false, CancellationToken cancellationToken = default)
        where TResource : class, IResource, new()
    {
        ArgumentNullException.ThrowIfNull(resource);
        var result = await repository.ReplaceAsync(resource, resource.Definition.Group, resource.Definition.Version, resource.Definition.Plural, dryRun, cancellationToken).ConfigureAwait(false);
        return result.ConvertTo<TResource>()!;
    }

    /// <summary>
    /// Patches the specified <see cref="IResource"/>'s status
    /// </summary>
    /// <typeparam name="TResource">The type of the <see cref="IResource"/> to patch the status of</typeparam>
    /// <param name="repository">The extended <see cref="IResourceRepository"/></param>
    /// <param name="patch">The patch to apply</param>
    /// <param name="name">The name of the <see cref="IResource"/> to patch the status of</param>
    /// <param name="namespace">The namespace the <see cref="IResource"/> to patch the status of belongs to, if any</param>
    /// <param name="resourceVersion">The expected resource version, if any, used for optimistic concurrency</param>
    /// <param name="dryRun">A boolean indicating whether or not to persist the changes induced by the operation</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The replaced <see cref="IResource"/></returns>
    public static async Task<TResource> PatchStatusAsync<TResource>(this IResourceRepository repository, Patch patch, string name, string? @namespace = null, string? resourceVersion = null, bool dryRun = false, CancellationToken cancellationToken = default)
        where TResource : class, IResource, new()
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        ArgumentNullException.ThrowIfNull(patch);
        var resource = new TResource();
        var result = await repository.PatchSubResourceAsync(patch, resource.Definition.Group, resource.Definition.Version, resource.Definition.Plural, name, "status", @namespace, resourceVersion, dryRun, cancellationToken).ConfigureAwait(false);
        return result.ConvertTo<TResource>()!;
    }

    /// <summary>
    /// Replaces the specified <see cref="IResource"/>'s status
    /// </summary>
    /// <typeparam name="TResource">The type of the <see cref="IResource"/> to replace the status with</typeparam>
    /// <param name="repository">The extended <see cref="IResourceRepository"/></param>
    /// <param name="resource">The state to replace the <see cref="IResource"/> the status with</param>
    /// <param name="dryRun">A boolean indicating whether or not to persist the changes induced by the operation</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The replaced <see cref="IResource"/></returns>
    public static async Task<TResource> ReplaceStatusAsync<TResource>(this IResourceRepository repository, TResource resource, bool dryRun = false, CancellationToken cancellationToken = default)
        where TResource : class, IResource, new()
    {
        ArgumentNullException.ThrowIfNull(resource);
        var result = await repository.ReplaceSubResourceAsync(resource, resource.Definition.Group, resource.Definition.Version, resource.Definition.Plural, "status", dryRun, cancellationToken).ConfigureAwait(false);
        return result.ConvertTo<TResource>()!;
    }

    /// <summary>
    /// Removes the specified <see cref="IResource"/>
    /// </summary>
    /// <typeparam name="TResource">The type of the <see cref="IResource"/> to remove</typeparam>
    /// <param name="repository">The extended <see cref="IResourceRepository"/></param>
    /// <param name="name">The name of the <see cref="IResource"/> to remove</param>
    /// <param name="namespace">The namespace the <see cref="IResource"/> to remove belongs to, if any</param>
    /// <param name="dryRun">A boolean indicating whether or not to persist the changes induced by the operation</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The removed <see cref="IResource"/></returns>
    public static async Task<TResource> RemoveAsync<TResource>(this IResourceRepository repository, string name, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default)
        where TResource : class, IResource, new()
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        var resource = new TResource();
        var result = await repository.RemoveAsync(resource.Definition.Group, resource.Definition.Version, resource.Definition.Plural, name, @namespace, dryRun, cancellationToken).ConfigureAwait(false);
        return result.ConvertTo<TResource>()!;
    }

    /// <summary>
    /// Gets all <see cref="MutatingWebhook"/>s that apply to the specified resource and operation
    /// </summary>
    /// <param name="resources">The <see cref="IResourceRepository"/> to query</param>
    /// <param name="operation">The operation for which to retrieve matching <see cref="MutatingWebhook"/>s</param>
    /// <param name="resource">An object used to reference the resource to get <see cref="MutatingWebhook"/>s for</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IAsyncEnumerable{T}"/> containing all matching <see cref="MutatingWebhook"/>s</returns>
    public static IAsyncEnumerable<MutatingWebhook> GetMutatingWebhooksFor(this IResourceRepository resources, Operation operation, IResourceReference resource, CancellationToken cancellationToken = default)
    {
        return resources
            .GetAllAsync<MutatingWebhook>(cancellationToken: cancellationToken)
            .Where(wh => wh.Spec.Resources == null || wh.Spec.Resources.Any(r => r.Matches(operation, resource.Definition.Group, resource.Definition.Version, resource.Definition.Plural, resource.Namespace)));
    }

    /// <summary>
    /// Gets all <see cref="ValidatingWebhook"/>s that apply to the specified resource and operation
    /// </summary>
    /// <param name="resources">The <see cref="IResourceRepository"/> to query</param>
    /// <param name="operation">The operation for which to retrieve matching <see cref="ValidatingWebhook"/>s</param>
    /// <param name="resource">An object used to reference the resource to get <see cref="ValidatingWebhook"/>s for</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IAsyncEnumerable{T}"/> containing all matching <see cref="ValidatingWebhook"/>s</returns>
    public static IAsyncEnumerable<ValidatingWebhook> GetValidatingWebhooksFor(this IResourceRepository resources, Operation operation, IResourceReference resource, CancellationToken cancellationToken = default)
    {
        return resources
            .GetAllAsync<ValidatingWebhook>(cancellationToken: cancellationToken)
            .Where(wh => wh.Spec.Resources == null || wh.Spec.Resources.Any(r => r.Matches(operation, resource.Definition.Group, resource.Definition.Version, resource.Definition.Plural, resource.Namespace)));
    }

}