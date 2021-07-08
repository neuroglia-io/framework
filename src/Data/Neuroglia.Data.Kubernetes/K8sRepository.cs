/*
 * Copyright © 2021 Neuroglia SPRL. All rights reserved.
 * <p>
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * <p>
 * http://www.apache.org/licenses/LICENSE-2.0
 * <p>
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */
using k8s;
using k8s.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using Neuroglia.K8s;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.Data
{

    /// <summary>
    /// Represents an <see cref="IRepository{TEntity, TKey}"/> implementation used to manage <see cref="ICustomResource"/>s
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="ICustomResource"/> to manage</typeparam>
    public class K8sRepository<TResource>
        : RepositoryBase<TResource, string>, ICustomResourceRepository<TResource>
        where TResource : class, ICustomResource, IIdentifiable<string>, new()
    {

        /// <summary>
        /// Initializes a new <see cref="K8sRepository{TResource}"/>
        /// </summary>
        /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
        /// <param name="kubernetes">The service used to interact with the Kubernetes API</param>
        public K8sRepository(ILoggerFactory loggerFactory, IKubernetes kubernetes)
        {
            this.Logger = loggerFactory.CreateLogger(this.GetType());
            this.Kubernetes = kubernetes;
            this.ResourceDefinition = new TResource().Definition;
        }

        /// <summary>
        /// Gets the service used to perform logging
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the service used to interact with the Kubernetes API
        /// </summary>
        protected IKubernetes Kubernetes { get; }

        /// <summary>
        /// Gets the definition of the <see cref="ICustomResource"/>s to manage
        /// </summary>
        protected ICustomResourceDefinition ResourceDefinition { get; }

        /// <inheritdoc/>
        public override async Task<bool> ContainsAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                TResource resource = await this.FindAsync(key, cancellationToken);
                return resource != null;
            }
            catch (HttpOperationException ex) when (ex.Response.StatusCode == HttpStatusCode.NotFound) 
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<TResource>> FilterAsync(string labelSelector = null, string fieldSelector = null, string @namespace = null, CancellationToken cancellationToken = default)
        {
            KubernetesList<TResource> resources;
            if (string.IsNullOrWhiteSpace(@namespace))
                resources = await this.Kubernetes.ListClusterCustomObjectAsync<TResource>(this.ResourceDefinition.Group, this.ResourceDefinition.Version, this.ResourceDefinition.Plural, fieldSelector: fieldSelector, labelSelector: labelSelector, cancellationToken: cancellationToken);
            else
                resources = await this.Kubernetes.ListNamespacedCustomObjectAsync<TResource>(this.ResourceDefinition.Group, this.ResourceDefinition.Version, @namespace, this.ResourceDefinition.Plural, fieldSelector: fieldSelector, labelSelector: labelSelector, cancellationToken: cancellationToken);
            return resources.Items;
        }

        /// <inheritdoc/>
        public override async Task<TResource> FindAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                    throw new ArgumentNullException(nameof(key));
                return ((JObject)await this.Kubernetes.GetClusterCustomObjectAsync(this.ResourceDefinition.Group, this.ResourceDefinition.Version, this.ResourceDefinition.Plural, key, cancellationToken: cancellationToken)).ToObject<TResource>();
            }
            catch (HttpOperationException ex) when (ex.Response.StatusCode == HttpStatusCode.NotFound)
            {
                return default;
            }
        }

        /// <inheritdoc/>
        public override Task<TResource> FindAsync(object[] keyValues, CancellationToken cancellationToken = default)
        {
            if (keyValues == null
                || keyValues.Length < 1)
                throw new ArgumentNullException(nameof(keyValues));
            return this.FindAsync((string)keyValues.First(), cancellationToken);
        }

        /// <inheritdoc/>
        public override async Task<TResource> AddAsync(TResource entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            return ((JObject)await this.Kubernetes.CreateNamespacedCustomObjectAsync(entity, entity.Definition.Group, entity.Definition.Version, entity.Namespace(), entity.Definition.Plural, cancellationToken: cancellationToken)).ToObject<TResource>();
        }

        /// <inheritdoc/>
        public override async Task<TResource> UpdateAsync(TResource entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            if (entity is IPatchable)
                entity = await this.PatchAsync(entity, cancellationToken);
            else
                entity = await this.PutAsync(entity, cancellationToken);
            if (entity is IStatusPatchable)
                entity = await this.PatchStatusAsync(entity, cancellationToken);
            return entity;
        }

        /// <inheritdoc/>
        protected virtual async Task<TResource> PatchAsync(TResource entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            if (entity is not IPatchable patchable
                || patchable.TryGetPatch(out JsonPatchDocument patch))
                return entity;
            return ((JObject)await this.Kubernetes.PatchNamespacedCustomObjectAsync(new V1Patch(patch, V1Patch.PatchType.JsonPatch), this.ResourceDefinition.Group, this.ResourceDefinition.Version, entity.Namespace(), this.ResourceDefinition.Plural, entity.Name(), cancellationToken: cancellationToken)).ToObject<TResource>();
        }

        /// <inheritdoc/>
        protected virtual async Task<TResource> PatchStatusAsync(TResource entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            if (entity is not IStatusPatchable patchable
                || !patchable.TryGetStatusPatch(out JsonPatchDocument patch))
                return entity;
            return ((JObject)await this.Kubernetes.PatchNamespacedCustomObjectStatusAsync(new V1Patch(patch, V1Patch.PatchType.JsonPatch), this.ResourceDefinition.Group, this.ResourceDefinition.Version, entity.Namespace(), this.ResourceDefinition.Plural, entity.Name(), cancellationToken: cancellationToken)).ToObject<TResource>();
        }

        /// <inheritdoc/>
        protected virtual async Task<TResource> PutAsync(TResource entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            return ((JObject)await this.Kubernetes.ReplaceNamespacedCustomObjectAsync(entity, entity.Definition.Group, entity.Definition.Version, entity.Namespace(), entity.Definition.Plural, entity.Name(), cancellationToken: cancellationToken)).ToObject<TResource>();
        }

        /// <inheritdoc/>
        public override async Task<TResource> RemoveAsync(TResource entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            return ((JObject)await this.Kubernetes.DeleteNamespacedCustomObjectAsync(entity.Definition.Group, entity.Definition.Version, entity.Namespace(), entity.Definition.Plural, entity.Name(), cancellationToken: cancellationToken)).ToObject<TResource>();
        }

        /// <inheritdoc/>
        public override async Task<List<TResource>> ToListAsync(CancellationToken cancellationToken = default)
        {
            KubernetesList<TResource> resources = await this.Kubernetes.ListClusterCustomObjectAsync<TResource>(this.ResourceDefinition.Group, this.ResourceDefinition.Version, this.ResourceDefinition.Plural, cancellationToken: cancellationToken);
            return resources.Items.ToList();
        }

        /// <inheritdoc/>
        public override IQueryable<TResource> AsQueryable()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public override Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

    }

}
