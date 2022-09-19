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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;

namespace Neuroglia.Data.Services
{
    /// <summary>
    /// Represents the default OpenAPI implementation of the <see cref="ISchemaReader"/> interface
    /// </summary>
    public class OpenApiSchemaReader
        : ISchemaReader
    {

        /// <summary>
        /// Initializes a new <see cref="OpenApiSchemaReader"/>
        /// </summary>
        /// <param name="logger">The service used to perform logging</param>
        /// <param name="httpClient">The service used to perform <see cref="HttpRequestMessage"/>s</param>
        public OpenApiSchemaReader(ILogger<OpenApiSchemaReader> logger, HttpClient httpClient)
        {
            this.Logger = logger;
            this.HttpClient = httpClient;
        }

        /// <inheritdoc/>
        public SchemaType SchemaType => SchemaType.OpenApi;

        /// <summary>
        /// Gets the service used to perform logging
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the service used to perform <see cref="HttpRequestMessage"/>s
        /// </summary>
        protected HttpClient HttpClient { get; }

        /// <inheritdoc/>
        public virtual async Task<ISchemaDescriptor> ReadFromAsync(Uri documentUri, CancellationToken cancellationToken = default)
        {
            Stream stream;
            if (documentUri.IsFile)
            {
                stream = File.OpenRead(documentUri.LocalPath);
            }
            else
            {
                using HttpRequestMessage request = new(HttpMethod.Get, documentUri);
                using HttpResponseMessage response = await this.HttpClient.SendAsync(request, cancellationToken);
                var responseContent = response.Content == null ? null : await response.Content.ReadAsStringAsync(cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    this.Logger.LogInformation("Failed to retrieve the Open API document at location '{uri}'. The remote server responded with a non-success status code '{statusCode}'.", documentUri, response.StatusCode);
                    this.Logger.LogDebug("Response content:\n{responseContent}", responseContent ?? "None");
                    response.EnsureSuccessStatusCode();
                }
                var responseStream = await response.Content!.ReadAsStreamAsync(cancellationToken)!;
                stream = new MemoryStream();
                await responseStream.CopyToAsync(stream, cancellationToken);
                stream.Position = 0;
            }
            var document = new OpenApiStreamReader().Read(stream, out var diagnostic);
            if (diagnostic.Errors.Any())
                throw new Exception($"Error(s) occured while reading the OpenAPI document at '{documentUri}':\n{string.Join("\n", diagnostic.Errors.Select(e => $"{e.Pointer}: {e.Message}"))}");
            await stream.DisposeAsync();
            return new SchemaDescriptor<OpenApiDocument>(this.SchemaType, document.Info.Title, document.Info.Version, documentUri, document);
        }

        /// <summary>
        /// Creates a new <see cref="OpenApiSchemaReader"/>
        /// </summary>
        /// <returns>A new <see cref="OpenApiSchemaReader"/></returns>
        public static OpenApiSchemaReader Create()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddHttpClient();
            services.AddSingleton<OpenApiSchemaReader>();
            return services.BuildServiceProvider().GetRequiredService<OpenApiSchemaReader>();
        }

    }

}
