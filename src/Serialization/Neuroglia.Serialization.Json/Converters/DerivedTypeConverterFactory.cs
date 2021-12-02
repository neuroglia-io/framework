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
using System.Collections.Concurrent;

namespace System.Text.Json.Serialization
{

    /// <summary>
    /// Represents a <see cref="JsonConverterFactory"/> used to create and manage <see cref="DerivedTypeConverter{T}"/> instances
    /// </summary>
    public class DerivedTypeConverterFactory
        : JsonConverterFactory
    {

        /// <summary>
        /// Gets a <see cref="ConcurrentDictionary{TKey, TValue}"/> containing all registered <see cref="DerivedTypeConverter{T}"/> instances
        /// </summary>
        protected static ConcurrentDictionary<Type, JsonConverter> Converters { get; } = new();

        /// <inheritdoc/>
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.BaseType != null;
        }

        /// <inheritdoc/>
        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            if (!Converters.TryGetValue(typeToConvert, out JsonConverter converter))
            {
                converter = (JsonConverter)Activator.CreateInstance(typeof(DerivedTypeConverter<>).MakeGenericType(typeToConvert));
                Converters.TryAdd(typeToConvert, converter);
            }
            return converter;
        }
    }

}
