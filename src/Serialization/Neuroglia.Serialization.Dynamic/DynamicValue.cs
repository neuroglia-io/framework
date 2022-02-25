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
using ProtoBuf;
using ProtoBuf.WellKnownTypes;
using System.Runtime.Serialization;

namespace Neuroglia.Serialization
{

    /// <summary>
    /// Represents a dynamic, primitive value
    /// </summary>
    [DataContract]
    [ProtoContract]
    public class DynamicValue
        : Dynamic
    {

        /// <summary>
        /// Initializes a new <see cref="Dynamic"/>
        /// </summary>
        protected DynamicValue()
        {

        }

        /// <summary>
        /// Initializes a new <see cref="Dynamic"/>
        /// </summary>
        /// <param name="value">The primitive value to wrap</param>
        public DynamicValue(object value)
        {
            this.Type = DynamicHelper.GetDynamicType(value?.GetType());
            this.Bytes = ProtobufHelper.Serialize(DynamicHelper.ConvertToDynamicValue(value));
        }

        /// <summary>
        /// Gets the bytes of the serialized primitive
        /// </summary>
        [DataMember(Order = 1)]
        [ProtoMember(1)]
        protected virtual byte[] Bytes { get; set; } = null!;

        /// <inheritdoc/>
        public override object ToObject()
        {
            var value = DynamicHelper.DeserializeProtoBuf(this.Bytes, this.Type);
            return value switch
            {
                Timestamp timestamp => timestamp.AsDateTime(),
                Duration duration => duration.AsTimeSpan(),
                _ => value!
            };
        }

        /// <inheritdoc/>
        public override object ToObject(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            var value = ProtobufHelper.Deserialize(this.Bytes, DynamicHelper.GetClrType(DynamicHelper.GetDynamicType(type)));
            return value switch
            {
                string str => type == typeof(Guid) ? Guid.Parse(str) : str,
                Timestamp timestamp => type == typeof(DateTimeOffset) ? (object)new DateTimeOffset(timestamp.AsDateTime()) : timestamp.AsDateTime(),
                Duration duration => duration.AsTimeSpan(),
                _ => value
            };
        }

        /// <inheritdoc/>
        public override T ToObject<T>()
        {
            return (T)this.ToObject(typeof(T));
        }

        /// <inheritdoc/>
        public new static DynamicValue? FromObject(object? value)
        {
            if (value == null)
                return null;
            if (value is DynamicValue primitive)
                return primitive;
            if (!value.GetType().IsPrimitiveType())
                throw new Exception($"The specified value must be a primitive");
            return new DynamicValue(value);
        }

    }

}