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
using Neuroglia;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace System.Text.Json.Serialization
{

    /// <summary>
    /// Represents a <see cref="JsonConverter{T}"/> used to convert derived types
    /// </summary>
    /// <typeparam name="T">The abstract type to convert to a concrete type</typeparam>
    public class DerivedTypeConverter<T>
        : JsonConverter<T>
    {

        /// <summary>
        /// Initializes a new <see cref="DerivedTypeConverter{T}"/>
        /// </summary>
        public DerivedTypeConverter()
        {
            foreach(Type derivedType in TypeCacheUtil.FindFilteredTypes($"{typeof(T).FullName}-derived", t => t.IsClass && !t.IsAbstract && !t.IsInterface && typeof(T).IsAssignableFrom(t)))
            {

                this.MemberMappings.Add(derivedType, derivedType.GetMembers(BindingFlags.Default | BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)
                    .Where(m => (m is PropertyInfo property && property.CanRead) || (m.MemberType == MemberTypes.Field && m.TryGetCustomAttribute<JsonPropertyNameAttribute>(out _)))
                    .Where(m => !m.TryGetCustomAttribute<JsonIgnoreAttribute>(out _))
                    .ToDictionary(m => m.TryGetCustomAttribute(out JsonPropertyNameAttribute propertyNameAttribute) ? propertyNameAttribute.Name : m.Name));
            }
        }

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> containing the <see cref="MemberInfo"/> mappings of the specifed abstract type concretions
        /// </summary>
        protected Dictionary<Type, Dictionary<string, MemberInfo>> MemberMappings { get; } = new();

        /// <inheritdoc/>
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            if (value == null)
                return;
            writer.WriteStartObject();
            if (!this.MemberMappings.TryGetValue(value.GetType(), out var memberMappings))
                throw new SerializationException($"The specified derived type is not supported '{value.GetType().FullName}'");
            foreach (var kvp in memberMappings)
            {
                writer.WritePropertyName(kvp.Key);
                var rawValue = kvp.Value switch
                {
                    FieldInfo field => field.GetValue(value),
                    PropertyInfo property => property.GetValue(value),
                    _ => throw new NotSupportedException($"The specified member type '{kvp.Value.MemberType}' is not supported")
                };
                if (rawValue == null)
                    continue;
                var converter = new JsonSerializerOptions().GetConverter(rawValue.GetType());
                var writeMethod = typeof(JsonConverter<>).MakeGenericType(rawValue.GetType()).GetMethod(nameof(JsonConverter<object>.Write));
                writeMethod.Invoke(converter, new object[] { writer, rawValue, options });
            }
            writer.WriteEndObject();
        }

    }

}
