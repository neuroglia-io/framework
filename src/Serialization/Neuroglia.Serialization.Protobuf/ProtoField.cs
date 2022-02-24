using ProtoBuf;
using ProtoBuf.WellKnownTypes;
using System;
using System.Runtime.Serialization;

namespace Neuroglia.Serialization
{
    /// <summary>
    /// Describes a <see cref="ProtoObject"/>'s field
    /// </summary>
    [ProtoContract]
    public class ProtoField
    {

        /// <summary>
        /// Initializes a new <see cref="ProtoField"/>
        /// </summary>
        protected ProtoField()
        {

        }

        /// <summary>
        /// Initializes a new <see cref="ProtoField"/>
        /// </summary>
        /// <param name="tag">The <see cref="ProtoField"/>'s tag</param>
        /// <param name="name">The <see cref="ProtoField"/>'s name</param>
        /// <param name="value">The <see cref="ProtoField"/>'s value</param>
        public ProtoField(int tag, string name, object value)
        {
            this.Tag = tag;
            this.Name = name;
            this.SetValue(value);
        }

        /// <summary>
        /// Gets the <see cref="ProtoField"/>'s tag
        /// </summary>
        [ProtoMember(1)]
        public virtual int Tag { get; protected set; }

        /// <summary>
        /// Gets the <see cref="ProtoField"/>'s name
        /// </summary>
        [ProtoMember(2)]
        public virtual string Name { get; protected set; }

        /// <summary>
        /// Gets the <see cref="ProtoField"/>'s type
        /// </summary>
        [ProtoMember(3)]
        public virtual ProtoType Type { get; set; }

        /// <summary>
        /// Gets the <see cref="ProtoField"/>'s serialized value
        /// </summary>
        [ProtoMember(4)]
        public virtual byte[] Bytes { get; set; }

        /// <summary>
        /// Sets the <see cref="ProtoField"/>'s value
        /// </summary>
        /// <param name="value">The value to set</param>
        public virtual void SetValue(object value)
        {
            try
            {
                this.Type = ProtobufHelper.GetProtoType(value?.GetType());
                this.Bytes = ProtobufHelper.Serialize(ProtobufHelper.ConvertToProtoValue(value));
            }
            catch (Exception ex)
            {
                throw new SerializationException($"An error occured while serializing the value of field with name '{this.Name}':{Environment.NewLine}{ex}");
            }
        }

        /// <summary>
        /// Gets the <see cref="ProtoField"/>'s value
        /// </summary>
        /// <returns>The <see cref="ProtoField"/>'s value</returns>
        public virtual object GetValue()
        {
            try
            {
                var value = ProtobufHelper.Deserialize(this.Bytes, this.Type);
                return value switch
                {
                    Timestamp timestamp => timestamp.AsDateTime(),
                    Duration duration => duration.AsTimeSpan(),
                    ProtoArray array => array.ToObject(),
                    ProtoObject obj => obj.ToObject(),
                    _ => value
                };
            }
            catch(Exception ex)
            {
                throw new SerializationException($"An error occured while deserializing the value of field with name '{this.Name}':{Environment.NewLine}{ex}");
            }
        }

        /// <summary>
        /// Gets the <see cref="ProtoField"/>'s value
        /// </summary>
        /// <param name="type">The expected type of value</param>
        /// <returns>The <see cref="ProtoField"/>'s value</returns>
        public virtual object GetValue(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            try
            {
                var value = ProtobufHelper.Deserialize(this.Bytes, ProtobufHelper.GetRuntimeType(ProtobufHelper.GetProtoType(type)));
                return value switch
                {
                    string str => type == typeof(Guid) ? (object)Guid.Parse(str) : str,
                    Timestamp timestamp => type == typeof(DateTimeOffset) ? (object)new DateTimeOffset(timestamp.AsDateTime()) : timestamp.AsDateTime(),
                    Duration duration => duration.AsTimeSpan(),
                    ProtoArray array => array.ToObject(type),
                    ProtoObject obj => obj.ToObject(type),
                    _ => value
                };
            }
            catch (Exception ex)
            {
                throw new SerializationException($"An error occured while deserializing the value of field with name '{this.Name}':{Environment.NewLine}{ex}");
            }
        }

        /// <summary>
        /// Gets the <see cref="ProtoField"/>'s value
        /// </summary>
        /// <typeparam name="T">The expected type of value</typeparam>
        /// <returns>The <see cref="ProtoField"/>'s value</returns>
        public virtual T GetValue<T>()
        {
            return (T)this.GetValue(typeof(T));
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.Name;
        }

    }

}
