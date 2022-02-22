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
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.Serialization
{

    /// <summary>
    /// Represents the service used to serialize and deserialize JSON
    /// </summary>
    public class NewtonsoftJsonSerializer
        : TextSerializerBase, IJsonSerializer
    {

        /// <summary>
        /// Initializes a new <see cref="NewtonsoftJsonSerializer"/>
        /// </summary>
        /// <param name="settings">The service used to access the current <see cref="JsonSerializerSettings"/></param>
        public NewtonsoftJsonSerializer(IOptions<JsonSerializerSettings> settings)
        {
            this.Settings = settings.Value;
            this.Serializer = JsonSerializer.Create(this.Settings);
        }

        /// <inheritdoc/>
        public override IEnumerable<string> SupportedMimeTypes => new string[]
        {
            "application/json",
            "application/x-javascript",
            "text/javascript",
            "text/x-javascript",
            "text/x-json"
        };

        /// <inheritdoc/>
        public override string DefaultMimeType => MediaTypeNames.Application.Json;

        /// <summary>
        /// Gets the current <see cref="JsonSerializerSettings"/>
        /// </summary>
        protected JsonSerializerSettings Settings { get; }

        /// <summary>
        /// Gets the underlying <see cref="JsonSerializer"/> used to serialize and deserialize JSON
        /// </summary>
        protected JsonSerializer Serializer { get; }

        /// <inheritdoc/>
        public override object Deserialize(Stream input, Type returnType)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            if (returnType == null)
                throw new ArgumentNullException(nameof(returnType));
            using StreamReader reader = new(input, null, true, -1, true);
            using JsonTextReader jsonReader = new(reader);
            return this.Serializer.Deserialize(jsonReader, returnType);
        }

        /// <inheritdoc/>
        public override async Task<object> DeserializeAsync(Stream input, Type returnType, CancellationToken cancellationToken = default)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            if (returnType == null)
                throw new ArgumentNullException(nameof(returnType));
            return await Task.Run(() => this.Deserialize(input, returnType), cancellationToken);
        }

        /// <inheritdoc/>
        public override void Serialize(object value, Stream output, Type type)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (output == null)
                throw new ArgumentNullException(nameof(output));
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            using StreamWriter writer = new(output, null, -1, true);
            using JsonTextWriter jsonWriter = new(writer);
            this.Serializer.Serialize(writer, value, type);
        }

        /// <inheritdoc/>
        public override async Task SerializeAsync(object value, Stream output, Type type, CancellationToken cancellationToken = default)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (output == null)
                throw new ArgumentNullException(nameof(output));
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            await Task.Run(() => this.Serialize(value, output, type), cancellationToken);
        }

    }

}
