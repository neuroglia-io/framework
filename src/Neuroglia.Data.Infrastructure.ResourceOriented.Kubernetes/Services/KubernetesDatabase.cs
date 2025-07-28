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
/// Represents a Kubernetes implementation of the <see cref="IDatabase"/> interface.
/// </summary>
public class KubernetesDatabase(ILogger<KubernetesDatabase> logger, IKubernetes kubernetes, IJsonSerializer jsonSerializer)
    : IDatabase
{

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

    /// <inheritdoc/>
    public virtual Task<bool> InitializeAsync(CancellationToken cancellationToken = default) => Task.FromResult(true);

    /// <inheritdoc/>
    public virtual async Task<IResource> CreateResourceAsync(IResource resource, string group, string version, string plural, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        var result = string.IsNullOrWhiteSpace(@namespace)
            ? await Kubernetes.CustomObjects.CreateClusterCustomObjectAsync(resource, group, version, plural, dryRun ? "All" :  null, cancellationToken: cancellationToken).ConfigureAwait(false)
            : await Kubernetes.CustomObjects.CreateNamespacedCustomObjectAsync(resource, group, version, @namespace, plural, dryRun ? "All" : null, cancellationToken: cancellationToken).ConfigureAwait(false);
        return (IResource)JsonSerializer.Convert(result, resource.GetType())!;
    }

    /// <inheritdoc/>
    public virtual async Task<IResource?> GetResourceAsync(string group, string version, string plural, string name, string? @namespace = null, CancellationToken cancellationToken = default)
    {
        var result = string.IsNullOrWhiteSpace(@namespace)
            ? await Kubernetes.CustomObjects.GetClusterCustomObjectAsync(group, version, plural, name, cancellationToken: cancellationToken).ConfigureAwait(false)
            : await Kubernetes.CustomObjects.GetNamespacedCustomObjectAsync(group, version, @namespace, plural, name, cancellationToken: cancellationToken).ConfigureAwait(false);
        return (IResource?)JsonSerializer.Convert(result, typeof(Resource));
    }

    /// <inheritdoc/>
    public virtual async Task<ICollection> ListResourcesAsync(string group, string version, string plural, string? @namespace = null, IEnumerable<LabelSelector>? labelSelectors = null, ulong? maxResults = null, string? continuationToken = null, CancellationToken cancellationToken = default)
    {
        var labelSelector = labelSelectors?.Select(ls => ls.ToString()).Join(',');
        var limit = maxResults.HasValue ? (int?)maxResults.Value : null;
        var results = string.IsNullOrWhiteSpace(@namespace)
            ? await Kubernetes.CustomObjects.ListClusterCustomObjectAsync(group, version, plural, labelSelector: labelSelector, limit: limit, continueParameter: continuationToken, cancellationToken: cancellationToken).ConfigureAwait(false)
            : await Kubernetes.CustomObjects.ListNamespacedCustomObjectAsync(group, version, @namespace, plural, labelSelector: labelSelector, limit: limit, continueParameter: continuationToken, cancellationToken: cancellationToken).ConfigureAwait(false);
        return (Collection)JsonSerializer.Convert(results, typeof(Collection))!;
    }

    /// <inheritdoc/>
    public virtual async IAsyncEnumerable<IResource> GetResourcesAsync(string group, string version, string plural, string? @namespace = null, IEnumerable<LabelSelector>? labelSelectors = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var labelSelector = labelSelectors?.Select(ls => ls.ToString()).Join(',');
        var results = string.IsNullOrWhiteSpace(@namespace)
            ? await Kubernetes.CustomObjects.ListClusterCustomObjectAsync(group, version, plural, labelSelector: labelSelector, cancellationToken: cancellationToken).ConfigureAwait(false)
            : await Kubernetes.CustomObjects.ListNamespacedCustomObjectAsync(group, version, @namespace, plural, labelSelector: labelSelector, cancellationToken: cancellationToken).ConfigureAwait(false);
        var collection = (Collection)JsonSerializer.Convert(results, typeof(Collection))!;
        if (collection.Items is null) yield break;
        foreach(var item in collection.Items) yield return (IResource)JsonSerializer.Convert(item, typeof(Resource))!;
    }

    /// <inheritdoc/>
    public virtual async Task<IResourceWatch> WatchResourcesAsync(string group, string version, string plural, string? @namespace = null, IEnumerable<LabelSelector>? labelSelectors = null, CancellationToken cancellationToken = default)
    {
        var response = string.IsNullOrWhiteSpace(@namespace)
            ? await Kubernetes.CustomObjects.ListClusterCustomObjectWithHttpMessagesAsync(group, version, plural,watch: true,cancellationToken: cancellationToken).ConfigureAwait(false)
            : await Kubernetes.CustomObjects.ListNamespacedCustomObjectWithHttpMessagesAsync(group, version, @namespace, plural,watch: true,cancellationToken: cancellationToken).ConfigureAwait(false);
        var observable = Observable.Create<IResourceWatchEvent>(observer =>
        {
            var watcher = response.Watch<Resource, object>(
                (type, resource) =>
                {
                    observer.OnNext(new ResourceWatchEvent
                    {
                        Type = type switch
                        {
                            WatchEventType.Added => ResourceWatchEventType.Created,
                            WatchEventType.Bookmark => ResourceWatchEventType.Bookmark,
                            WatchEventType.Modified => ResourceWatchEventType.Updated,
                            WatchEventType.Deleted => ResourceWatchEventType.Deleted,
                            WatchEventType.Error => ResourceWatchEventType.Error,
                            _ => throw new NotSupportedException($"The specified watch event type '{EnumHelper.GetDisplayName(type)}' is not supported")
                        },
                        Resource = resource
                    });
                },
                ex => observer.OnError(ex),
                () => observer.OnCompleted()
            );

            return Disposable.Create(() => watcher.Dispose());
        });
        return new ResourceWatch(observable, true);
    }

    /// <inheritdoc/>
    public virtual async Task<IResource> PatchResourceAsync(Patch patch, string group, string version, string plural, string name, string? @namespace = null, string? resourceVersion = null, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        var result = string.IsNullOrWhiteSpace(@namespace)
            ? await Kubernetes.CustomObjects.PatchClusterCustomObjectAsync(patch, group, version, plural, name, dryRun: dryRun ? "All" : null, cancellationToken: cancellationToken).ConfigureAwait(false)
            : await Kubernetes.CustomObjects.PatchNamespacedCustomObjectAsync(patch, group, version, @namespace, plural, name, dryRun: dryRun ? "All" : null, cancellationToken: cancellationToken).ConfigureAwait(false);
        return (IResource)JsonSerializer.Convert(result, typeof(Resource))!;
    }

    /// <inheritdoc/>
    public virtual async Task<IResource> ReplaceResourceAsync(IResource resource, string group, string version, string plural, string name, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        var result = string.IsNullOrWhiteSpace(@namespace)
            ? await Kubernetes.CustomObjects.ReplaceClusterCustomObjectAsync(resource, group, version, plural, name, dryRun: dryRun ? "All" : null, cancellationToken: cancellationToken).ConfigureAwait(false)
            : await Kubernetes.CustomObjects.ReplaceNamespacedCustomObjectAsync(resource, group, version, @namespace, plural, name, dryRun: dryRun ? "All" : null, cancellationToken: cancellationToken).ConfigureAwait(false);
        return (IResource)JsonSerializer.Convert(result, typeof(Resource))!;
    }

    /// <inheritdoc/>
    public virtual async Task<IResource> PatchSubResourceAsync(Patch patch, string group, string version, string plural, string name, string subResource, string? @namespace = null, string? resourceVersion = null, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        if (subResource == "status") throw new NotSupportedException($"Patching the specified sub-resource '{subResource}' is not supported");
        var result = string.IsNullOrWhiteSpace(@namespace)
           ? await Kubernetes.CustomObjects.PatchClusterCustomObjectStatusAsync(patch, group, version, plural, name, dryRun: dryRun ? "All" : null, cancellationToken: cancellationToken).ConfigureAwait(false)
           : await Kubernetes.CustomObjects.PatchNamespacedCustomObjectStatusAsync(patch, group, version, @namespace, plural, name, dryRun: dryRun ? "All" : null, cancellationToken: cancellationToken).ConfigureAwait(false);
        return (IResource)JsonSerializer.Convert(result, typeof(Resource))!;
    }

    /// <inheritdoc/>
    public virtual async Task<IResource> ReplaceSubResourceAsync(IResource resource, string group, string version, string plural, string name, string subResource, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        if (subResource == "status") throw new NotSupportedException($"Patching the specified sub-resource '{subResource}' is not supported");
        var result = string.IsNullOrWhiteSpace(@namespace)
            ? await Kubernetes.CustomObjects.ReplaceClusterCustomObjectStatusAsync(resource, group, version, plural, name, dryRun: dryRun ? "All" : null, cancellationToken: cancellationToken).ConfigureAwait(false)
            : await Kubernetes.CustomObjects.ReplaceNamespacedCustomObjectStatusAsync(resource, group, version, @namespace, plural, name, dryRun: dryRun ? "All" : null, cancellationToken: cancellationToken).ConfigureAwait(false);
        return (IResource)JsonSerializer.Convert(result, typeof(Resource))!;
    }

    /// <inheritdoc/>
    public virtual async Task<IResource> DeleteResourceAsync(string group, string version, string plural, string name, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default)
    {
        var result = string.IsNullOrWhiteSpace(@namespace)
            ? await Kubernetes.CustomObjects.DeleteClusterCustomObjectAsync(group, version, plural, name, dryRun: dryRun ? "All" : null, cancellationToken: cancellationToken).ConfigureAwait(false)
            : await Kubernetes.CustomObjects.DeleteNamespacedCustomObjectAsync(group, version, @namespace, plural, name, dryRun: dryRun ? "All" : null, cancellationToken: cancellationToken).ConfigureAwait(false);
        return (IResource)JsonSerializer.Convert(result, typeof(Resource))!;
    }

    /// <summary>
    /// Disposes of the <see cref="KubernetesDatabase"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not to dispose of the <see cref="KubernetesDatabase"/></param>
    /// <returns>A new awaitable <see cref="ValueTask"/></returns>
    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (!this._disposed || !disposing) return;
        await ValueTask.CompletedTask.ConfigureAwait(false);
        this._disposed = true;
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
