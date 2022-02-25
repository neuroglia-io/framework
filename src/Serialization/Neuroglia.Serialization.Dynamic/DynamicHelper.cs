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
using ProtoBuf.WellKnownTypes;
using System.Collections;
using System.Dynamic;

namespace Neuroglia.Serialization
{
    
    /// <summary>
    /// Defines helper methods for handling dynamic values
    /// </summary>
    public static class DynamicHelper
    {

        /// <summary>
        /// Deserializes the specified ProtoBuf input
        /// </summary>
        /// <param name="input">The ProtoBuf bytes to deserialize</param>
        /// <param name="type">The type to deserialize to</param>
        /// <returns>The deserialized value</returns>
        public static object? DeserializeProtoBuf(byte[] input, DynamicType type)
        {
            if (type == DynamicType.Null)
                return null;
            return ProtobufHelper.Deserialize(input, GetClrType(type));
        }

        /// <summary>
        /// Gets the dynamic type of the specified CLR type
        /// </summary>
        /// <param name="type">The CLR type to get the dynamic type of</param>
        /// <returns>The dynamic type of the specified CLR type</returns>
        public static DynamicType GetDynamicType(Type? type)
        {
            if (type == null)
                return DynamicType.Null;
            var typeToCheck = type;
            if (typeToCheck.IsNullable())
                typeToCheck = typeToCheck.GetNullableType();
            if (typeToCheck == typeof(char)
                || typeToCheck == typeof(string))
                return DynamicType.String;
            if (typeToCheck == typeof(bool))
                return DynamicType.Boolean;
            if (typeToCheck == typeof(byte)
                || typeToCheck == typeof(byte)
                || typeToCheck == typeof(sbyte)
                || typeToCheck == typeof(uint)
                || typeToCheck == typeof(int)
                || typeToCheck == typeof(long)
                || typeToCheck == typeof(ulong))
                return DynamicType.Integer;
            if (typeToCheck == typeof(float)
                || typeToCheck == typeof(double)
                || typeToCheck == typeof(decimal))
                return DynamicType.Number;
            if (typeToCheck == typeof(DateTime)
                || typeToCheck == typeof(DateTimeOffset)
                || typeToCheck == typeof(Timestamp))
                return DynamicType.Timestamp;
            if (typeToCheck == typeof(TimeSpan)
                || typeToCheck == typeof(Duration))
                return DynamicType.Duration;
            if (typeToCheck == typeof(Guid))
                return DynamicType.String;
            if (typeToCheck == typeof(ExpandoObject)
                || typeToCheck == typeof(IDictionary<string, object>))
                return DynamicType.Object;
            if (typeToCheck.IsEnumerable())
                return DynamicType.Array;
            return DynamicType.Object;
        }

        /// <summary>
        /// Gets the CLR type of the specified dynamic type
        /// </summary>
        /// <param name="type">The dynamic type to get the CLR type of</param>
        /// <returns>The CLR type of the specified dynamic type</returns>
        public static Type? GetClrType(DynamicType type)
        {
            return type switch
            {
                DynamicType.Null => null,
                DynamicType.String => typeof(string),
                DynamicType.Boolean => typeof(bool),
                DynamicType.Timestamp => typeof(Timestamp),
                DynamicType.Duration => typeof(Duration),
                DynamicType.Integer => typeof(int),
                DynamicType.Number => typeof(double),
                DynamicType.Array => typeof(DynamicArray),
                DynamicType.Object => typeof(DynamicObject),
                _ => throw new NotSupportedException($"The specified {nameof(DynamicType)} '{type}' is not supported")
            };
        }

        /// <summary>
        /// Converts the specified value into a dynamic value
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns>The converted value</returns>
        public static object ConvertToDynamicValue(object value)
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
                    return new DynamicObject(properties);
                default:
                    if (value is IEnumerable)
                        return DynamicArray.FromObject(value)!;
                    else
                        return DynamicObject.FromObject(value)!;
            }
        }

    }

}