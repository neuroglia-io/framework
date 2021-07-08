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
using Google.Protobuf.Reflection;
using System;
using System.Collections;

namespace Neuroglia.Serialization
{

    /// <summary>
    /// Defines extensions for the protobuf-net library
    /// </summary>
    public static class FieldDescriptorProtoExtensions
    {

        /// <summary>
        /// Gets the <see cref="FieldDescriptorProto.Type"/>'s runtime <see cref="Type"/>
        /// </summary>
        /// <param name="type">The <see cref="FieldDescriptorProto.Type"/> to get the runtime type for</param>
        /// <returns>The <see cref="FieldDescriptorProto.Type"/>'s runtime <see cref="Type"/></returns>
        public static Type ToRuntimeType(this FieldDescriptorProto.Type type)
        {
            return type switch
            {
                FieldDescriptorProto.Type.TypeBool => typeof(bool),
                FieldDescriptorProto.Type.TypeBytes => typeof(byte[]),
                FieldDescriptorProto.Type.TypeDouble => typeof(double),
                FieldDescriptorProto.Type.TypeFixed32 or FieldDescriptorProto.Type.TypeUint32 => typeof(uint),
                FieldDescriptorProto.Type.TypeFixed64 or FieldDescriptorProto.Type.TypeUint64 => typeof(ulong),
                FieldDescriptorProto.Type.TypeFloat => typeof(float),
                FieldDescriptorProto.Type.TypeGroup => typeof(IEnumerable),
                FieldDescriptorProto.Type.TypeEnum or FieldDescriptorProto.Type.TypeInt32 or FieldDescriptorProto.Type.TypeSfixed32 or FieldDescriptorProto.Type.TypeSint32 => typeof(int),
                FieldDescriptorProto.Type.TypeInt64 or FieldDescriptorProto.Type.TypeSfixed64 or FieldDescriptorProto.Type.TypeSint64 => typeof(long),
                FieldDescriptorProto.Type.TypeMessage => typeof(object),
                FieldDescriptorProto.Type.TypeString => typeof(string),
                _ => throw new NotSupportedException($"The specified protobuf type '{type}' is not supported"),
            };
        }

    }

}
