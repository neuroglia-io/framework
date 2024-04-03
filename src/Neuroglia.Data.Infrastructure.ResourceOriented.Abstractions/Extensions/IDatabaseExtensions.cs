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
/// Defines extensions for <see cref="IDatabase"/> implementations
/// </summary>
public static class IDatabaseExtensions
{

    /// <summary>
    /// Creates and persists the specified <see cref="IResource"/>
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="IResource"/> to create</typeparam>
    /// <param name="database">The extended <see cref="IDatabase"/></param>
    /// <param name="resource">The <see cref="IResource"/> to create</param>
    /// <param name="dryRun">A boolean indicating whether or not to persist the changes subsequent to the operation</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The newly created <see cref="IResource"/></returns>
    public static async Task<TResource> CreateResourceAsync<TResource>(this IDatabase database, TResource resource, bool dryRun = false, CancellationToken cancellationToken = default)
        where TResource : class, IResource, new()
    {
        ArgumentNullException.ThrowIfNull(resource);
        var result = await database.CreateResourceAsync(resource, resource.Definition.Group, resource.Definition.Version, resource.Definition.Plural, resource.GetNamespace(), dryRun, cancellationToken).ConfigureAwait(false);
        return result.ConvertTo<TResource>()!;
    }

    /// <summary>
    /// Creates a new <see cref="Namespace"/>
    /// </summary>
    /// <param name="database">The extended <see cref="IDatabase"/></param>
    /// <param name="name">The name of the <see cref="Namespace"/> to create</param>
    /// <param name="dryRun">A boolean indicating whether or not to persist the changes subsequent to the operation</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The newly created <see cref="Namespace"/></returns>
    public static async Task<Namespace> CreateNamespaceAsync(this IDatabase database, string name, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        return await database.CreateResourceAsync<Namespace>(new(name), dryRun, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the resource definition with the specified name
    /// </summary>
    /// <param name="database">The extended <see cref="IDatabase"/></param>
    /// <param name="group">The API group the resource definition to get belongs to</param>
    /// <param name="plural">The plural name of the resource definition to get</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The resource definition with the specified name, if any</returns>
    public static async Task<IResourceDefinition?> GetDefinitionAsync(this IDatabase database, string group, string plural, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        var resource = await database.GetResourceAsync<ResourceDefinition>(string.IsNullOrWhiteSpace(group) ? plural : $"{plural}.{group}", cancellationToken: cancellationToken).ConfigureAwait(false);
        if (resource == null) return null;
        return resource.ConvertTo<ResourceDefinition>();
    }

    /// <summary>
    /// Gets the definition of the specified resource type
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="IResource"/> to get the definition of</typeparam>
    /// <param name="database">The extended <see cref="IRepository"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The resource definition with the specified name, if any</returns>
    public static Task<IResourceDefinition?> GetDefinitionAsync<TResource>(this IDatabase database, CancellationToken cancellationToken = default)
        where TResource : class, IResource, new()
    {
        var resource = new TResource();
        return database.GetDefinitionAsync(resource.GetGroup(), resource.Definition.Plural, cancellationToken);
    }

    /// <summary>
    /// Gets all <see cref="IResourceDefinition"/>s
    /// </summary>
    /// <param name="database">The extended <see cref="IRepository"/></param>
    /// <param name="labelSelectors">A collection of objects used to configure the labels to filter the <see cref="IResourceDefinition"/>s to list by</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IAsyncEnumerable{T}"/> used to asynchronously enumerate <see cref="IResourceDefinition"/>s</returns>
    public static IAsyncEnumerable<IResourceDefinition> GetDefinitionsAsync(this IDatabase database, IEnumerable<LabelSelector>? labelSelectors = null, CancellationToken cancellationToken = default)
    {
        return database.GetResourcesAsync<ResourceDefinition>(null, labelSelectors, cancellationToken).OfType<IResourceDefinition>();
    }

    /// <summary>
    /// Lists <see cref="IResourceDefinition"/>s
    /// </summary>
    /// <param name="database">The extended <see cref="IRepository"/></param>
    /// <param name="labelSelectors">A collection of objects used to configure the labels to filter the <see cref="IResourceDefinition"/>s to list by</param>
    /// <param name="maxResults">The maximum amount of results that should be returned</param>
    /// <param name="continuationToken">A value used to continue paging resource definitions, in the context of a paging request</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="ICollection"/> that contains all matching <see cref="IResourceDefinition"/>s</returns>
    public static async Task<ICollection<ResourceDefinition>> ListDefinitionsAsync(this IDatabase database, IEnumerable<LabelSelector>? labelSelectors = null, ulong? maxResults = null, string? continuationToken = null, CancellationToken cancellationToken = default)
    {
        return await database.ListResourcesAsync<ResourceDefinition>(null, labelSelectors, maxResults, continuationToken, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the <see cref="IResource"/> with the specified name, if any
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="IResource"/> to get</typeparam>
    /// <param name="database">The extended <see cref="IRepository"/></param>
    /// <param name="name">The name of the <see cref="IResource"/> to get</param>
    /// <param name="namespace">The namespace the <see cref="IResource"/> to get belongs to, if any</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The <see cref="IResource"/> with the specified name, if any</returns>
    public static async Task<TResource?> GetResourceAsync<TResource>(this IDatabase database, string name, string? @namespace = null, CancellationToken cancellationToken = default)
        where TResource : class, IResource, new()
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        var resource = new TResource();
        var result = await database.GetResourceAsync(resource.Definition.Group, resource.Definition.Version, resource.Definition.Plural, name, @namespace, cancellationToken).ConfigureAwait(false);
        return result.ConvertTo<TResource>()!;
    }

    /// <summary>
    /// Lists <see cref="IResource"/>s of the specified type
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="IResource"/>s to list</typeparam>
    /// <param name="database">The extended <see cref="IRepository"/></param>
    /// <param name="namespace">The namespace the <see cref="IResource"/>s to list belongs to, if any. If not set, lists resources across all namespaces</param>
    /// <param name="labelSelectors">A collection of objects used to configure the labels to filter the <see cref="IResource"/>s to list by</param>
    /// <param name="maxResults">The maximum amount of results that should be returned</param>
    /// <param name="continuationToken">A value used to continue paging resources, in the context of a paging request</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="ICollection"/> that contains all matching <see cref="IResource"/>s of type specified type</returns>
    public static async Task<ICollection<TResource>> ListResourcesAsync<TResource>(this IDatabase database, string? @namespace = null, IEnumerable<LabelSelector>? labelSelectors = null, ulong? maxResults = null, string? continuationToken = null, CancellationToken cancellationToken = default)
        where TResource : class, IResource, new()
    {
        var resource = new TResource();
        var collection = await database.ListResourcesAsync(resource.Definition.Group, resource.Definition.Version, resource.Definition.Plural, @namespace, labelSelectors, maxResults, continuationToken, cancellationToken).ConfigureAwait(false);
        return collection.OfType<TResource>();
    }

    /// <summary>
    /// Streams <see cref="IResource"/>s of the specified type
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="IResource"/>s to stream</typeparam>
    /// <param name="database">The extended <see cref="IRepository"/></param>
    /// <param name="namespace">The namespace the <see cref="IResource"/>s to stream belongs to, if any. If not set, streams resources across all namespaces</param>
    /// <param name="labelSelectors">A collection of objects used to configure the labels to filter the <see cref="IResource"/>s to stream by</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IAsyncEnumerable{T}"/> used to stream all matching <see cref="IResource"/>s of type specified type</returns>
    public static IAsyncEnumerable<TResource> GetResourcesAsync<TResource>(this IDatabase database, string? @namespace = null, IEnumerable<LabelSelector>? labelSelectors = null, CancellationToken cancellationToken = default)
        where TResource : class, IResource, new()
    {
        var resource = new TResource();
        var stream = database.GetResourcesAsync(resource.Definition.Group, resource.Definition.Version, resource.Definition.Plural, @namespace, labelSelectors, cancellationToken);
        return stream.Select(r => r.ConvertTo<TResource>()!);
    }

}
