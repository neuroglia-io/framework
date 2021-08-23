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
using Microsoft.AspNetCore.JsonPatch.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.AspNetCore.JsonPatch
{

    public interface IJsonPatchTypeMetadata
    {

        Type Type { get; }

        bool TryGetOperationMetadata(string type, string path, out IJsonPatchOperationMetadata operationMetadata);

    }

    public static class JsonPatchTypeMetadataExtensions
    {

        public static bool TryGetOperationMetadata(this IJsonPatchTypeMetadata typeMetadata, Operation operation, out IJsonPatchOperationMetadata operationMetadata)
        {
            if(typeMetadata == null)
                throw new ArgumentNullException(nameof(typeMetadata));
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));
            return typeMetadata.TryGetOperationMetadata(operation.op, operation.path, out operationMetadata);
        }

    }

    public class JsonPatchTypeMetadata
        : IJsonPatchTypeMetadata
    {

        public virtual Type Type { get; }

        public virtual IReadOnlyCollection<IJsonPatchOperationMetadata> Operations { get; }

        public virtual bool TryGetOperationMetadata(string type, string path, out IJsonPatchOperationMetadata operationMetadata)
        {
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentNullException(nameof(type));
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));
            operationMetadata = this.Operations
                .FirstOrDefault(o => 
                    o.Type.Equals(type, StringComparison.OrdinalIgnoreCase) 
                    && (o.Path.Equals(path, StringComparison.OrdinalIgnoreCase)
                    || o.Path.Equals($"/{path}", StringComparison.OrdinalIgnoreCase)));
            return operationMetadata != null;
        }

    }

    public interface IJsonPatchOperationMetadata
    {

        string Type { get; }

        string Path { get; }

        Type ReferencedType { get; }

        MemberInfo Member { get; }

    }

    public class JsonPatchOperationMetadata
        : IJsonPatchOperationMetadata
    {

        public virtual string Type { get; }

        public virtual string Path { get; }

        public virtual Type ReferencedType { get; }

        public virtual MemberInfo Member { get; }

    }

}
