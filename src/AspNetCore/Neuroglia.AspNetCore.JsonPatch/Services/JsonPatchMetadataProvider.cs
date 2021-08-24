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
using Neuroglia;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.AspNetCore.JsonPatch
{

    /// <summary>
    /// Represents the default implementation of the <see cref="IJsonPatchMetadataProvider"/> interface
    /// </summary>
    public class JsonPatchMetadataProvider
        : IJsonPatchMetadataProvider
    {

        /// <summary>
        /// Gets a <see cref="ConcurrentDictionary{TKey, TValue}"/> containing the <see cref="IJsonPatchTypeMetadata"/> mappings by object type
        /// </summary>
        protected ConcurrentDictionary<Type, IJsonPatchTypeMetadata> MetadataMappings { get; } = new();

        /// <inheritdoc/>
        public virtual IJsonPatchTypeMetadata GetTypeMetadata(Type typeToPatch)
        {
            if (typeToPatch == null)
                throw new ArgumentNullException(nameof(typeToPatch));
            if (this.MetadataMappings.TryGetValue(typeToPatch, out IJsonPatchTypeMetadata typeMetadata))
                return typeMetadata;
            List<MethodInfo> operationMethods = typeToPatch.GetMethods()
                .Where(m => m.TryGetCustomAttribute<JsonPatchOperationAttribute>(out _))
                .ToList();
            List<IJsonPatchOperationMetadata> operations = new(operationMethods.Count);
            foreach (MethodInfo method in operationMethods)
            {
                JsonPatchOperationAttribute operationAttribute = method.GetCustomAttribute<JsonPatchOperationAttribute>();
                IJsonPatchOperationMetadata operationMetadata = new JsonPatchOperationMetadata(operationAttribute.Type, operationAttribute.Path, method)
                {
                    ReferencedType = operationAttribute.ReferencedType
                };
                operations.Add(operationMetadata);
            }
            typeMetadata = new JsonPatchTypeMetadata(typeToPatch, operations);
            this.MetadataMappings.TryAdd(typeToPatch, typeMetadata);
            return typeMetadata;
        }

    }

}
