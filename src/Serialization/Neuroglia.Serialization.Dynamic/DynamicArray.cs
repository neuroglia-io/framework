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
using System.Collections;
using System.Runtime.Serialization;

namespace Neuroglia.Serialization
{

    /// <summary>
    /// Describes a dynamic array
    /// </summary>
    [DataContract]
    [ProtoContract]
    public class DynamicArray
        : Dynamic
    {

        /// <summary>
        /// Gets the type of the <see cref="DynamicArray"/>'s elements
        /// </summary>
        [DataMember(Order = 1)]
        [ProtoMember(1)]
        public virtual DynamicType ElementType { get; protected set; }

        /// <summary>
        /// Gets the <see cref="DynamicArray"/>'s serialized value
        /// </summary>
        [DataMember(Order = 2)]
        [ProtoMember(2)]
        protected virtual byte[] Bytes { get; set; } = null!;

        /// <inheritdoc/>
        public override object? ToObject()
        {
            var elementType = DynamicHelper.GetClrType(this.ElementType);
            if (elementType == null)
                return null;
            var enumerableType = typeof(List<>).MakeGenericType(elementType);
            var enumerable = (IEnumerable)ProtobufHelper.Deserialize(this.Bytes, enumerableType);
            var results = new List<object>();
            if (enumerable != null)
            {
                foreach (var elem in enumerable)
                {
                    results.Add(elem switch
                    {
                        Timestamp timestamp => timestamp.AsDateTime(),
                        Duration duration => duration.AsTimeSpan(),
                        DynamicArray array => array.ToObject()!,
                        DynamicObject obj => obj.ToObject()!,
                        _ => elem
                    });
                }
            }
            return results;
        }

        /// <inheritdoc/>
        public override object? ToObject(Type expectedType)
        {
            if (expectedType == null)
                throw new ArgumentNullException(nameof(expectedType));
            if (expectedType == typeof(Dynamic)
                || expectedType == typeof(DynamicArray))
                return this;
            if (!expectedType.IsEnumerable())
                throw new ArgumentException($"The specified type '{expectedType.Name}' is not an {nameof(IEnumerable)} implementation", nameof(expectedType));
            var elementType = DynamicHelper.GetClrType(this.ElementType)!;
            var enumerableType = expectedType;
            var expectedElementType = expectedType.GetEnumerableElementType();
            enumerableType = typeof(List<>).MakeGenericType(elementType);
            var results = new List<object>();
            if (this.Bytes != null)
            {
                var enumerable = (IEnumerable)ProtobufHelper.Deserialize(this.Bytes, enumerableType);
                foreach (var elem in enumerable)
                {
                    results.Add(elem switch
                    {
                        Timestamp timestamp => timestamp.AsDateTime(),
                        Duration duration => duration.AsTimeSpan(),
                        DynamicArray array => array.ToObject()!,
                        DynamicObject obj => obj.ToObject(expectedElementType)!,
                        _ => elem
                    });
                }
            }
            if (expectedType.IsArray)
            {
                return results.OfType(expectedElementType).ToArray();
            }
            else
            {
                var collection = (ICollection)Activator.CreateInstance(expectedType)!;
                collection.AddRange(results);
                return collection;
            }
        }

        /// <inheritdoc/>
        public override T ToObject<T>()
        {
            return (T)this.ToObject(typeof(T))!;
        }

        /// <inheritdoc/>
        public new static DynamicArray? FromObject(object value)
        {
            if (value == null)
                return null;
            if (value is not IEnumerable enumerable)
                throw new ArgumentException("The specified value is not enumerable", nameof(value));
            var tokens = new List<object>();
            foreach (var elem in enumerable)
            {
                tokens.Add(DynamicHelper.ConvertToDynamicValue(elem));
            }
            var tokenType = tokens.FirstOrDefault()?.GetType();
            if (tokenType == null)
                tokenType = typeof(DynamicObject);
            var count = tokens.Count;
            var toserialize = tokens.OfType(tokenType);
            return new()
            {
                ElementType = DynamicHelper.GetDynamicType(tokenType),
                Bytes = count < 1 ? Array.Empty<byte>() : ProtobufHelper.Serialize(toserialize)
            };
        }

    }

}