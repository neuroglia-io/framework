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
using Newtonsoft.Json.Linq;
using System;

namespace Newtonsoft.Json
{

    /// <summary>
    /// Represents a <see cref="JsonConverter"/> used to deserialize implementations of the specified abstract class
    /// </summary>
    /// <typeparam name="T">The type of the abstract class to deserialize</typeparam>
    public class AbstractClassConverter<T>
        : JsonConverter
    {

        /// <inheritdoc/>
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsClass && objectType.TryGetCustomAttribute(out DiscriminatorAttribute _);
        }

        /// <inheritdoc/>
        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (!objectType.TryGetCustomAttribute(out DiscriminatorAttribute discriminatorAttribute))
                throw new NullReferenceException($"Failed to find the required '{nameof(DiscriminatorAttribute)}'");
            if (reader.TokenType == JsonToken.Null)
                return null;
            var jObject = JObject.Load(reader);
            var discriminatorProperty = TypeDiscriminator.GetDiscriminatorProperty<T>();
            var jProperty = jObject.Property(discriminatorProperty.Name, StringComparison.InvariantCultureIgnoreCase);
            object discriminatorValue;
            string discriminatorValueStr;
            if (jProperty == null)
                discriminatorValue = discriminatorProperty.PropertyType.GetDefaultValue();
            else
                discriminatorValue = jProperty.Value.ToObject(discriminatorProperty.PropertyType);
            if (discriminatorValue is Enum enumValue)
                discriminatorValueStr = EnumHelper.Stringify(enumValue, discriminatorProperty.PropertyType);
            else
                discriminatorValueStr = discriminatorValue.ToString();
            var derivedType = TypeDiscriminator.Discriminate<T>(discriminatorValueStr);
            var result = Activator.CreateInstance(derivedType, true);
            serializer.Populate(jObject.CreateReader(), result);
            return result;
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

    }

}
