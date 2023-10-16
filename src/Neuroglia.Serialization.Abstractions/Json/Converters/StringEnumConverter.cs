// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License"),
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections.Concurrent;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace Neuroglia.Serialization.Json.Converters;

/// <summary>
/// Represents the <see cref="JsonConverterFactory"/> used to create <see cref="StringEnumConverter{TEnum}"/>
/// </summary>
public class StringEnumConverter
    : JsonConverterFactory
{

    /// <summary>
    /// Gets a <see cref="ConcurrentDictionary{TKey, TValue}"/> containing the mappings of types to their respective <see cref="JsonConverter"/>
    /// </summary>
    private static readonly ConcurrentDictionary<Type, JsonConverter> Converters = new();

    /// <inheritdoc/>
	public override bool CanConvert(Type typeToConvert) => typeToConvert.IsEnum || typeToConvert.IsGenericType && typeToConvert.IsNullable() && typeToConvert.GetGenericArguments().First().IsEnum;

    /// <inheritdoc/>
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var enumType = typeToConvert;
        if (enumType.IsGenericType && enumType.IsNullable() && enumType.GetGenericArguments().First().IsEnum) enumType = enumType.GetGenericArguments().First();
        if (!Converters.TryGetValue(typeToConvert, out var converter) || converter == null)
        {
            var converterType = typeof(StringEnumConverter<>).MakeGenericType(typeToConvert);
            converter = (JsonConverter)Activator.CreateInstance(converterType, enumType)!;
            Converters.TryAdd(typeToConvert, converter);
        }
        return converter;
    }

}

