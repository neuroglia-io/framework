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
using System;
using System.Linq;
using System.Reflection;

namespace Microsoft.AspNetCore.JsonPatch
{

    /// <summary>
    /// Represents the default implementation of the <see cref="IJsonPatchOperationMetadata"/> interface
    /// </summary>
    public class JsonPatchOperationMetadata
        : IJsonPatchOperationMetadata
    {

        /// <summary>
        /// Initializes a new <see cref="JsonPatchOperationMetadata"/>
        /// </summary>
        /// <param name="type">The Json Patch operation type</param>
        /// <param name="path">The Json Patch operation path</param>
        /// <param name="member">The <see cref="MemberInfo"/> used to apply the Json Patch operation</param>
        public JsonPatchOperationMetadata(string type, string path, MemberInfo member)
        {
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentNullException(nameof(type));
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));
            this.OperationType = type;
            this.Path = path;
            this.Member = member ?? throw new ArgumentNullException(nameof(member));
            this.ValueType = this.Member switch
            {
                FieldInfo field => field.FieldType,
                PropertyInfo property => property.PropertyType,
                MethodInfo method => method.GetParameters().Length == 1 ? method.GetParameters().First().ParameterType : method.GetParameters().Last().ParameterType,
                _ => throw new NotSupportedException($"The specified member type '{this.Member.MemberType}' is not supported"),
            };
        }

        /// <inheritdoc/>
        public virtual string OperationType { get; }

        /// <inheritdoc/>
        public virtual string Path { get; }

        /// <inheritdoc/>
        public virtual Type ValueType { get; }

        /// <inheritdoc/>
        public virtual Type ReferencedType { get; init; }

        /// <summary>
        /// Gets the <see cref="MemberInfo"/> used to apply the Json Patch operation
        /// </summary>
        public virtual MemberInfo Member { get; }

        /// <inheritdoc/>
        public virtual void ApplyTo(object target, object value)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            switch (this.Member)
            {
                case FieldInfo field:
                    field.SetValue(target, value);
                    break;
                case MethodInfo method:
                    object[] args;
                    if (value is Tuple<object, object> tuple)
                        args = new object[] { tuple.Item1, tuple.Item2 };
                    else
                        args = new object[] { value };
                    method.Invoke(target, args);
                    break;
                case PropertyInfo property:
                    property.SetValue(target, value);
                    break;
                default:
                    throw new NotSupportedException($"The specified member type '{this.Member.MemberType}' is not supported");
            }
        }

    }

}
