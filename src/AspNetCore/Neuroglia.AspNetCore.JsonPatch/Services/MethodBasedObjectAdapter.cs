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
using Microsoft.AspNetCore.JsonPatch.Adapters;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.JsonPatch.Providers
{

    /// <summary>
    /// Represents a reflection-based implementation of the <see cref="IObjectAdapter"/> interface, which allows patching using methods instead of properties
    /// </summary>
    public class MethodBasedObjectAdapter
        : IObjectAdapter
    {

        /// <summary>
        /// Initializes a new <see cref="MethodBasedObjectAdapter"/>
        /// </summary>
        /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
        /// <param name="metadataProvider">The service used to provide <see cref="JsonPatchTypeMetadata"/></param>
        public MethodBasedObjectAdapter(IServiceProvider serviceProvider, IJsonPatchMetadataProvider metadataProvider)
        {
            this.ServiceProvider = serviceProvider;
            this.MetadataProvider = metadataProvider;
        }

        /// <summary>
        /// Gets the current <see cref="IServiceProvider"/>
        /// </summary>
        protected IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Gets the service used to provide <see cref="JsonPatchTypeMetadata"/>
        /// </summary>
        protected IJsonPatchMetadataProvider MetadataProvider { get; }

        public virtual async Task ApplyPatchToAsync(JsonPatchDocument patch, object target, CancellationToken cancellationToken = default)
        {
            if (patch == null)
                throw new ArgumentNullException(nameof(patch));
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            foreach (Operation operation in patch.Operations)
            {
                await this.ApplyToAsync(operation, target, cancellationToken);
            }
        }

        protected virtual async Task ApplyToAsync(Operation operation, object target, CancellationToken cancellationToken = default)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            IJsonPatchTypeMetadata typeMetadata = this.MetadataProvider.GetTypeMetadata(target.GetType());
            if (typeMetadata.TryGetOperationMetadata(operation, out IJsonPatchOperationMetadata operationMetadata))
                throw new InvalidOperationException(); //todo: throw more meaningfull exception
            object value = operation.value;
            if (operationMetadata.ReferencedType != null)
            {
                IRepository repository = (IRepository)this.ServiceProvider.GetRequiredService(typeof(IRepository<>).MakeGenericType(operationMetadata.ReferencedType));
                value = await repository.FindAsync(value, cancellationToken);
            }
            operationMetadata.Invoke(target, value);
        }

        /// <inheritdoc/>
        public virtual void Add(Operation operation, object objectToApplyTo)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));
            if (objectToApplyTo == null)
                throw new ArgumentNullException(nameof(objectToApplyTo));
        }

        /// <inheritdoc/>
        public virtual void Copy(Operation operation, object objectToApplyTo)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));
            if (objectToApplyTo == null)
                throw new ArgumentNullException(nameof(objectToApplyTo));

        }

        /// <inheritdoc/>
        public virtual void Move(Operation operation, object objectToApplyTo)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));
            if (objectToApplyTo == null)
                throw new ArgumentNullException(nameof(objectToApplyTo));

        }

        /// <inheritdoc/>
        public virtual void Remove(Operation operation, object objectToApplyTo)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));
            if (objectToApplyTo == null)
                throw new ArgumentNullException(nameof(objectToApplyTo));

        }

        /// <inheritdoc/>
        public virtual void Replace(Operation operation, object objectToApplyTo)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));
            if (objectToApplyTo == null)
                throw new ArgumentNullException(nameof(objectToApplyTo));

        }

    }

    public interface IJsonPatchMetadataProvider
    {

        IJsonPatchTypeMetadata GetTypeMetadata(Type typeToPatch);

    }

    public class JsonPatchMetadataProvider
        : IJsonPatchMetadataProvider
    {

        /// <inheritdoc/>
        public virtual IJsonPatchTypeMetadata GetTypeMetadata(Type typeToPatch)
        {
            if (typeToPatch == null)
                throw new ArgumentNullException(nameof(typeToPatch));

        }

    }

}
