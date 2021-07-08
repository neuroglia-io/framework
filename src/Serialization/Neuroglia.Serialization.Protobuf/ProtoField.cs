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

namespace Neuroglia.Serialization
{
    /// <summary>
    /// Represents an object used to describe a <see cref="ProtoObject"/> field
    /// </summary>
    public class ProtoField
    {

        /// <summary>
        /// Initializes a new <see cref="ProtoField"/>
        /// </summary>
        /// <param name="name">The <see cref="ProtoField"/>'s name</param>
        /// <param name="tag">The <see cref="ProtoField"/>'s tag</param>
        /// <param name="protobufType">The <see cref="ProtoField"/>'s protobuf type</param>
        /// <param name="value">The <see cref="ProtoField"/>'s value</param>
        public ProtoField(string name, int tag, FieldDescriptorProto.Type protobufType, object value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            this.Name = name;
            this.Tag = tag;
            this.ProtobufType = protobufType;
            this.RuntimeType = protobufType.ToRuntimeType();
            this.Value = value;
        }

        /// <summary>
        /// Initializes a new <see cref="ProtoField"/>
        /// </summary>
        /// <param name="name">The <see cref="ProtoField"/>'s name</param>
        /// <param name="tag">The <see cref="ProtoField"/>'s tag</param>
        /// <param name="runtimeType">The <see cref="ProtoField"/>'s runtime type</param>
        /// <param name="value">The <see cref="ProtoField"/>'s value</param>
        public ProtoField(string name, int tag, Type runtimeType, object value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            this.Name = name;
            this.Tag = tag;
            this.RuntimeType = runtimeType;
            this.Value = value;
        }

        /// <summary>
        /// Gets the <see cref="ProtoField"/>'s name
        /// </summary>
        public virtual string Name { get; }

        /// <summary>
        /// Gets the <see cref="ProtoField"/>'s tag
        /// </summary>
        public virtual int Tag { get; }

        /// <summary>
        /// Gets the <see cref="ProtoField"/>'s protobuf type
        /// </summary>
        public virtual FieldDescriptorProto.Type ProtobufType { get; }

        /// <summary>
        /// Gets the <see cref="ProtoField"/>'s runtime type
        /// </summary>
        public virtual Type RuntimeType { get; }

        /// <summary>
        /// Gets the <see cref="ProtoField"/>'s value
        /// </summary>
        public virtual object Value { get; internal set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.Name;
        }

    }

}
