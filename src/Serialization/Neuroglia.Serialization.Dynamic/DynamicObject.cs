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
        /// Gets the value of the property with the specified name
        /// </summary>
        /// <param name="name">The name of the property to get the value of</param>
        /// <returns>The value of the property with the specified name</returns>
        public virtual object? Get(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            var property = this.Properties.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (property == null)
                throw new MissingMemberException($"Failed to find the property with the specified name '{name}'");
            return property.GetValue();
        }

        /// <summary>
        /// Gets the value of the property with the specified name
        /// </summary>
        /// <param name="name">The name of the property to get the value of</param>
        /// <param name="expectedType">The expected type of the property's value</param>
        /// <returns>The value of the property with the specified name</returns>
        public virtual object? Get(string name, Type expectedType)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            var property = this.Properties.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (property == null)
                throw new MissingMemberException($"Failed to find the property with the specified name '{name}'");
            return property.GetValue(expectedType);
        }

        /// <summary>
        /// Gets the value of the property with the specified name
        /// </summary>
        /// <param name="name">The name of the property to get the value of</param>
        /// <typeparam name="T">The expected type of the property's value</typeparam>
        /// <returns>The value of the property with the specified name</returns>
        public virtual T? Get<T>(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            var property = this.Properties.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (property == null)
                throw new MissingMemberException($"Failed to find the property with the specified name '{name}'");
            return property.GetValue<T>();
        }

        /// <summary>
        /// Attempts to get the value of the specified property
        /// </summary>
        /// <param name="name">The name of the property to get</param>
        /// <param name="value">The value of the property, if any</param>
        /// <returns>A boolean indicating whether or not the specified property is defined</returns>
        public virtual bool TryGet(string name, out object value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            value = null!;
            try
            {
                value = this.Get(name)!;
                return true;
            }
            catch(MissingMemberException)
            {
                return false;
            }
        }

        /// <summary>
        /// Attempts to get the value of the specified property
        /// </summary>
        /// <param name="name">The name of the property to get</param>
        /// <param name="expectedType">The The expected type of value</param>
        /// <param name="value">The value of the property, if any</param>
        /// <returns>A boolean indicating whether or not the specified property is defined</returns>
        public virtual bool TryGet(string name, Type expectedType, out object value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (expectedType == null)
                throw new ArgumentNullException(nameof(name));
            value = null!;
            try
            {
                value = this.Get(name, expectedType)!;
                return true;
            }
            catch(MissingMemberException)
            {
                return false;
            }
        }

        /// <summary>
        /// Attempts to get the value of the specified property
        /// </summary>
        /// <param name="name">The name of the property to get</param>
        /// <param name="value">The value of the property, if any</param>
        /// <typeparam name="T">The expected type of value</typeparam>
        /// <returns>A boolean indicating whether or not the specified property is defined</returns>
        public virtual bool TryGet<T>(string name, out T value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            value = default!;
            try
            {
                value = this.Get<T>(name)!;
                return true;
            }
            catch(MissingMemberException)
            {
                return false;
            }
        }

        /// <summary>
        /// Sets the property with the specified name
        /// </summary>
        /// <param name="name">The name of the property to set</param>
        /// <param name="value">The value to set</param>
        public virtual void Set(string name, object? value)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            var property = this.Properties.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
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
                var dynProperty = this.Properties.FirstOrDefault(p => p.Name.Equals(property.Name, StringComparison.OrdinalIgnoreCase));
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