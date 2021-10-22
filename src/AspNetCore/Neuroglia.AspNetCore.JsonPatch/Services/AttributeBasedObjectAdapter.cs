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
using Neuroglia;
using Neuroglia.Data;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Reflection;

namespace Microsoft.AspNetCore.JsonPatch
{

    /// <summary>
    /// Represents a reflection-based implementation of the <see cref="IObjectAdapter"/> interface, which allows patching using methods instead of properties
    /// </summary>
    public class AttributeBasedObjectAdapter
        : IObjectAdapter
    {

        private static readonly MethodInfo ElementAtMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.ElementAt));

        /// <summary>
        /// Initializes a new <see cref="AttributeBasedObjectAdapter"/>
        /// </summary>
        /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
        /// <param name="metadataProvider">The service used to provide <see cref="JsonPatchTypeMetadata"/></param>
        public AttributeBasedObjectAdapter(IServiceProvider serviceProvider, IJsonPatchMetadataProvider metadataProvider)
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

        /// <summary>
        /// Applies a given <see cref="Operation"/> to the specified object
        /// </summary>
        /// <param name="operation">The <see cref="Operation"/> to apply</param>
        /// <param name="target">The object to apply the <see cref="Operation"/> to</param>
        protected virtual void ApplyTo(Operation operation, object target)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            IJsonPatchTypeMetadata typeMetadata = this.MetadataProvider.GetTypeMetadata(target.GetType());
            if (!typeMetadata.TryGetOperationMetadata(operation, out IJsonPatchOperationMetadata operationMetadata))
                throw new InvalidOperationException($"Failed to find a Patch Operation of type '{operation.op}' at path '{operation.path}' for type '{target.GetType().Name}'");
            object value = operation.value;
            if(operation.OperationType == OperationType.Remove)
            {
                string path = operation.path;
                if (path.StartsWith("/"))
                    path = path.Substring(1);
                value = target.GetType().GetProperty(path).GetValue(target);
                value = ElementAtMethod.MakeGenericMethod(value.GetType().GetEnumerableElementType()).Invoke(null, new object[] { value, (int)operation.value });
            }
            if (operationMetadata.ReferencedType != null)
            {
                IRepository repository = (IRepository)this.ServiceProvider.GetRequiredService(typeof(IRepository<>).MakeGenericType(operationMetadata.ValueType));
                value = (repository.FindAsync(value)).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            if (value is JObject jObject)
                value = jObject.ToObject(operationMetadata.ValueType);
            operationMetadata.ApplyTo(target, value);
        }

        /// <inheritdoc/>
        public virtual void Add(Operation operation, object objectToApplyTo)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));
            if (objectToApplyTo == null)
                throw new ArgumentNullException(nameof(objectToApplyTo));
            this.ApplyTo(operation, objectToApplyTo);
        }

        /// <inheritdoc/>
        public virtual void Copy(Operation operation, object objectToApplyTo)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));
            if (objectToApplyTo == null)
                throw new ArgumentNullException(nameof(objectToApplyTo));
            this.ApplyTo(operation, objectToApplyTo);
        }

        /// <inheritdoc/>
        public virtual void Move(Operation operation, object objectToApplyTo)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));
            if (objectToApplyTo == null)
                throw new ArgumentNullException(nameof(objectToApplyTo));
            this.ApplyTo(operation, objectToApplyTo);
        }

        /// <inheritdoc/>
        public virtual void Remove(Operation operation, object objectToApplyTo)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));
            if (objectToApplyTo == null)
                throw new ArgumentNullException(nameof(objectToApplyTo));
            this.ApplyTo(operation, objectToApplyTo);
        }

        /// <inheritdoc/>
        public virtual void Replace(Operation operation, object objectToApplyTo)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));
            if (objectToApplyTo == null)
                throw new ArgumentNullException(nameof(objectToApplyTo));
            this.ApplyTo(operation, objectToApplyTo);
        }

    }

}
