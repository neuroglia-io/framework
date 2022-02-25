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
using System.Runtime.Serialization;

namespace Neuroglia.Serialization
{

    /// <summary>
    /// Represents the base class for all dynamic values
    /// </summary>
    [DataContract]
    [ProtoContract]
    [ProtoInclude(100, typeof(DynamicValue))]
    [ProtoInclude(200, typeof(DynamicObject))]
    [ProtoInclude(300, typeof(DynamicArray))]
    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.DynamicConverter))]
    public abstract class Dynamic
    {

        /// <summary>
        /// Gets the <see cref="Dynamic"/>'s type
        /// </summary>
        [DataMember(Order = 1)]
        [ProtoMember(1)]
        public virtual DynamicType Type { get; protected set; }

        /// <summary>
        /// Converts the <see cref="Dynamic"/> into a new object
        /// </summary>
        /// <returns>A new object</returns>
        public abstract object? ToObject();

        /// <summary>
        /// Converts the <see cref="Dynamic"/> into a new object of the specified type
        /// </summary>
        /// <param name="type">The type to convert the <see cref="Dynamic"/> into</param>
        /// <returns>A new object of the specified type</returns>
        public abstract object? ToObject(Type type);

        /// <summary>
        /// Converts the <see cref="Dynamic"/> into a new object of the specified type
        /// </summary>
        /// <typeparam name="T">The type to convert the <see cref="Dynamic"/> into</typeparam>
        /// <returns>A new object of the specified type</returns>
        public abstract T ToObject<T>();

        /// <summary>
        /// Converts the specified value into a new <see cref="Dynamic"/>
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns>A new <see cref="Dynamic"/></returns>
        public static Dynamic? FromObject(object? value)
        {
            if (value is Dynamic any)
                return any;
            return DynamicHelper.GetDynamicType(value?.GetType()) switch
            {
                DynamicType.Array => DynamicArray.FromObject(value!),
                DynamicType.Null => null,
                DynamicType.Object => DynamicObject.FromObject(value),
                _ => DynamicValue.FromObject(value),
            };
        }

    }

}