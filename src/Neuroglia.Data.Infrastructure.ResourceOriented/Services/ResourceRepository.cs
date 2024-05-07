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

using Microsoft.Extensions.Logging;
using Neuroglia.Data.PatchModel.Services;
using Neuroglia.Security.Services;

namespace Neuroglia.Data.Infrastructure.ResourceOriented.Services;

/// <summary>
/// Represents the default implementation of the <see cref="IResourceRepository"/> interface
/// </summary>
public class ResourceRepository
    : IResourceRepository
{

    private bool _disposed;

    /// <summary>
    /// Initializes a new <see cref="ResourceRepository"/>
    /// </summary>
    /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
    /// <param name="userInfoProvider">The service used to provide information about users</param>
    /// <param name="admissionControl">The service used to control admission of resource operations</param>
    /// <param name="versionControl">The service used to control versioning of admitted resources</param>
    /// <param name="database">The current resource database</param>
    /// <param name="patchHandlers">An <see cref="IEnumerable{T}"/> containing known <see cref="IPatchHandler"/>s</param>
    public ResourceRepository(ILoggerFactory loggerFactory, IUserInfoProvider userInfoProvider, IAdmissionControl admissionControl, IVersionControl versionControl, IDatabase database, IEnumerable<IPatchHandler> patchHandlers)
    {
        this.Logger = loggerFactory.CreateLogger(this.GetType());
        this.UserInfoProvider = userInfoProvider;
        this.AdmissionControl = admissionControl;
        this.VersionControl = versionControl;
        this.Database = database;
        this.PatchHandlers = patchHandlers;
    }

    /// <summary>
    /// Gets the service used to perform logging
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Gets the service used to provide information about users
    /// </summary>
    protected IUserInfoProvider UserInfoProvider { get; }

    /// <summary>
    /// Gets the service used to control admission of resource operations
    /// </summary>
    protected IAdmissionControl AdmissionControl { get; }

    /// <summary>
    /// Gets the service used to control versioning of admitted resources
    /// </summary>
    protected IVersionControl VersionControl { get; }

    /// <summary>
    /// Gets the current resource database
    /// </summary>
    protected IDatabase Database { get; }

    /// <summary>
    /// Gets an <see cref="IEnumerable{T}"/> containing known <see cref="IPatchHandler"/>s
    /// </summary>
    protected IEnumerable<IPatchHandler> PatchHandlers { get; }

    /// <inheritdoc/>
    public virtual async Task<IResource> AddAsync(IResource resource, string group, string version, string plural, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(resource);
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));

        IResource storageResource;
        if (resource.IsResourceDefinition() || resource.IsNamespace())
        {
            storageResource = resource;
        }
        else
        {
            var resourceReference = new ResourceReference(new(group, version, plural), resource.GetName(), resource.GetNamespace());
            var resourceDefinition = await this.GetDefinitionAsync(group, plural, cancellationToken).ConfigureAwait(false) ?? throw new ProblemDetailsException(ResourceProblemDetails.ResourceDefinitionNotFound(resourceReference.Definition));
            var result = await this.AdmissionControl.ReviewAsync(new(Guid.NewGuid().ToShortString(), Operation.Create, resourceReference, null, null, resource, null, this.UserInfoProvider.GetCurrentUser(), dryRun), cancellationToken).ConfigureAwait(false);
            if (!result.Allowed) throw new ProblemDetailsException(ResourceProblemDetails.ResourceAdmissionFailed(Operation.Create, resourceReference, result.Problem?.Errors?.ToArray()!));
            var patchHandler = this.PatchHandlers.FirstOrDefault(h => h.Supports(result.Patch!.Type)) ?? throw new NullReferenceException($"No service registered to handle patches of type '{result.Patch!.Type}'");
            storageResource = (await patchHandler.ApplyPatchAsync(result.Patch!.Document, resource.ConvertTo<Resource>(), cancellationToken).ConfigureAwait(false))!;
            var storageVersion = resourceDefinition.GetStorageVersion();
            if (resource.ApiVersion != storageVersion.Name) storageResource = await this.VersionControl.ConvertToStorageVersionAsync(new VersioningContext(resourceReference, resourceDefinition, storageResource), cancellationToken).ConfigureAwait(false);
        }

        storageResource = await this.Database.CreateResourceAsync(storageResource, group, version, plural, resource.GetNamespace(), dryRun, cancellationToken).ConfigureAwait(false);
        return storageResource;
    }

    /// <inheritdoc/>
    public virtual Task<IResource?> GetAsync(string group, string version, string plural, string name, string? @namespace = null, CancellationToken cancellationToken = default)
    {
        return this.Database.GetResourceAsync(group, version, plural, name, @namespace, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual IAsyncEnumerable<IResource> GetAllAsync(string group, string version, string plural, string? @namespace = null, IEnumerable<LabelSelector>? labelSelectors = null, CancellationToken cancellationToken = default)
    {
        return this.Database.GetResourcesAsync(group, version, plural, @namespace, labelSelectors, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual Task<ICollection> ListAsync(string group, string version, string plural, string? @namespace = null, IEnumerable<LabelSelector>? labelSelectors = null, ulong? maxResults = null, string? continuationToken = null, CancellationToken cancellationToken = default)
    {
        return this.Database.ListResourcesAsync(group, version, plural, @namespace, labelSelectors, maxResults, continuationToken, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual Task<IResourceWatch> WatchAsync(string group, string version, string plural, string? @namespace = null, IEnumerable<LabelSelector>? labelSelectors = null, CancellationToken cancellationToken = default)
    {
        return this.Database.WatchResourcesAsync(group, version, plural, @namespace, labelSelectors, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async Task<IResource> ReplaceAsync(IResource resource, string group, string version, string plural, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(resource);
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));

        var resourceReference = new ResourceReference(new(group, version, plural), resource.GetName(), resource.GetNamespace());
        if (string.IsNullOrWhiteSpace(resource.Metadata.ResourceVersion)) throw new ProblemDetailsException(ResourceProblemDetails.ResourceVersionRequired(resourceReference));

        var originalResource = await this.GetAsync(group, version, plural, resource.GetName(), resource.GetNamespace(), cancellationToken).ConfigureAwait(false);
        var resourceDefinition = (await this.GetDefinitionAsync(group, plural, cancellationToken).ConfigureAwait(false))!;

        var result = await this.AdmissionControl.ReviewAsync(new(Guid.NewGuid().ToShortString(), Operation.Replace, resourceReference, null, null, resource, originalResource, this.UserInfoProvider.GetCurrentUser(), dryRun), cancellationToken).ConfigureAwait(false);
        if (!result.Allowed) throw new ProblemDetailsException(ResourceProblemDetails.ResourceAdmissionFailed(Operation.Replace, resourceReference, result.Problem?.Errors?.ToArray()!));

        var patchHandler = this.PatchHandlers.FirstOrDefault(h => h.Supports(result.Patch!.Type)) ?? throw new NullReferenceException($"No service registered to handle patches of type '{result.Patch!.Type}'");
        var storageResource = result.Patch == null ? resource : (await patchHandler.ApplyPatchAsync(result.Patch!.Document, resource.ConvertTo<Resource>(), cancellationToken).ConfigureAwait(false))!;
        var storageVersion = resourceDefinition.GetStorageVersion();
        if (storageResource.ApiVersion != storageVersion.Name) storageResource = await this.VersionControl.ConvertToStorageVersionAsync(new VersioningContext(resourceReference, resourceDefinition, storageResource), cancellationToken).ConfigureAwait(false);
        storageResource.Metadata.ResourceVersion = resource.Metadata.ResourceVersion;

        storageResource = await this.Database.ReplaceResourceAsync(storageResource, group, version, plural, resource.GetName(), resource.GetNamespace(), dryRun, cancellationToken).ConfigureAwait(false);

        return storageResource;
    }

    /// <inheritdoc/>
    public virtual async Task<IResource> PatchAsync(Patch patch, string group, string version, string plural, string name, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(patch);
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

        var originalResource = await this.GetAsync(group, version, plural, name, @namespace, cancellationToken).ConfigureAwait(false);
        var resourceReference = new ResourceReference(new(group, version, plural), name, @namespace);

        var result = await this.AdmissionControl.ReviewAsync(new(Guid.NewGuid().ToShortString(), Operation.Patch, resourceReference, null, patch, null, originalResource, this.UserInfoProvider.GetCurrentUser(), dryRun), cancellationToken).ConfigureAwait(false);
        if (!result.Allowed) throw new ProblemDetailsException(ResourceProblemDetails.ResourceAdmissionFailed(Operation.Patch, resourceReference, result.Problem?.Errors?.ToArray()!));

        var patchToApply = result.Patch ?? patch;
        var patchHandler = this.PatchHandlers.FirstOrDefault(h => h.Supports(patchToApply.Type)) ?? throw new NullReferenceException($"No service registered to handle patches of type '{patchToApply.Type}'");
        var patchedResource = (await patchHandler.ApplyPatchAsync(patchToApply!.Document, originalResource.ConvertTo<Resource>(), cancellationToken).ConfigureAwait(false))!;
        var diffPatch = JsonPatchUtility.CreateJsonPatchFromDiff(originalResource, patchedResource);
        if (diffPatch.Operations.Any(o => o.Path.Segments[0] == nameof(Resource.Metadata).ToCamelCase() ? o.Path.Segments[1] != nameof(ResourceMetadata.Labels).ToCamelCase() && o.Path.Segments[1] != nameof(ResourceMetadata.Annotations).ToCamelCase() : o.Path.Segments.First() != "spec"))
            throw new ProblemDetailsException(ResourceProblemDetails.InvalidResourcePatch(resourceReference));

        return await this.Database.PatchResourceAsync(patchToApply, group, version, plural, name, @namespace, dryRun, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public virtual async Task<IResource> ReplaceSubResourceAsync(IResource resource, string group, string version, string plural, string subResource, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(resource);
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));

        var resourceReference = new SubResourceReference(new(group, version, plural), resource.GetName(), subResource, resource.GetNamespace());
        if (string.IsNullOrWhiteSpace(resource.Metadata.ResourceVersion)) throw new ProblemDetailsException(ResourceProblemDetails.ResourceVersionRequired(resourceReference));

        var originalResource = await this.GetAsync(group, version, plural, resource.GetName(), resource.GetNamespace(), cancellationToken).ConfigureAwait(false);
        var resourceDefinition = (await this.GetDefinitionAsync(group, plural, cancellationToken).ConfigureAwait(false))!;

        var result = await this.AdmissionControl.ReviewAsync(new(Guid.NewGuid().ToShortString(), Operation.Replace, resourceReference, subResource, null, resource, originalResource, this.UserInfoProvider.GetCurrentUser(), dryRun), cancellationToken).ConfigureAwait(false);
        if (!result.Allowed) throw new ProblemDetailsException(ResourceProblemDetails.ResourceAdmissionFailed(Operation.Replace, resourceReference, result.Problem?.Errors?.ToArray()!));
        
        var patchHandler = this.PatchHandlers.FirstOrDefault(h => h.Supports(result.Patch!.Type)) ?? throw new NullReferenceException($"No service registered to handle patches of type '{result.Patch!.Type}'");
        var storageResource = result.Patch == null ? resource : (await patchHandler.ApplyPatchAsync(result.Patch!.Document, resource.ConvertTo<Resource>(), cancellationToken).ConfigureAwait(false))!;
        var storageVersion = resourceDefinition.GetStorageVersion();
        if (storageResource.ApiVersion != storageVersion.Name) storageResource = await this.VersionControl.ConvertToStorageVersionAsync(new VersioningContext(resourceReference, resourceDefinition, storageResource), cancellationToken).ConfigureAwait(false);
        storageResource.Metadata.ResourceVersion = resource.Metadata.ResourceVersion;

        storageResource = await this.Database.ReplaceSubResourceAsync(storageResource, group, version, plural, resource.GetName(), subResource, resource.GetNamespace(), dryRun, cancellationToken).ConfigureAwait(false);

        return storageResource;
    }

    /// <inheritdoc/>
    public virtual async Task<IResource> PatchSubResourceAsync(Patch patch, string group, string version, string plural, string name, string subResource, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(patch);
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

        var originalResource = await this.GetAsync(group, version, plural, name, @namespace, cancellationToken).ConfigureAwait(false);
        var resourceReference = new ResourceReference(new(group, version, plural), name, @namespace);

        var result = await this.AdmissionControl.ReviewAsync(new(Guid.NewGuid().ToShortString(), Operation.Patch, resourceReference, subResource, patch, null, originalResource, this.UserInfoProvider.GetCurrentUser(), dryRun), cancellationToken).ConfigureAwait(false);
        if (!result.Allowed) throw new ProblemDetailsException(ResourceProblemDetails.ResourceAdmissionFailed(Operation.Patch, resourceReference, result.Problem?.Errors?.ToArray()!));

        var patchToApply = result.Patch ?? patch;
        var patchHandler = this.PatchHandlers.FirstOrDefault(h => h.Supports(patchToApply.Type)) ?? throw new NullReferenceException($"No service registered to handle patches of type '{patchToApply.Type}'");
        var patchedResource = (await patchHandler.ApplyPatchAsync(patchToApply.Document, originalResource.ConvertTo<Resource>(), cancellationToken).ConfigureAwait(false))!;
        var diffPatch = JsonPatchUtility.CreateJsonPatchFromDiff(originalResource, patchedResource);
        if (diffPatch.Operations.Any(o => o.Path.Segments.First() != subResource)) throw new ProblemDetailsException(ResourceProblemDetails.InvalidSubResourcePatch(resourceReference));

        return await this.Database.PatchSubResourceAsync(patchToApply, group, version, plural, name, subResource, @namespace, dryRun, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public virtual async Task<IResource> RemoveAsync(string group, string version, string plural, string name, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

        var originalResource = await this.GetAsync(group, version, plural, name, @namespace, cancellationToken).ConfigureAwait(false) ?? throw new ProblemDetailsException(ResourceProblemDetails.ResourceNotFound(new ResourceReference(new(group, version, plural), name, @namespace)));
        var resourceReference = new ResourceReference(new(group, version, plural), name, @namespace);

        var result = await this.AdmissionControl.ReviewAsync(new(Guid.NewGuid().ToShortString(), Operation.Delete, resourceReference, null, null, null, originalResource, this.UserInfoProvider.GetCurrentUser(), dryRun), cancellationToken).ConfigureAwait(false);
        if (!result.Allowed) throw new ProblemDetailsException(ResourceProblemDetails.ResourceAdmissionFailed(Operation.Delete, resourceReference, result.Problem?.Errors?.ToArray()!));

        await this.Database.DeleteResourceAsync(group, version, plural, name, @namespace, dryRun, cancellationToken).ConfigureAwait(false);

        return originalResource;
    }

    /// <summary>
    /// Disposes of the <see cref="IResourceRepository"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not to dispose of the <see cref="IResourceRepository"/></param>
    /// <returns>A new <see cref="ValueTask"/></returns>
    protected virtual ValueTask DisposeAsync(bool disposing)
    {
        if (!this._disposed)
        {
            if (disposing)
            {

            }
            this._disposed = true;
        }
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        await this.DisposeAsync(disposing: true).ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes of the <see cref="IResourceRepository"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not to dispose of the <see cref="IResourceRepository"/></param>
    protected virtual void Dispose(bool disposing)
    {
        if (!this._disposed)
        {
            if (disposing)
            {

            }
            this._disposed = true;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

}
