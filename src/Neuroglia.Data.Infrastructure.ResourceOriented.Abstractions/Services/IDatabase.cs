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

namespace Neuroglia.Data.Infrastructure.ResourceOriented.Services;

/// <summary>
/// Defines the fundamentals of an <see cref="IResource"/> database
/// </summary>
public interface IDatabase
    : IDisposable, IAsyncDisposable
{

    /// <summary>
    /// Initializes the <see cref="IDatabase"/>
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A boolean indicating whether or not the <see cref="IDatabase"/> has been initialized. True means that the <see cref="IDatabase"/> did not exist and has been initialized as expected</returns>
    Task<bool> InitializeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new <see cref="IResource"/>
    /// </summary>
    /// <param name="resource">The <see cref="IResource"/> to create</param>
    /// <param name="group">The API group the <see cref="IResource"/> to create belongs to</param>
    /// <param name="version">The version of the type of <see cref="IResource"/> to create</param>
    /// <param name="plural">The plural name of the type of <see cref="IResource"/> to insert</param>
    /// <param name="namespace">The namespace the <see cref="IResource"/> to create belongs to, if any</param>
    /// <param name="dryRun">A boolean indicating whether or not to persist the changes resulting from the operation</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The created <see cref="IResource"/></returns>
    Task<IResource> CreateResourceAsync(IResource resource, string group, string version, string plural, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the <see cref="IResource"/> with the specified name, if any
    /// </summary>
    /// <param name="group">The API group the <see cref="IResource"/> to get belongs to</param>
    /// <param name="version">The version of the type of <see cref="IResource"/> to get</param>
    /// <param name="plural">The plural name of the type of <see cref="IResource"/> to get</param>
    /// <param name="name">The name of the <see cref="IResource"/> to get</param>
    /// <param name="namespace">The namespace the <see cref="IResource"/> to get belongs to, if any</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The <see cref="IResource"/> with the specified name, if any</returns>
    Task<IResource?> GetResourceAsync(string group, string version, string plural, string name, string? @namespace = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists <see cref="IResource"/>s of the specified type
    /// </summary>
    /// <param name="group">The API group the <see cref="IResource"/>s to list belongs to</param>
    /// <param name="version">The version of the type of <see cref="IResource"/>s to list</param>
    /// <param name="plural">The plural name of the type of <see cref="IResource"/>s to list</param>
    /// <param name="namespace">The namespace the <see cref="IResource"/>s to list belongs to, if any. If not set, lists resources across all namespaces</param>
    /// <param name="labelSelectors">A collection of objects used to configure the labels to filter the <see cref="IResource"/>s to list by</param>
    /// <param name="maxResults">The maximum amount of results that should be returned</param>
    /// <param name="continuationToken">A value used to continue paging resources, in the context of a paging request</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="ICollection"/> that contains all matching <see cref="IResource"/>s of type specified type</returns>
    Task<ICollection> ListResourcesAsync(string group, string version, string plural, string? @namespace = null, IEnumerable<LabelSelector>? labelSelectors = null, ulong? maxResults = null, string? continuationToken = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all <see cref="IResource"/>s of the specified type, asynchronously
    /// </summary>
    /// <param name="group">The API group the <see cref="IResource"/>s to get belongs to</param>
    /// <param name="version">The version of the type of <see cref="IResource"/>s to get</param>
    /// <param name="plural">The plural name of the type of <see cref="IResource"/>s to get</param>
    /// <param name="namespace">The namespace the <see cref="IResource"/>s to get belongs to, if any</param>
    /// <param name="labelSelectors">A collection of objects used to configure the labels to filter the <see cref="IResource"/>s to get by</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IAsyncEnumerable{T}"/> used to get all matching <see cref="IResource"/>s of type specified type</returns>
    IAsyncEnumerable<IResource> GetResourcesAsync(string group, string version, string plural, string? @namespace = null, IEnumerable<LabelSelector>? labelSelectors = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Watches <see cref="IResource"/>s of the specified type
    /// </summary>
    /// <param name="group">The API group the <see cref="IResource"/>s to watch belongs to</param>
    /// <param name="version">The version of the type of <see cref="IResource"/>s to watch</param>
    /// <param name="plural">The plural name of the type of <see cref="IResource"/>s to watch</param>
    /// <param name="namespace">The namespace the <see cref="IResource"/>s to watch belongs to, if any</param>
    /// <param name="labelSelectors">A collection of objects used to configure the labels to filter the <see cref="IResource"/>s to observe by</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The deleted <see cref="IResource"/></returns>
    Task<IResourceWatch> WatchResourcesAsync(string group, string version, string plural, string? @namespace = null, IEnumerable<LabelSelector>? labelSelectors = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Replaces the specified <see cref="IResource"/>
    /// </summary>
    /// <param name="resource">The state to replace the specified resource with</param>
    /// <param name="group">The API group the <see cref="IResource"/> to replace belongs to</param>
    /// <param name="version">The version of the type of <see cref="IResource"/> to replace</param>
    /// <param name="plural">The plural name of the type of <see cref="IResource"/> to replace</param>
    /// <param name="name">The name of the <see cref="IResource"/> to replace</param>
    /// <param name="namespace">The namespace the <see cref="IResource"/> to replace belongs to, if any</param>
    /// <param name="dryRun">A boolean indicating whether or not to persist the changes resulting from the operation</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The deleted <see cref="IResource"/></returns>
    Task<IResource> ReplaceResourceAsync(IResource resource, string group, string version, string plural, string name, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Replaces the specified <see cref="IResource"/>'s sub resource
    /// </summary>
    /// <param name="resource">The state to replace the specified resource with</param>
    /// <param name="group">The API group the <see cref="IResource"/> to replace belongs to</param>
    /// <param name="version">The version of the type of <see cref="IResource"/> to replace</param>
    /// <param name="plural">The plural name of the type of <see cref="IResource"/> to replace</param>
    /// <param name="name">The name of the <see cref="IResource"/> to replace</param>
    /// <param name="subResource">The name of the sub resource to replace (ex: status)</param>
    /// <param name="namespace">The namespace the <see cref="IResource"/> to replace belongs to, if any</param>
    /// <param name="dryRun">A boolean indicating whether or not to persist the changes resulting from the operation</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The deleted <see cref="IResource"/></returns>
    Task<IResource> ReplaceSubResourceAsync(IResource resource, string group, string version, string plural, string name, string subResource, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Patches the specified <see cref="IResource"/>
    /// </summary>
    /// <param name="patch">The patch to apply</param>
    /// <param name="group">The API group the <see cref="IResource"/> to patch belongs to</param>
    /// <param name="version">The version of the type of <see cref="IResource"/> to patch</param>
    /// <param name="plural">The plural name of the type of <see cref="IResource"/> to patch</param>
    /// <param name="name">The name of the <see cref="IResource"/> to patch</param>
    /// <param name="namespace">The namespace the <see cref="IResource"/> to patch belongs to, if any</param>
    /// <param name="dryRun">A boolean indicating whether or not to persist the changes resulting from the operation</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The deleted <see cref="IResource"/></returns>
    Task<IResource> PatchResourceAsync(Patch patch, string group, string version, string plural, string name, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Patches the specified <see cref="IResource"/>'s sub resource
    /// </summary>
    /// <param name="patch">The patch to apply</param>
    /// <param name="group">The API group the <see cref="IResource"/> to patch belongs to</param>
    /// <param name="version">The version of the type of <see cref="IResource"/> to patch</param>
    /// <param name="plural">The plural name of the type of <see cref="IResource"/> to patch</param>
    /// <param name="name">The name of the <see cref="IResource"/> to patch</param>
    /// <param name="subResource">The name of the sub resource to patch ex: status)</param>
    /// <param name="namespace">The namespace the <see cref="IResource"/> to patch belongs to, if any</param>
    /// <param name="dryRun">A boolean indicating whether or not to persist the changes resulting from the operation</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The deleted <see cref="IResource"/></returns>
    Task<IResource> PatchSubResourceAsync(Patch patch, string group, string version, string plural, string name, string subResource, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes the specified <see cref="IResource"/>
    /// </summary>
    /// <param name="group">The API group the <see cref="IResource"/> to remove belongs to</param>
    /// <param name="version">The version of the type of <see cref="IResource"/> to remove</param>
    /// <param name="plural">The plural name of the type of <see cref="IResource"/> to remove</param>
    /// <param name="name">The name of the <see cref="IResource"/> to remove</param>
    /// <param name="namespace">The namespace the <see cref="IResource"/> to remove belongs to, if any</param>
    /// <param name="dryRun">A boolean indicating whether or not to persist the changes resulting from the operation</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The removed <see cref="IResource"/></returns>
    Task<IResource> DeleteResourceAsync(string group, string version, string plural, string name, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default);

}
