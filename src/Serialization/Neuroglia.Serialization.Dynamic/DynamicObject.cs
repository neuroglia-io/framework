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
            var concreteType = type;
            var kvpType = concreteType.GetGenericType(typeof(KeyValuePair<,>));
            if (kvpType != null)
            {
                var keyType = kvpType.GetGenericArguments()[0];
                var valueType = kvpType.GetGenericArguments()[1];
                var key = this.Get(nameof(KeyValuePair<string, string>.Key), keyType)!;
                var value = this.Get(nameof(KeyValuePair<string, string>.Value), valueType)!;
                return Activator.CreateInstance(concreteType, key, value);
            }
                
            if (concreteType == typeof(Dynamic) || concreteType == typeof(DynamicObject))
                return this;
            if (concreteType.IsInterface 
                || concreteType.IsAbstract)
            {
                var discriminatorProperty = TypeDiscriminator.GetDiscriminatorProperty(concreteType);
                var dynamicProperty = this.Properties.FirstOrDefault(p => p.Name.Equals(discriminatorProperty.Name, StringComparison.OrdinalIgnoreCase));
                if (dynamicProperty == null)
                    throw new MissingMemberException($"Failed to find the discriminator property '{discriminatorProperty.Name}'");
                var discriminatorValue = dynamicProperty.GetValue(discriminatorProperty.PropertyType);
                var discriminatorValueStr = discriminatorValue!.ToString();
                if (discriminatorValue.GetType().IsEnum)
                    discriminatorValueStr = EnumHelper.Stringify((Enum)discriminatorValue, discriminatorValue.GetType());
                concreteType = TypeDiscriminator.Discriminate(concreteType, discriminatorValueStr);
            }
            var ignoreIfNotDecorated = false;
            if (concreteType.TryGetCustomAttribute<DataContractAttribute>(out _)
                || concreteType.TryGetCustomAttribute<ProtoContractAttribute>(out _))
                ignoreIfNotDecorated = true;
            var result = Activator.CreateInstance(concreteType, true);
            foreach (var property in concreteType.GetProperties()
                .Where(p => p.CanRead && p.CanWrite)
                .Where(p => ignoreIfNotDecorated ? p.TryGetCustomAttribute<DataMemberAttribute>(out _) || p.TryGetCustomAttribute<ProtoMemberAttribute>(out _) : true))
            {
                var propertyName = property.Name;
                if (property.TryGetCustomAttribute<DataMemberAttribute>(out var dataMemberAttribute)
                    && !string.IsNullOrWhiteSpace(dataMemberAttribute.Name))
                    propertyName = dataMemberAttribute.Name;
                var dynProperty = this.Properties.FirstOrDefault(p => p.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
                if (dynProperty == null)
                    continue;
                var value = dynProperty.GetValue(property.PropertyType);
                if (value == null
                    || value is Empty)
                    continue;
                if (value is string str && property.PropertyType == typeof(Uri))
                {
                    value = new Uri(str);
                }
                else if(value is int enumValue)
                {
                    var enumType = property.PropertyType.IsNullable() ? property.PropertyType.GetNullableType() : property.PropertyType;
                    if (enumType.IsEnum)
                        value = Enum.Parse(enumType, enumValue.ToString());
                }
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
                var propertyName = property.Name;
                if (property.TryGetCustomAttribute<DataMemberAttribute>(out var dataMemberAttribute)
                    && !string.IsNullOrWhiteSpace(dataMemberAttribute.Name))
                    propertyName = dataMemberAttribute.Name;
                dyn.Set(propertyName, property.GetValue(value));
            }
            return dyn;
        }

    }

}