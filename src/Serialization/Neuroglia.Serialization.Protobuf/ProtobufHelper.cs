using ProtoBuf;
using System;
using System.IO;

namespace Neuroglia.Serialization
{

    /// <summary>
    /// Defines helper methods for Protobuf
    /// </summary>
    public static class ProtobufHelper
    {

        /// <summary>
        /// Serializes the specified value
        /// </summary>
        /// <param name="value">The value to serialize</param>
        /// <returns>A new byte array that represents the serialized value</returns>
        public static byte[] Serialize(object value)
        {
            if (value == null)
                return null;
            using var stream = new MemoryStream();
            Serializer.Serialize(stream, value);
            return stream.ToArray();
        }

        /// <summary>
        /// Deserializes the specified input
        /// </summary>
        /// <param name="input">The ProtoBuf bytes to deserialize</param>
        /// <param name="type">The type to deserialize to</param>
        /// <returns>The deserialized value</returns>
        public static object Deserialize(byte[] input, Type type)
        {
            if (input == null || input.Length == 0)
                return type.GetDefaultValue();
            using var stream = new MemoryStream(input);
            try
            {
                return Serializer.Deserialize(type, stream);
            }
            catch(Exception ex)
            {
                throw;
            }

        }

    }

}
