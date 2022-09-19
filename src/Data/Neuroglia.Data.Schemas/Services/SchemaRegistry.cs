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
using System.Collections.Concurrent;

namespace Neuroglia.Data.Services
{
    /// <summary>
    /// Represents the default implementation of the <see cref="ISchemaRegistry"/> interface
    /// </summary>
    public class SchemaRegistry
        : ISchemaRegistry
    {

        /// <summary>
        /// Initializes a new <see cref="SchemaRegistry"/>
        /// </summary>
        /// <param name="logger">The service used to perform logging</param>
        /// <param name="documentReaders">An <see cref="IEnumerable{T}"/> containing all registered <see cref="ISchemaReader"/>s</param>
        /// <param name="httpClient">The service used to perform <see cref="HttpRequestMessage"/>s</param>
        public SchemaRegistry(ILogger<SchemaRegistry> logger, IEnumerable<ISchemaReader> documentReaders, HttpClient httpClient)
        {
            this.Logger = logger;
            this.DocumentReaders = documentReaders
                .Select(r => new KeyValuePair<SchemaType, ISchemaReader>(r.SchemaType, r))
                .GroupBy(kvp => kvp.Key)
                .Select(g => g.First())
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            this.HttpClient = httpClient;
        }

        /// <summary>
        /// Gets the service used to perform logging
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the service used to perform <see cref="HttpRequestMessage"/>s
        /// </summary>
        protected HttpClient HttpClient { get; }

        /// <summary>
        /// Gets an <see cref="IDictionary{TKey, TValue}"/> containing the type mappings of all registered <see cref="ISchemaReader"/>s
        /// </summary>
        protected IDictionary<SchemaType, ISchemaReader> DocumentReaders { get; }

        protected ConcurrentDictionary<string, ISchemaDescriptor> Schemas { get; } = new();

        /// <inheritdoc/>
        public virtual async Task<ISchemaDescriptor> GetSchemaAsync(SchemaType schemaType, Uri documentUri, SchemaDocumentPullPolicy pullPolicy = SchemaDocumentPullPolicy.IfNotPresent, CancellationToken cancellationToken = default)
        {
            var descriptor = await this.GetSchemaByDocumentUriAsync(documentUri, cancellationToken);
            if (descriptor != null)
            {
                if (pullPolicy == SchemaDocumentPullPolicy.IfNotPresent)
                    return descriptor;
                else
                    this.Schemas.Remove(descriptor.Id, out _);
            }
            if (!this.DocumentReaders.TryGetValue(schemaType, out var reader))
                throw new NotSupportedException($"No reader supporting schema documents of type '{schemaType}' has been registered");
            descriptor = await reader.ReadFromAsync(documentUri, cancellationToken);
            this.Schemas.TryAdd(descriptor.Id, descriptor);
            return descriptor;
        }

        /// <inheritdoc/>
        public virtual async Task<ISchemaDescriptor?> GetSchemaByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            this.Schemas.TryGetValue(id, out var schema);
            return await Task.FromResult(schema);
        }

        /// <inheritdoc/>
        public virtual async Task<ISchemaDescriptor?> GetSchemaByDocumentUriAsync(Uri documentUri, CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(this.Schemas.Values.ToList().FirstOrDefault(s => s.DocumentUri == documentUri));
        }

        /// <summary>
        /// Creates a new <see cref="SchemaRegistry"/>
        /// </summary>
        /// <returns>A new <see cref="SchemaRegistry"/></returns>
        public static SchemaRegistry Create()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddHttpClient();
            services.AddSchemaRegistry();
            return services.BuildServiceProvider().GetRequiredService<SchemaRegistry>();
        }

    }

}
