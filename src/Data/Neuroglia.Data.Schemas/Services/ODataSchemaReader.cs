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

using Microsoft.Data.OData;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Mime;

namespace Neuroglia.Data.Services
{
    /// <summary>
    /// Represents the default implementation of the <see cref="ISchemaReader"/> interface used to read ODATA schemas
    /// </summary>
    public class ODataSchemaReader
        : ISchemaReader
    {

        /// <summary>
        /// Initializes a new <see cref="ODataSchemaReader"/>
        /// </summary>
        /// <param name="logger">The service used to perform logging</param>
        /// <param name="httpClient">The service used to perform <see cref="HttpRequestMessage"/>s</param>
        public ODataSchemaReader(ILogger<OpenApiSchemaReader> logger, HttpClient httpClient)
        {
            this.Logger = logger;
            this.HttpClient = httpClient;
        }

        /// <inheritdoc/>
        public SchemaType SchemaType => SchemaType.OData;

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
            IDictionary<string, string> headers = new Dictionary<string, string>();
            if (documentUri.IsFile)
            {
                stream = File.OpenRead(documentUri.LocalPath);
            }
            else
            {
                using HttpRequestMessage request = new(HttpMethod.Get, documentUri);
                request.Headers.Accept.Add(new(MediaTypeNames.Application.Xml));
                using HttpResponseMessage response = await this.HttpClient.SendAsync(request, cancellationToken);
                var responseContent = response.Content == null ? null : await response.Content.ReadAsStringAsync(cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    this.Logger.LogInformation("Failed to retrieve the ODATA document at location '{uri}'. The remote server responded with a non-success status code '{statusCode}'.", documentUri, response.StatusCode);
                    this.Logger.LogDebug("Response content:\n{responseContent}", responseContent ?? "None");
                    response.EnsureSuccessStatusCode();
                }
                var responseStream = await response.Content!.ReadAsStreamAsync(cancellationToken)!;
                stream = new MemoryStream();
                await responseStream.CopyToAsync(stream, cancellationToken);
                stream.Position = 0;
                headers = response.Headers.ToDictionary(kvp => kvp.Key, kvp => string.Join(',', kvp.Value));
            }
            using var odataResponse = ODataResponseMessage.ReadFrom(stream, headers);
            using var odataReader = new ODataMessageReader(odataResponse);
            var document = (await odataReader.ReadServiceDocumentAsync())!;
            return new SchemaDescriptor<ODataWorkspace>(this.SchemaType, documentUri.ToString(), string.Empty, documentUri, document);
        }

        /// <summary>
        /// Creates a new <see cref="ODataSchemaReader"/>
        /// </summary>
        /// <returns>A new <see cref="ODataSchemaReader"/></returns>
        public static ODataSchemaReader Create()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddHttpClient();
            services.AddSingleton<ODataSchemaReader>();
            return services.BuildServiceProvider().GetRequiredService<ODataSchemaReader>();
        }

    }

}
