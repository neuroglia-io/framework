using System;
using System.Collections;

namespace Neuroglia.Serialization
{
    /// <summary>
    /// Represents a Protobuf token
    /// </summary>
    public abstract class ProtoToken
    {

        /// <summary>
        /// Converts the <see cref="ProtoToken"/> into a new object
        /// </summary>
        /// <returns>A new object</returns>
        public abstract object ToObject();

        /// <summary>
        /// Converts the <see cref="ProtoToken"/> into a new object
        /// </summary>
        /// <param name="expectedType">The type to convert the <see cref="ProtoToken"/> to</param>
        /// <returns>A new object</returns>
        public abstract object ToObject(Type expectedType);

        /// <summary>
        /// Converts the <see cref="ProtoToken"/> into a new object
        /// </summary>
        /// <typeparam name="T">The type to convert the <see cref="ProtoToken"/> to</typeparam>
        /// <returns>A new object</returns>
        public abstract T ToObject<T>();

        /// <summary>
        /// Creates a new <see cref="ProtoToken"/> from the specified object
        /// </summary>
        /// <param name="source">The object to create a new <see cref="ProtoToken"/> for</param>
        /// <returns>A new <see cref="ProtoToken"/></returns>
        public static ProtoToken FromObject(object source)
        {
            if (source == null)
                return null;
            if (source.GetType().IsPrimitiveType())
            {
                //todo: do something, like throw a meaningful exception
            }
            if (source is IEnumerable)
                return ProtoArray.FromObject(source);
            else
                return ProtoObject.FromObject(source);
        }

    }

}
