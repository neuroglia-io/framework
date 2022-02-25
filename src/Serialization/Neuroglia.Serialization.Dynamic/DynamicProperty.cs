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
    /// Represents a dynamic object property
    /// </summary>
    [DataContract]
    [ProtoContract]
    public class DynamicProperty
    {

        /// <summary>
        /// Initializes a new <see cref="DynamicProperty"/>
        /// </summary>
        protected DynamicProperty()
        {

        }

        /// <summary>
        /// Initializes a new <see cref="DynamicProperty"/>
        /// </summary>
        /// <param name="order">The <see cref="DynamicProperty"/>'s order</param>
        /// <param name="name">The <see cref="DynamicProperty"/>'s name</param>
        /// <param name="value">The <see cref="DynamicProperty"/>'s value</param>
        public DynamicProperty(int order, string name, object? value)
        {
            this.Order = order;
            this.Name = name;
            this.SetValue(value);
        }

        /// <summary>
        /// Gets the <see cref="ProtoField"/>'s order
        /// </summary>
        [DataMember(Order = 1)]
        [ProtoMember(1)]
        public virtual int Order { get; protected set; }

        /// <summary>
        /// Gets the <see cref="ProtoField"/>'s name
        /// </summary>
        [DataMember(Order = 2)]
        [ProtoMember(2)]
        public virtual string Name { get; protected set; } = null!;

        /// <summary>
        /// Gets the <see cref="ProtoField"/>'s type
        /// </summary>
        [DataMember(Order = 3)]
        [ProtoMember(3)]
        public virtual DynamicType Type { get; set; }

        /// <summary>
        /// Gets the <see cref="ProtoField"/>'s value
        /// </summary>
        [DataMember(Order = 4)]
        public virtual Dynamic? Value { get; set; }

        /// <summary>
        /// Sets the <see cref="ProtoField"/>'s value
        /// </summary>
        /// <param name="value">The value to set</param>
        public virtual void SetValue(object? value)
        {
            try
            {
                if (value == null)
                {
                    this.Type = DynamicType.Null;
                    this.Value = null!;
                }
                else
                {
                    this.Type = DynamicHelper.GetDynamicType(value?.GetType());
                    this.Value = Dynamic.FromObject(value!);
                }
            }
            catch (Exception ex)
            {
                throw new SerializationException($"An error occured while serializing the value of property with name '{this.Name}':{Environment.NewLine}{ex}");
            }
        }

        /// <summary>
        /// Gets the <see cref="ProtoField"/>'s value
        /// </summary>
        /// <returns>The <see cref="ProtoField"/>'s value</returns>
        public virtual object? GetValue()
        {
            try
            {
                return this.Value?.ToObject();
            }
            catch (Exception ex)
            {
                throw new SerializationException($"An error occured while deserializing the value of property with name '{this.Name}':{Environment.NewLine}{ex}");
            }
        }

        /// <summary>
        /// Gets the <see cref="ProtoField"/>'s value
        /// </summary>
        /// <param name="type">The expected type of value</param>
        /// <returns>The <see cref="ProtoField"/>'s value</returns>
        public virtual object? GetValue(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            try
            {
                return this.Value?.ToObject(type);
            }
            catch (Exception ex)
            {
                throw new SerializationException($"An error occured while deserializing the value of property with name '{this.Name}':{Environment.NewLine}{ex}");
            }
     
        }

        /// <summary>
        /// Gets the <see cref="ProtoField"/>'s value
        /// </summary>
        /// <typeparam name="T">The expected type of value</typeparam>
        /// <returns>The <see cref="ProtoField"/>'s value</returns>
        public virtual T? GetValue<T>()
        {
            try
            {
                if (this.Value == null)
                    return default;
                else
                    return this.Value.ToObject<T>();
            }
            catch (Exception ex)
            {
                throw new SerializationException($"An error occured while deserializing the value of property with name '{this.Name}':{Environment.NewLine}{ex}");
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.Name;
        }

    }

}