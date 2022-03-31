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
using System.Reflection;

namespace System.Text.Json.Serialization
{

    /// <summary>
    /// Represents the <see cref="JsonConverter"/> used to convert to/from an abstract class
    /// </summary>
    /// <typeparam name="T">The type of the abstract class to convert to/from</typeparam>
    public class AbstractClassConverter<T>
        : JsonConverter<T>
    {

        /// <summary>
        /// Initializes a new <see cref="AbstractClassConverter{T}"/>
        /// </summary>
        /// <param name="jsonSerializerOptions">The current <see cref="JsonSerializerOptions"/></param>
        public AbstractClassConverter(JsonSerializerOptions jsonSerializerOptions)
        {
            this.JsonSerializerOptions = jsonSerializerOptions;
        }

        /// <summary>
        /// Gets the current <see cref="JsonSerializerOptions"/>
        /// </summary>
        protected JsonSerializerOptions JsonSerializerOptions { get; }

        /// <inheritdoc/>
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException("Start object token type expected");
            using JsonDocument jsonDocument = JsonDocument.ParseValue(ref reader);
            var discriminatorProperty = TypeDiscriminator.GetDiscriminatorProperty<T>();
            var discriminatorPropertyName = this.JsonSerializerOptions?.PropertyNamingPolicy == null ? discriminatorProperty.Name : this.JsonSerializerOptions.PropertyNamingPolicy.ConvertName(discriminatorProperty.Name);
            if (!jsonDocument.RootElement.TryGetProperty(discriminatorPropertyName, out var discriminatorJsonValue))
                throw new JsonException($"Failed to find the required '{discriminatorProperty.Name}' discriminator property");
            var discriminatorValue = discriminatorJsonValue.GetString();
            var derivedType = TypeDiscriminator.Discriminate<T>(discriminatorValue);
            string json = jsonDocument.RootElement.GetRawText();
            return (T)JsonSerializer.Deserialize(json, derivedType, this.JsonSerializerOptions);
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (object)value, options);
        }

    }

}