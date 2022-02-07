using ProtoBuf;
using ProtoBuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.Serialization;

namespace Neuroglia.Serialization
{

    /// <summary>
    /// Represents a Protobuf object
    /// </summary>
    [ProtoContract]
    public class ProtoObject
        : ProtoToken
    {

        /// <summary>
        /// Gets a <see cref="List{T}"/> containing the <see cref="ProtoObject"/>'s fields
        /// </summary>
        [ProtoMember(1)]
        protected List<ProtoField> Fields { get; set; } = new();

        /// <summary>
        /// Sets the field with the specified name
        /// </summary>
        /// <param name="name">The name of the field to set</param>
        /// <param name="value">The value to set</param>
        public virtual void Set(string name, object value)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            var field = this.Fields.FirstOrDefault(f => f.Name == name);
            if (field == null)
                this.Fields.Add(new(this.Fields.Count + 1, name, value));
            else
                field.SetValue(value);
        }

        /// <summary>
        /// Gets the value of the field with the specified name
        /// </summary>
        /// <param name="name">The name of the field to get the value of</param>
        /// <returns>The value of the field with the specified name</returns>
        public virtual object Get(string name)
        {
            if(string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            var field = this.Fields.FirstOrDefault(f => f.Name == name);
            if (field == null)
                throw new MissingMemberException($"Failed to find the field with the specified name '{name}'");
            return field.GetValue();
        }

        /// <summary>
        /// Gets the value of the field with the specified name
        /// </summary>
        /// <param name="name">The name of the field to get the value of</param>
        /// <param name="expectedType">The expected type of the field's value</param>
        /// <returns>The value of the field with the specified name</returns>
        public virtual object Get(string name, Type expectedType)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            var field = this.Fields.FirstOrDefault(f => f.Name == name);
            if (field == null)
                throw new MissingMemberException($"Failed to find the field with the specified name '{name}'");
            return field.GetValue(expectedType);
        }

        /// <summary>
        /// Gets the value of the field with the specified name
        /// </summary>
        /// <param name="name">The name of the field to get the value of</param>
        /// <typeparam name="T">The expected type of the field's value</typeparam>
        /// <returns>The value of the field with the specified name</returns>
        public virtual T Get<T>(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            var field = this.Fields.FirstOrDefault(f => f.Name == name);
            if (field == null)
                throw new MissingMemberException($"Failed to find the field with the specified name '{name}'");
            return field.GetValue<T>();
        }

        /// <inheritdoc/>
        public override object ToObject()
        {
            var expando = new ExpandoObject();
            foreach(var field in this.Fields)
            {
                var value = field.GetValue();
                value = value switch
                {
                    ProtoArray array => array.ToObject(),
                    ProtoObject obj => obj.ToObject(),
                    _ => value
                };
                ((IDictionary<string, object>)expando).Add(field.Name, value);
            }
            return expando;
        }

        /// <inheritdoc/>
        public override object ToObject(Type expectedType)
        {
            if (expectedType == null)
                throw new ArgumentNullException(nameof(expectedType));
            var ignoreIfNotDecorated = false;
            if (expectedType.TryGetCustomAttribute<DataContractAttribute>(out _)
                || expectedType.TryGetCustomAttribute<ProtoContractAttribute>(out _))
                ignoreIfNotDecorated = true;
            var result = Activator.CreateInstance(expectedType, true);
            foreach (var property in expectedType.GetProperties()
                .Where(p => p.CanRead && p.CanWrite)
                .Where(p => ignoreIfNotDecorated ? p.TryGetCustomAttribute<DataMemberAttribute>(out _) || p.TryGetCustomAttribute<ProtoMemberAttribute>(out _) : true))
            {
                var field = this.Fields.FirstOrDefault(f => f.Name == property.Name);
                if (field == null)
                    continue;
                var value = field.GetValue(property.PropertyType);
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
            return (T)this.ToObject(typeof(T));
        }

        /// <summary>
        /// Creates a new <see cref="ProtoObject"/> from the specified object
        /// </summary>
        /// <param name="source">The object to create a new <see cref="ProtoObject"/> for</param>
        /// <returns>A new <see cref="ProtoObject"/></returns>
        public static new ProtoObject FromObject(object source)
        {
            if (source == null)
                return null;
            var ignoreIfNotDecorated = false;
            if (source.GetType().TryGetCustomAttribute<DataContractAttribute>(out _)
                || source.GetType().TryGetCustomAttribute<ProtoContractAttribute>(out _))
                ignoreIfNotDecorated = true;
            var proto = new ProtoObject();
            foreach (var property in source.GetType()
                .GetProperties()
                .Where(p => p.CanRead && p.GetGetMethod(true) != null)
                .Where(p => ignoreIfNotDecorated ? p.TryGetCustomAttribute<DataMemberAttribute>(out _) || p.TryGetCustomAttribute<ProtoMemberAttribute>(out _) : true))
            {
                proto.Set(property.Name, property.GetValue(source));
            }
            return proto;
        }

    }

}
