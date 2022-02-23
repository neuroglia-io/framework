using ProtoBuf;
using ProtoBuf.WellKnownTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Neuroglia.Serialization
{

    /// <summary>
    /// Represents a Protobuf array
    /// </summary>
    [ProtoContract]
    public class ProtoArray
        : ProtoToken
    {

        /// <summary>
        /// Gets the type of the <see cref="ProtoArray"/>'s elements
        /// </summary>
        [ProtoMember(1)]
        public virtual ProtoType ElementType { get; protected set; }

        /// <summary>
        /// Gets the <see cref="ProtoArray"/>'s serialized value
        /// </summary>
        [ProtoMember(2)]
        protected virtual byte[] Bytes { get; set; }

        /// <summary>
        /// Creates a new <see cref="ProtoArray"/> from the specified object
        /// </summary>
        /// <param name="value">The object to create a new <see cref="ProtoArray"/> for</param>
        /// <returns>A new <see cref="ProtoArray"/></returns>
        public static new ProtoArray FromObject(object value)
        {
            if (value == null)
                return null;
            if (value is not IEnumerable enumerable)
                throw new ArgumentException("The specified value is not enumerable", nameof(value));
            var tokens = new List<object>();
            foreach(var elem in enumerable)
            {
                tokens.Add(ProtobufHelper.ConvertToProtoValue(elem));
            }
            var tokenType = tokens.FirstOrDefault()?.GetType();
            if (tokenType == null)
                tokenType = typeof(ProtoObject);
            var count = tokens.Count;
            var toserialize = tokens.OfType(tokenType);
            return new()
            {
                ElementType = ProtobufHelper.GetProtoType(tokenType),
                Bytes = count < 1 ? Array.Empty<byte>() : ProtobufHelper.Serialize(toserialize)
            };
        }

        /// <inheritdoc/>
        public override object ToObject()
        {
            var elementType = ProtobufHelper.GetRuntimeType(this.ElementType);
            var enumerableType = typeof(List<>).MakeGenericType(elementType);
            var enumerable = (IEnumerable)ProtobufHelper.Deserialize(this.Bytes, enumerableType);
            var results = new List<object>();
            if(enumerable != null)
            {
                foreach (var elem in enumerable)
                {
                    results.Add(elem switch
                    {
                        Timestamp timestamp => timestamp.AsDateTime(),
                        Duration duration => duration.AsTimeSpan(),
                        ProtoArray array => array.ToObject(),
                        ProtoObject obj => obj.ToObject(),
                        _ => elem
                    });
                }
            }
            return results;
        }

        /// <inheritdoc/>
        public override object ToObject(Type expectedType)
        {
            if (expectedType == null)
                throw new ArgumentNullException(nameof(expectedType));
            if (!expectedType.IsEnumerable())
                throw new ArgumentException($"The specified type '{expectedType.Name}' is not an {nameof(IEnumerable)} implementation", nameof(expectedType));
            var elementType = ProtobufHelper.GetRuntimeType(this.ElementType);
            var enumerableType = expectedType;
            if (!enumerableType.IsClass
                || enumerableType.IsInterface
                || enumerableType.IsAbstract)
                enumerableType = typeof(List<>).MakeGenericType(expectedType.GetEnumerableElementType());
            var results = new List<object>();
            if(this.Bytes != null)
            {
                var enumerable = (IEnumerable)ProtobufHelper.Deserialize(this.Bytes, enumerableType);
                foreach (var elem in enumerable)
                {
                    results.Add(elem switch
                    {
                        Timestamp timestamp => timestamp.AsDateTime(),
                        Duration duration => duration.AsTimeSpan(),
                        ProtoArray array => array.ToObject(),
                        ProtoObject obj => obj.ToObject(),
                        _ => elem
                    });
                }
            }
            if (enumerableType.IsArray)
            {
                return results.OfType(elementType).ToArray();
            }
            else
            {
                var collection = (ICollection)Activator.CreateInstance(enumerableType);
                collection.AddRange(results);
                return collection;
            }   
        }

        /// <inheritdoc/>
        public override T ToObject<T>()
        {
            return (T)this.ToObject(typeof(T));
        }

    }

}
