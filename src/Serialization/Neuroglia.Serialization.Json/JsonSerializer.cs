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
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.Serialization
{

    /// <summary>
    /// Represents the service used to serialize and deserialize JSON
    /// </summary>
    public class JsonSerializer
        : TextSerializerBase, IJsonSerializer
    {

        /// <summary>
        /// Initializes a new <see cref="JsonSerializer"/>
        /// </summary>
        /// <param name="options">The <see cref="JsonSerializerOptions"/> used to configure the underlying <see cref="System.Text.Json.JsonSerializer"/></param>
        public JsonSerializer(IOptions<JsonSerializerOptions> options)
        {
            this.Options = options.Value;
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

        /// <summary>
        /// Gets the <see cref="JsonSerializerOptions"/> used to configure the underlying <see cref="System.Text.Json.JsonSerializer"/>
        /// </summary>
        protected JsonSerializerOptions Options { get; }

        /// <inheritdoc/>
        public override string DefaultMimeType => MediaTypeNames.Application.Json;

        /// <inheritdoc/>
        public override string Serialize(object value, Type type)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            return System.Text.Json.JsonSerializer.Serialize(value, type, this.Options);
        }

        /// <inheritdoc/>
        public override async Task<string> SerializeAsync(object value, Type type, CancellationToken cancellationToken = default)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            using MemoryStream stream = new();
            await System.Text.Json.JsonSerializer.SerializeAsync(stream, value, type, this.Options, cancellationToken);
            return Encoding.UTF8.GetString(stream.ToArray());
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
            string json = this.Serialize(value, type);
            using StreamWriter writer = new(output);
            writer.Write(json);
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
            string json = await this.SerializeAsync(value, type, cancellationToken);
            using StreamWriter writer = new(output, null, -1, true);
            await writer.WriteAsync(json);
        }

        /// <inheritdoc/>
        public override object Deserialize(Stream input, Type returnType)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            if (returnType == null)
                throw new ArgumentNullException(nameof(returnType));
            using StreamReader reader = new(input, null, true, -1, true);
            string json = reader.ReadToEnd();
            return this.Deserialize(json, returnType);
        }

        /// <inheritdoc/>
        public override async Task<object> DeserializeAsync(Stream input, Type returnType, CancellationToken cancellationToken = default)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            if (returnType == null)
                throw new ArgumentNullException(nameof(returnType));
            return await System.Text.Json.JsonSerializer.DeserializeAsync(input, returnType, this.Options, cancellationToken);
        }

        /// <inheritdoc/>
        public override object Deserialize(string input, Type returnType)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentNullException(nameof(input));
            if (returnType == null)
                throw new ArgumentNullException(nameof(returnType));
            return System.Text.Json.JsonSerializer.Deserialize(input, returnType, this.Options);
        }

    }

}