/// <summary>
/// Represents the <see cref="JsonConverter{T}"/> used to convert from and to <see cref="Enum"/>s
/// </summary>
/// <typeparam name="T">The type to convert</typeparam>
public class StringEnumConverter<T>
    : JsonConverter<T>
{

    /// <summary>
    /// Initializes a new <see cref="StringEnumConverter{TEnum}"/>
    /// </summary>
    /// <param name="underlyingType">The underlying <see cref="Enum"/>'s type</param>
    public StringEnumConverter(Type underlyingType)
    {
        UnderlyingType = underlyingType;
        var values = UnderlyingType.GetEnumValues();
        var names = UnderlyingType.GetEnumNames();
        TypeCode = Type.GetTypeCode(UnderlyingType);
        IsFlags = UnderlyingType.IsDefined(typeof(FlagsAttribute), true);
        ValueMappings = new Dictionary<ulong, EnumFieldMetadata>();
        NameMappings = new Dictionary<string, EnumFieldMetadata>();
        for (var index = 0; index < values.Length; index++)
        {
            var value = (Enum)values.GetValue(index)!;
            var rawValue = GetRawValue(value);
            var name = names[index];
            var field = UnderlyingType.GetField(name, BindingFlags.Default | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)!;
            var enumMemberAttribute = field.GetCustomAttribute<EnumMemberAttribute>(true)!;
            var transformedName = enumMemberAttribute?.Value ?? NamingPolicy.Current.ConvertName(name) ?? name;
            var fieldMetadata = new EnumFieldMetadata(name, transformedName, value, rawValue);
            if (!ValueMappings.ContainsKey(rawValue)) ValueMappings.Add(rawValue, fieldMetadata);
            NameMappings.Add(transformedName, fieldMetadata);
        }
    }

    /// <summary>
    /// Gets the underlying <see cref="Enum"/>'s type
    /// </summary>
    protected Type UnderlyingType { get; }

    /// <summary>
    /// Gets the <see cref="Enum"/>'s <see cref="System.TypeCode"/>
    /// </summary>
    protected TypeCode TypeCode { get; }

    /// <summary>
    /// Gets a boolean indicating whether or not the specified <see cref="Enum"/> is flags
    /// </summary>
    protected bool IsFlags { get; }

    /// <summary>
    /// Gets a <see cref="Dictionary{TKey, TValue}"/> containing mappings of raw values to field metadata
    /// </summary>
    protected Dictionary<ulong, EnumFieldMetadata> ValueMappings { get; }

    /// <summary>
    /// Gets a <see cref="Dictionary{TKey, TValue}"/> containing mappings of names to field metadata
    /// </summary>
    protected Dictionary<string, EnumFieldMetadata> NameMappings { get; }

    /// <inheritdoc/>
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                var enumString = reader.GetString()!;
                if (NameMappings.TryGetValue(enumString, out var fieldMetadata)) return (T)Enum.ToObject(UnderlyingType, fieldMetadata.RawValue);
                if (IsFlags)
                {
                    ulong rawValue = 0;
                    var flagValues = enumString.Split(", ");
                    foreach (var flagValue in flagValues)
                    {
                        if (NameMappings.TryGetValue(flagValue, out fieldMetadata)) rawValue |= fieldMetadata.RawValue;
                        else
                        {
                            var matched = false;
                            foreach (var kvp in NameMappings)
                            {
                                if (string.Equals(kvp.Key, flagValue, StringComparison.OrdinalIgnoreCase))
                                {
                                    rawValue |= kvp.Value.RawValue;
                                    matched = true;
                                    break;
                                }
                            }
                            if (!matched) throw new JsonException($"Unknown flag value {flagValue}.");
                        }
                    }
                    return (T)Enum.ToObject(UnderlyingType, rawValue);
                }
                foreach (KeyValuePair<string, EnumFieldMetadata> kvp in NameMappings)
                {
                    if (string.Equals(kvp.Key, enumString, StringComparison.OrdinalIgnoreCase)) return (T)Enum.ToObject(UnderlyingType, kvp.Value.RawValue);
                }
                throw new JsonException($"Unknown {UnderlyingType} value {enumString}");
            case JsonTokenType.Number:
                switch (TypeCode)
                {
                    case TypeCode.Int32:
                        if (reader.TryGetInt32(out int int32)) return (T)Enum.ToObject(UnderlyingType, int32);
                        break;
                    case TypeCode.UInt32:
                        if (reader.TryGetUInt32(out uint uint32)) return (T)Enum.ToObject(UnderlyingType, uint32);
                        break;
                    case TypeCode.UInt64:
                        if (reader.TryGetUInt64(out ulong uint64)) return (T)Enum.ToObject(UnderlyingType, uint64);
                        break;
                    case TypeCode.Int64:
                        if (reader.TryGetInt64(out long int64)) return (T)Enum.ToObject(UnderlyingType, int64);
                        break;
                    case TypeCode.SByte:
                        if (reader.TryGetSByte(out sbyte byte8)) return (T)Enum.ToObject(UnderlyingType, byte8);
                        break;
                    case TypeCode.Byte:
                        if (reader.TryGetByte(out byte ubyte8)) return (T)Enum.ToObject(UnderlyingType, ubyte8);
                        break;
                    case TypeCode.Int16:
                        if (reader.TryGetInt16(out short int16)) return (T)Enum.ToObject(UnderlyingType, int16);
                        break;
                    case TypeCode.UInt16:
                        if (reader.TryGetUInt16(out ushort uint16)) return (T)Enum.ToObject(UnderlyingType, uint16);
                        break;
                }
                throw new JsonException($"Unsupported type code '{TypeCode}'");
            default:
                throw new JsonException($"Unsupported token type '{reader.TokenType}'");
        }
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        var rawValue = GetRawValue(value!);
        if (ValueMappings.TryGetValue(rawValue, out var fieldMetadata))
        {
            writer.WriteStringValue(fieldMetadata.TransformedName);
            return;
        }
        if (IsFlags)
        {
            ulong flagsValue = 0;
            var stringBuilder = new StringBuilder();
            foreach (KeyValuePair<ulong, EnumFieldMetadata> kvp in ValueMappings)
            {
                fieldMetadata = kvp.Value;
                if ((value as Enum)!.HasFlag(fieldMetadata.Value) || fieldMetadata.RawValue == 0) continue;
                flagsValue |= fieldMetadata.RawValue;
                if (stringBuilder.Length > 0) stringBuilder.Append(", ");
                stringBuilder.Append(fieldMetadata.TransformedName);
            }
            if (flagsValue == rawValue)
            {
                writer.WriteStringValue(stringBuilder.ToString());
                return;
            }
        }
        switch (TypeCode)
        {
            case TypeCode.Int32:
                writer.WriteNumberValue((int)rawValue);
                break;
            case TypeCode.UInt32:
                writer.WriteNumberValue((uint)rawValue);
                break;
            case TypeCode.UInt64:
                writer.WriteNumberValue(rawValue);
                break;
            case TypeCode.Int64:
                writer.WriteNumberValue((long)rawValue);
                break;
            case TypeCode.Int16:
                writer.WriteNumberValue((short)rawValue);
                break;
            case TypeCode.UInt16:
                writer.WriteNumberValue((ushort)rawValue);
                break;
            case TypeCode.Byte:
                writer.WriteNumberValue((byte)rawValue);
                break;
            case TypeCode.SByte:
                writer.WriteNumberValue((sbyte)rawValue);
                break;
            default:
                throw new JsonException();
        }
    }

    private ulong GetRawValue(object value)
    {
        return TypeCode switch
        {
            TypeCode.Int32 => (ulong)(int)value,
            TypeCode.UInt32 => (uint)value,
            TypeCode.UInt64 => (ulong)value,
            TypeCode.Int64 => (ulong)(long)value,
            TypeCode.SByte => (ulong)(sbyte)value,
            TypeCode.Byte => (byte)value,
            TypeCode.Int16 => (ulong)(short)value,
            TypeCode.UInt16 => (ushort)value,
            _ => throw new JsonException($"Unsupported type code '{TypeCode}'"),
        };
    }

    /// <summary>
    /// Holds information about an <see cref="Enum"/>'s field
    /// </summary>
    protected class EnumFieldMetadata
    {

        /// <summary>
        /// Initializes a new <see cref="EnumFieldMetadata"/>
        /// </summary>
        /// <param name="name">The <see cref="Enum"/> field's name</param>
        /// <param name="transformedName">The <see cref="Enum"/> field's transformed name</param>
        /// <param name="value">The <see cref="Enum"/> field's value</param>
        /// <param name="rawValue">The <see cref="Enum"/> field's raw value</param>
        public EnumFieldMetadata(string name, string transformedName, Enum value, ulong rawValue)
        {
            Name = name;
            TransformedName = transformedName;
            Value = value;
            RawValue = rawValue;
        }

        /// <summary>
        /// Gets the <see cref="Enum"/>'s field name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the <see cref="Enum"/> field's transformed name
        /// </summary>
        public string TransformedName { get; }

        /// <summary>
        /// Gets the <see cref="Enum"/> field's value
        /// </summary>
        public Enum Value { get; }

        /// <summary>
        /// Gets the <see cref="Enum"/> field's raw value
        /// </summary>
        public ulong RawValue { get; }

    }

}
