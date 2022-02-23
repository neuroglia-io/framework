using ProtoBuf;
using ProtoBuf.WellKnownTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
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
            return Serializer.Deserialize(type, stream);
        }

        /// <summary>
        /// Deserializes the specified input
        /// </summary>
        /// <param name="input">The ProtoBuf bytes to deserialize</param>
        /// <param name="type">The type to deserialize to</param>
        /// <returns>The deserialized value</returns>
        public static object Deserialize(byte[] input, ProtoType type)
        {
            if (type == ProtoType.Empty)
                return null;
            return Deserialize(input, GetRuntimeType(type));
        }

        /// <summary>
        /// Gets the Proto type of the specified runtime type
        /// </summary>
        /// <param name="type">The runtime type to get the Proto type of</param>
        /// <returns>The Proto type of the specified runtime type</returns>
        public static ProtoType GetProtoType(Type type)
        {
            if (type == null)
                return ProtoType.Empty;
            var typeToCheck = type;
            if (typeToCheck.IsNullable())
                typeToCheck = typeToCheck.GetNullableType();
            if(typeToCheck == typeof(char)
                || typeToCheck == typeof(string))
                return ProtoType.String;
            if (typeToCheck == typeof(bool))
                return ProtoType.Boolean;
            if (typeToCheck == typeof(byte)
                || typeToCheck == typeof(byte)
                || typeToCheck == typeof(sbyte)
                || typeToCheck == typeof(uint)
                || typeToCheck == typeof(int)
                || typeToCheck == typeof(long)
                || typeToCheck == typeof(ulong))
                return ProtoType.Integer;
            if (typeToCheck == typeof(float)
                || typeToCheck == typeof(double)
                || typeToCheck == typeof(decimal))
                return ProtoType.Double;
            if (typeToCheck == typeof(DateTime)
                || typeToCheck == typeof(DateTimeOffset))
                return ProtoType.Timestamp;
            if (typeToCheck == typeof(TimeSpan))
                return ProtoType.Duration;
            if (typeToCheck == typeof(Guid))
                return ProtoType.String;
            if (typeToCheck == typeof(ExpandoObject)
                || typeToCheck == typeof(IDictionary<string, object>))
                return ProtoType.Object;
            if (typeToCheck.IsEnumerable())
                return ProtoType.Array;
            return ProtoType.Object;
        }

        /// <summary>
        /// Gets the Proto type of the specified Protobuf type
        /// </summary>
        /// <param name="type">The Protobuf type to get the runtime type of</param>
        /// <returns>The runtime type of the specified Protobuf type</returns>
        public static Type GetRuntimeType(ProtoType type)
        {
            return type switch
            {
                ProtoType.Empty => null,
                ProtoType.String => typeof(string),
                ProtoType.Boolean => typeof(bool),
                ProtoType.Timestamp => typeof(Timestamp),
                ProtoType.Duration => typeof(Duration),
                ProtoType.Integer => typeof(int),
                ProtoType.Double => typeof(double),
                ProtoType.Array => typeof(ProtoArray),
                ProtoType.Object => typeof(ProtoObject),
                _ => throw new NotSupportedException($"The specified {nameof(ProtoType)} '{type}' is not supported")
            };
        }

        /// <summary>
        /// Converts the specified value into a Protobuf-compatible value
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns>The converted value</returns>
        public static object ConvertToProtoValue(object value)
        {
            if (value == null)
                return new Empty();
            switch (value)
            {
                case char:
                case string:
                case bool:
                case byte:
                case sbyte:
                case uint:
                case int:
                case long:
                case ulong:
                case float:
                case double:
                case decimal:
                    return value;
                case DateTime dateTime:
                    return new Timestamp(dateTime);
                case DateTimeOffset dateTimeOffset:
                    return new Timestamp(dateTimeOffset.UtcDateTime);
                case TimeSpan timespan:
                    return new Duration(timespan);
                case Guid guid:
                    return guid.ToString();
                case IDictionary<string, object> properties:
                    return new ProtoObject(properties);
                default:
                    if (value is IEnumerable)
                        return ProtoArray.FromObject(value);
                    else
                        return ProtoObject.FromObject(value);
            }
        }

    }

}
