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
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Neuroglia.Serialization
{

    /// <summary>
    /// Represents a <see cref="IExtensible"/> object used to deserialize and deserialize dynamic types with ProtoBuf
    /// </summary>
    [ProtoContract]
    public class ProtoObject
        : Extensible
    {

        private static readonly MethodInfo ExtensibleAppendFieldMethod = typeof(Extensible).GetMethods().First(m => m.Name == nameof(Extensible.AppendValue) && m.IsStatic && m.IsGenericMethod && m.GetParameters().Length == 3);
        private static readonly MethodInfo ExtensibleTryGetValueMethod = typeof(Extensible).GetMethods().First(m => m.Name == nameof(Extensible.TryGetValue) && m.IsGenericMethod && m.GetParameters().Length == 3);
        private static readonly MethodInfo SerializerChangeTypeMethod = typeof(Serializer).GetMethod(nameof(Serializer.ChangeType));

        /// <summary>
        /// Initializes a new <see cref="ProtoObject"/>
        /// </summary>
        protected ProtoObject()
        {

        }

        private readonly List<ProtoField> _Fields = new();
        /// <summary>
        /// Gets an <see cref="IReadOnlyCollection{T}"/> containing the <see cref="ProtoObject"/>'s fields
        /// </summary>
        protected IReadOnlyCollection<ProtoField> Fields
        {
            get
            {
                return this._Fields.AsReadOnly();
            }
        }

        /// <summary>
        /// Adds a new <see cref="ProtoField"/> to the <see cref="ProtoObject"/>
        /// </summary>
        /// <param name="name">The name of the <see cref="ProtoField"/> to add</param>
        /// <param name="tag">The tag of the <see cref="ProtoField"/> to add</param>
        /// <param name="type">The type of the <see cref="ProtoField"/> to add</param>
        /// <param name="value">The value of the <see cref="ProtoField"/> to add</param>
        public virtual void AddField(string name, int tag, Type type, object value = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            Type protoType = type;
            object protoValue = value;
            if (typeof(object).IsAssignableFrom(type)
                && type != typeof(string)
                && type != typeof(char)
                && !type.IsValueType)
            {
                value = FromObject(value);
                using MemoryStream stream = new();
                Serializer.Serialize(stream, value);
                protoValue = stream.ToArray();
                type = typeof(ProtoObject);
                protoType = typeof(byte[]);
            }
            this._Fields.Add(new ProtoField(name, tag, type, value));
            ExtensibleAppendFieldMethod.MakeGenericMethod(protoType).Invoke(null, new object[] { this, tag, protoValue });
        }

        /// <summary>
        /// Gets the <see cref="ProtoField"/> with the specified name
        /// </summary>
        /// <param name="name">The name of the <see cref="ProtoField"/> to get</param>
        /// <returns>The <see cref="ProtoField"/> with the specified name, if any</returns>
        public virtual ProtoField GetField(string name)
        {
            return this._Fields.FirstOrDefault(f => string.Equals(f.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets the <see cref="ProtoField"/> with the specified tag
        /// </summary>
        /// <param name="tag">The tag of the <see cref="ProtoField"/> to get</param>
        /// <returns>The <see cref="ProtoField"/> with the specified tag, if any</returns>
        public virtual ProtoField GetField(int tag)
        {
            return this._Fields.FirstOrDefault(f => f.Tag == tag);
        }

        /// <summary>
        /// Sets the value of the specified <see cref="ProtoField"/>
        /// </summary>
        /// <param name="name">The name of the <see cref="ProtoField"/> to set</param>
        /// <param name="value">The value to set</param>
        public virtual void SetField(string name, object value)
        {
            ProtoField field = this.Fields.FirstOrDefault(f => string.Equals(f.Name, name, StringComparison.OrdinalIgnoreCase));
            if (field == null)
                throw new MissingMemberException($"Failed to find a field with the specified name '{name}'");
            field.Value = value;
            if (typeof(object).IsAssignableFrom(field.RuntimeType)
                && field.RuntimeType != typeof(string)
                && field.RuntimeType != typeof(char)
                && !field.RuntimeType.IsValueType)
            {
                using MemoryStream stream = new();
                Serializer.Serialize(stream, FromObject(value));
                value = stream.ToArray();
            }
            ExtensibleAppendFieldMethod.MakeGenericMethod(field.RuntimeType).Invoke(null, new object[] { this, field.Tag, value });
        }

        /// <summary>
        /// Sets the value of the specified <see cref="ProtoField"/>
        /// </summary>
        /// <param name="tag">The tag of the <see cref="ProtoField"/> to set</param>
        /// <param name="value">The value to set</param>
        public virtual void SetField(int tag, object value)
        {
            ProtoField field = this.Fields.FirstOrDefault(f => f.Tag == tag);
            if (field == null)
                throw new MissingMemberException($"Failed to find a field with the specified tag '{tag}'");
            field.Value = value;
            if (typeof(object).IsAssignableFrom(field.RuntimeType)
                && field.RuntimeType != typeof(string)
                && field.RuntimeType != typeof(char)
                && !field.RuntimeType.IsValueType)
            {
                using MemoryStream stream = new();
                Serializer.Serialize(stream, FromObject(value));
                value = stream.ToArray();
            }
            ExtensibleAppendFieldMethod.MakeGenericMethod(field.RuntimeType).Invoke(null, new object[] { this, field.Tag, value });
        }

        /// <summary>
        /// Serializes and writes the <see cref="ProtoObject"/> to the specified <see cref="Stream"/>
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to write the <see cref="ProtoObject"/> to</param>
        public virtual void WriteTo(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            Serializer.Serialize(stream, this);
        }

        /// <summary>
        /// Converts the <see cref="ProtoObject"/> to an object of the specified type
        /// </summary>
        /// <param name="type">The type to convert the <see cref="ProtoObject"/> to</param>
        /// <returns>A new object of the specified type</returns>
        public virtual object ToObject(Type type)
        {
            ProtoContractAttribute protoContract = type.GetCustomAttribute<ProtoContractAttribute>();
            if (protoContract != null)
                return SerializerChangeTypeMethod.MakeGenericMethod(typeof(ProtoObject), type).Invoke(null, new object[] { this });
            object instance = Activator.CreateInstance(type, true);
            foreach (ProtoField field in this.Fields)
            {
                PropertyInfo property = type.GetProperty(field.Name, BindingFlags.Default | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (property == null)
                    continue;
                object value = field.Value;
                if (value is ProtoObject proto)
                    value = proto.ToObject(property.PropertyType);
                property.SetValue(instance, value);
            }
            return instance;
        }

        /// <summary>
        /// Converts the <see cref="ProtoObject"/> to an object of the specified type
        /// </summary>
        /// <typeparam name="T">The type to convert the <see cref="ProtoObject"/> to</typeparam>
        /// <returns>A new object of the specified type</returns>
        public virtual T ToObject<T>()
        {
            return (T)this.ToObject(typeof(T));
        }

        /// <summary>
        /// Creates a new <see cref="ProtoObject"/> based on the specified <see cref="DescriptorProto"/>
        /// </summary>
        /// <param name="descriptor">The <see cref="DescriptorProto"/> based on which to create a new <see cref="ProtoObject"/></param>
        /// <param name="fields">An <see cref="IDictionary{TKey, TValue}"/> containing the fields name/value pairs of the <see cref="ProtoObject"/> to create</param>
        /// <returns>A new <see cref="ProtoObject"/></returns>
        public static ProtoObject FromDescriptor(DescriptorProto descriptor, IDictionary<string, object> fields = null)
        {
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));
            if (fields == null)
                fields = new Dictionary<string, object>();
            ProtoObject protoObject = new();
            foreach (FieldDescriptorProto fieldDescriptor in descriptor.Fields)
            {
                if (fields.TryGetValue(fieldDescriptor.Name, out object value))
                    protoObject.AddField(fieldDescriptor.Name, fieldDescriptor.Number, value.GetType(), value);
                else
                    protoObject.AddField(fieldDescriptor.Name, fieldDescriptor.Number, fieldDescriptor.type.ToRuntimeType());
            }
            return protoObject;
        }

        /// <summary>
        /// Creates a new <see cref="ProtoObject"/> based on the specified object
        /// </summary>
        /// <param name="obj">The object based on which to create a new <see cref="ProtoObject"/></param>
        /// <returns>A new <see cref="ProtoObject"/></returns>
        public static ProtoObject FromObject(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            ProtoObject protoObject = new();
            int index = 1;
            bool ignoreNonProtoMembers = obj.GetType().GetCustomAttribute<ProtoContractAttribute>() != null;
            foreach (PropertyInfo property in obj.GetType().GetProperties())
            {
                ProtoMemberAttribute protoMember = property.GetCustomAttribute<ProtoMemberAttribute>();
                int tag = index;
                if (protoMember == null)
                {
                    if (ignoreNonProtoMembers)
                        continue;
                }
                else if (ignoreNonProtoMembers)
                {
                    tag = protoMember.Tag;
                }
                object value = property.GetValue(obj);
                if (typeof(object).IsAssignableFrom(property.PropertyType)
                    && property.PropertyType != typeof(String)
                    && property.PropertyType != typeof(Char)
                    && !property.PropertyType.IsValueType)
                    protoObject.AddField(property.Name, index, typeof(ProtoObject), ProtoObject.FromObject(value));
                else
                    protoObject.AddField(property.Name, index, value.GetType(), value);
                index++;
            }
            return protoObject;
        }

        /// <summary>
        /// Reads a new <see cref="ProtoObject"/> from the specified <see cref="Stream"/>
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to read a <see cref="ProtoObject"/> from</param>
        /// <param name="descriptor">The <see cref="DescriptorProto"/> that describes the type of the expected <see cref="ProtoObject"/></param>
        /// <returns>A new <see cref="ProtoObject"/></returns>
        public static ProtoObject ReadFrom(Stream stream, DescriptorProto descriptor)
        {
            ProtoObject protoObject = Serializer.Deserialize<ProtoObject>(stream);
            foreach (FieldDescriptorProto fieldDescriptor in descriptor.Fields)
            {
                Type fieldType = fieldDescriptor.type.ToRuntimeType();
                MethodInfo tryGetValueMethod = ExtensibleTryGetValueMethod.MakeGenericMethod(fieldType);
                object[] parameters = new object[] { protoObject, fieldDescriptor.Number, null };
                if (!(bool)tryGetValueMethod.Invoke(null, parameters))
                    continue;
                object value = parameters.Last();
                protoObject.AddField(fieldDescriptor.Name, fieldDescriptor.Number, value.GetType(), value);
            }
            return protoObject;
        }

    }

}
