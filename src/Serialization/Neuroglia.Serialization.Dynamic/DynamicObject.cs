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
using System.Dynamic;
using System.Runtime.Serialization;

namespace Neuroglia.Serialization
{
    /// <summary>
    /// Represents a dynamic object
    /// </summary>
    [DataContract]
    [ProtoContract]
    public class DynamicObject
        : Dynamic
    {

        /// <summary>
        /// Initializes a new <see cref="DynamicObject"/>
        /// </summary>
        public DynamicObject()
        {

        }

        /// <summary>
        /// Innitializes a new <see cref="DynamicObject"/>
        /// </summary>
        /// <param name="properties">An <see cref="IDictionary{TKey, TValue}"/> containing the <see cref="DynamicObject"/>'s name/value property mappings</param>
        public DynamicObject(IDictionary<string, object> properties)
        {
            foreach (var property in properties)
            {
                this.Set(property.Key, property.Value);
            }
        }

        /// <summary>
        /// Gets a <see cref="List{T}"/> containing the <see cref="DynamicObject"/>'s fields
        /// </summary>
        [DataMember(Order = 1)]
        [ProtoMember(1)]
        protected List<DynamicProperty> Properties { get; set; } = new();

        /// <summary>
        /// Gets an <see cref="IDictionary{TKey, TValue}"/> containing the <see cref="DynamicObject"/>'s property key/value pairs
        /// </summary>
        [IgnoreDataMember]
        [ProtoIgnore]
        public IDictionary<string, object> DynamicProperties => this.ToObject().ToDictionary();

        /// <summary>
        /// Sets the property with the specified name
        /// </summary>
        /// <param name="name">The name of the property to set</param>
        /// <param name="value">The value to set</param>
        public virtual void Set(string name, object? value)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            var property = this.Properties.FirstOrDefault(f => f.Name == name);
            if (property == null)
                this.Properties.Add(new(this.Properties.Count + 1, name, value));
            else
                property.SetValue(value);
        }

        /// <inheritdoc/>
        public override object? ToObject()
        {
            var expando = new ExpandoObject();
            foreach (var field in this.Properties)
            {
                var value = field.GetValue();
                value = value switch
                {
                    DynamicArray array => array.ToObject(),
                    DynamicObject obj => obj.ToObject(),
                    _ => value
                };
                ((IDictionary<string, object>)expando!).Add(field.Name, value!);
            }
            return expando;
        }

        /// <inheritdoc/>
        public override object? ToObject(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            var ignoreIfNotDecorated = false;
            if (type.TryGetCustomAttribute<DataContractAttribute>(out _)
                || type.TryGetCustomAttribute<ProtoContractAttribute>(out _))
                ignoreIfNotDecorated = true;
            var result = Activator.CreateInstance(type, true);
            foreach (var property in type.GetProperties()
                .Where(p => p.CanRead && p.CanWrite)
                .Where(p => ignoreIfNotDecorated ? p.TryGetCustomAttribute<DataMemberAttribute>(out _) || p.TryGetCustomAttribute<ProtoMemberAttribute>(out _) : true))
            {
                var dynProperty = this.Properties.FirstOrDefault(f => f.Name == property.Name);
                if (dynProperty == null)
                    continue;
                var value = dynProperty.GetValue(property.PropertyType);
                if (value == null
                    || value is Empty)
                    continue;
                property.SetValue(result, value);
            }
            return result;
        }

        /// <inheritdoc/>
        public override T ToObject<T>()
        {
            return (T)this.ToObject(typeof(T))!;
        }

        /// <inheritdoc/>
        public new static DynamicObject? FromObject(object? value)
        {
            if (value == null)
                return null;
            if (value is DynamicObject dyn)
                return dyn;
            if (value is IDictionary<string, object> mappings)
                return new(mappings);
            var ignoreIfNotDecorated = false;
            if (value.GetType().TryGetCustomAttribute<DataContractAttribute>(out _)
                || value.GetType().TryGetCustomAttribute<ProtoContractAttribute>(out _))
                ignoreIfNotDecorated = true;
            dyn = new();
            foreach (var property in value.GetType()
                .GetProperties()
                .Where(p => p.CanRead && p.GetGetMethod(true) != null)
                .Where(p => ignoreIfNotDecorated ? p.TryGetCustomAttribute<DataMemberAttribute>(out _) || p.TryGetCustomAttribute<ProtoMemberAttribute>(out _) : true))
            {
                dyn.Set(property.Name, property.GetValue(value));
            }
            return dyn;
        }

    }

}