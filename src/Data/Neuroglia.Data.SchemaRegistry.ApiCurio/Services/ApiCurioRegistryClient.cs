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

using System.Text;

namespace Neuroglia.Data.SchemaRegistry.Services
{

    /// <summary>
    /// Represents the default implementation of the <see cref="IApiCurioRegistryClient"/> interface
    /// </summary>
    public class ApiCurioRegistryClient
        : IApiCurioRegistryClient
    {

        const string PathPrefix = "/apis/registry/v2";

        /// <summary>
        /// Initializes a new <see cref="ApiCurioRegistryClient"/>
        /// </summary>
        /// <param name="logger">The service used to perform logging</param>
        /// <param name="options">The current <see cref="ApiCurioRegistryClientOptions"/></param>
        /// <param name="httpClientFactory">The service used to create <see cref="System.Net.Http.HttpClient"/>s</param>
        /// <param name="jsonSerializer">The service used to serialize and deserialize JSON</param>
        public ApiCurioRegistryClient(ILogger<ApiCurioRegistryClient> logger, IOptions<ApiCurioRegistryClientOptions> options, IHttpClientFactory httpClientFactory, IJsonSerializer jsonSerializer)
        {
            this.Logger = logger;
            this.Options = options.Value;
            this.HttpClient = httpClientFactory.CreateClient(typeof(ApiCurioRegistryClient).Name);
            this.JsonSerializer = jsonSerializer;
        }

        /// <summary>
        /// Gets the service used to perform logging
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the current <see cref="ApiCurioRegistryClientOptions"/>
        /// </summary>
        protected ApiCurioRegistryClientOptions Options { get; }

        /// <summary>
        /// Gets the <see cref="System.Net.Http.HttpClient"/> used to perform calls to the remote ApiCurio Registry API
        /// </summary>
        protected HttpClient HttpClient { get; }

        /// <summary>
        /// Gets the service used to serialize and deserialize JSON
        /// </summary>
        protected IJsonSerializer JsonSerializer { get; }

        /// <summary>
        /// Formats the specified string
        /// </summary>
        /// <param name="value">The string to format</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The formatted string</returns>
        protected virtual async Task<string> FormatAsync(string value, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));
            var formatted = value;
            return await Task.FromResult(this.Options.LineEndingFormatMode switch
            {
                LineEndingFormatMode.ConvertToUnix => formatted.ReplaceLineEndings("\r\n"),
                LineEndingFormatMode.ConvertToWindows => formatted.ReplaceLineEndings("\n"),
                _ => value,
            });
        }

        #region Artifacts

        /// <inheritdoc/>
        public virtual async Task<string> GetLatestArtifactAsync(string artifactId, string groupId = "default", CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(artifactId))
                throw new ArgumentNullException(nameof(artifactId));
            if (string.IsNullOrWhiteSpace(groupId))
                throw new ArgumentNullException(nameof(groupId));
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{PathPrefix}/groups/{groupId}/artifacts/{artifactId}");
            using var response = await this.HttpClient.SendAsync(request, cancellationToken);
            var content = await response.Content?.ReadAsStringAsync(cancellationToken)!;
            if (!response.IsSuccessStatusCode)
            {
                this.Logger.LogError("An error occured while retrieving the latest version of the artifact with the specified id '{artifactId}': the remote server responded with a non-success status code '{statusCode}'./r/Response content: {json}", artifactId, response.StatusCode, content);
                response.EnsureSuccessStatusCode();
            }
            return content;
        }

        /// <inheritdoc/>
        public virtual async Task<string?> GetArtifactContentByIdAsync(string contentId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(contentId))
                throw new ArgumentNullException(nameof(contentId));
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{PathPrefix}/ids/contentIds/{contentId}/");
            using var response = await this.HttpClient.SendAsync(request, cancellationToken);
            var content = await response.Content?.ReadAsStringAsync(cancellationToken)!;
            if (!response.IsSuccessStatusCode)
            {
                this.Logger.LogError("An error occured while retrieving the artifact with the specified content id '{contentId}': the remote server responded with a non-success status code '{statusCode}'./r/Response content: {json}", contentId, response.StatusCode, content);
                response.EnsureSuccessStatusCode();
            }
            return content;
        }

        /// <inheritdoc/>
        public virtual async Task<string?> GetArtifactContentByGlobalIdAsync(string globalId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(globalId))
                throw new ArgumentNullException(nameof(globalId));
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{PathPrefix}/ids/globalIds/{globalId}/");
            using var response = await this.HttpClient.SendAsync(request, cancellationToken);
            var content = await response.Content?.ReadAsStringAsync(cancellationToken)!;
            if (!response.IsSuccessStatusCode)
            {
                this.Logger.LogError("An error occured while retrieving the artifact with the specified global id '{globalId}': the remote server responded with a non-success status code '{statusCode}'./r/Response content: {json}", globalId, response.StatusCode, content);
                response.EnsureSuccessStatusCode();
            }
            return content;
        }

        /// <inheritdoc/>
        public virtual async Task<string?> GetArtifactContentBySha256HashAsync(string contentHash, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(contentHash))
                throw new ArgumentNullException(nameof(contentHash));
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{PathPrefix}/ids/contentHashes/{contentHash}/");
            using var response = await this.HttpClient.SendAsync(request, cancellationToken);
            var content = await response.Content?.ReadAsStringAsync(cancellationToken)!;
            if (!response.IsSuccessStatusCode)
            {
                this.Logger.LogError("An error occured while retrieving the artifact with the specified content hash '{contentHash}': the remote server responded with a non-success status code '{statusCode}'./r/Response content: {json}", contentHash, response.StatusCode, content);
                response.EnsureSuccessStatusCode();
            }
            return content;
        }

        /// <inheritdoc/>
        public virtual async Task<ArtifactQueryResults> SearchForArtifactsAsync(SearchForArtifactsQuery? query = null, CancellationToken cancellationToken = default)
        {
            var queryParameters = query.ToDictionary();
            string? queryString = null;
            if (queryParameters != null)
                queryString = $"?{string.Join("&", queryParameters.Where(kvp => kvp.Value != null).Select(kvp => $"{kvp.Key}={kvp.Value}"))}"; ;
            var uri = $"{PathPrefix}/search/artifacts" + queryString;
            using var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Accept.Add(new(MediaTypeNames.Application.Json));
            using var response = await this.HttpClient.SendAsync(request, cancellationToken);
            var json = await response.Content?.ReadAsStringAsync(cancellationToken)!;
            if (!response.IsSuccessStatusCode)
            {
                this.Logger.LogError("An error occured while searching for artifacts: the remote server responded with a non-success status code '{statusCode}'./r/Response content: {json}", response.StatusCode, json);
                response.EnsureSuccessStatusCode();
            }
            return await this.JsonSerializer.DeserializeAsync<ArtifactQueryResults>(json, cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async Task<ArtifactQueryResults> SearchForArtifactsByContentAsync(string content, SearchForArtifactsByContentQuery? query = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentNullException(nameof(content));
            var formattedContent = await this.FormatAsync(content, cancellationToken);
            var queryParameters = query.ToDictionary();
            string? queryString = null;
            if (queryParameters != null)
                queryString = $"?{string.Join("&", queryParameters.Where(kvp => kvp.Value != null).Select(kvp => $"{kvp.Key}={kvp.Value}"))}"; ;
            var uri = $"{PathPrefix}/search/artifacts" + queryString;
            using var httpContent = new StringContent(formattedContent, Encoding.UTF8);
            using var request = new HttpRequestMessage(HttpMethod.Post, uri) { Content = httpContent };
            request.Headers.Accept.Add(new(MediaTypeNames.Application.Json));
            using var response = await this.HttpClient.SendAsync(request, cancellationToken);
            var json = await response.Content?.ReadAsStringAsync(cancellationToken)!;
            if (!response.IsSuccessStatusCode)
            {
                this.Logger.LogError("An error occured while searching for artifacts: the remote server responded with a non-success status code '{statusCode}'./r/Response content: {json}", response.StatusCode, json);
                response.EnsureSuccessStatusCode();
            }
            return await this.JsonSerializer.DeserializeAsync<ArtifactQueryResults>(json, cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async Task<ArtifactQueryResults> ListArtifactsInGroupAsync(string groupId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(groupId))
                throw new ArgumentNullException(nameof(groupId));
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{PathPrefix}/groups/{groupId}/artifacts");
            using var response = await this.HttpClient.SendAsync(request, cancellationToken);
            var json = await response.Content?.ReadAsStringAsync(cancellationToken)!;
            if (!response.IsSuccessStatusCode)
            {
                this.Logger.LogError("An error occured while listing artifacts belonging to the group with the specified id '{groupId}': the remote server responded with a non-success status code '{statusCode}'./r/Response content: {json}", groupId, response.StatusCode, json);
                response.EnsureSuccessStatusCode();
            }
            return await this.JsonSerializer.DeserializeAsync<ArtifactQueryResults>(json, cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async Task UpdateArtifactStateAsync(string artifactId, ArtifactState state, string groupId = "default", CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(artifactId))
                throw new ArgumentNullException(nameof(artifactId));
            if (string.IsNullOrWhiteSpace(groupId))
                throw new ArgumentNullException(nameof(groupId));
            var json = await this.JsonSerializer.SerializeAsync(new { state = EnumHelper.Stringify(state) }, cancellationToken);
            using var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
            using var request = new HttpRequestMessage(HttpMethod.Put, $"{PathPrefix}/groups/{groupId}/artifacts/{artifactId}/state") { Content = content };
            using var response = await this.HttpClient.SendAsync(request, cancellationToken);
            json = await response.Content?.ReadAsStringAsync(cancellationToken)!;
            if (!response.IsSuccessStatusCode)
            {
                this.Logger.LogError("An error occured while updating the state of the artifact with the specified id '{artifactId}': the remote server responded with a non-success status code '{statusCode}'./r/Response content: {json}", artifactId, response.StatusCode, content);
                response.EnsureSuccessStatusCode();
            }
        }

        /// <inheritdoc/>
        public virtual async Task<Artifact> UpdateArtifactAsync(string artifactId, string content, string groupId = "default", string? version = null, string? name = null, string? description = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(artifactId))
                throw new ArgumentNullException(nameof(artifactId));
            if (string.IsNullOrWhiteSpace(groupId))
                throw new ArgumentNullException(nameof(groupId));
            var formattedContent = await this.FormatAsync(content, cancellationToken);
            using var httpContent = new StringContent(formattedContent, Encoding.UTF8);
            using var request = new HttpRequestMessage(HttpMethod.Put, $"{PathPrefix}/groups/{groupId}/artifacts/{artifactId}") { Content = httpContent };
            if (!string.IsNullOrWhiteSpace(version))
                request.Headers.TryAddWithoutValidation("X-Registry-Version", version);
            if (!string.IsNullOrWhiteSpace(name))
                request.Headers.TryAddWithoutValidation("X-Registry-Name", name);
            if (!string.IsNullOrWhiteSpace(description))
                request.Headers.TryAddWithoutValidation("X-Registry-Description", description);
            using var response = await this.HttpClient.SendAsync(request, cancellationToken);
            var json = await response.Content?.ReadAsStringAsync(cancellationToken)!;
            if (!response.IsSuccessStatusCode)
            {
                this.Logger.LogError("An error occured while retrieving the artifact with the specified content hash '{contentHash}': the remote server responded with a non-success status code '{statusCode}'./r/Response content: {json}", json, response.StatusCode, content);
                response.EnsureSuccessStatusCode();
            }
            return await this.JsonSerializer.DeserializeAsync<Artifact>(json, cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async Task<Artifact> CreateArtifactAsync(ArtifactType artifactType, string content, IfArtifactExistsAction ifExists, string? artifactId = null, string groupId = "default", bool canonical = false, string? version = null, string? name = null, string? description = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(groupId))
                throw new ArgumentNullException(nameof(groupId));
            var formattedContent = await this.FormatAsync(content, cancellationToken);
            using var httpContent = new StringContent(formattedContent, Encoding.UTF8);
            using var request = new HttpRequestMessage(HttpMethod.Post, $"{PathPrefix}/groups/{groupId}/artifacts?ifExists={EnumHelper.Stringify(ifExists)}&canonical={canonical}") { Content = httpContent };
            request.Headers.TryAddWithoutValidation("X-Registry-ArtifactType", EnumHelper.Stringify(artifactType));
            if (!string.IsNullOrWhiteSpace(artifactId))
                request.Headers.TryAddWithoutValidation("X-Registry-ArtifactId", artifactId);
            if (!string.IsNullOrWhiteSpace(version))
                request.Headers.TryAddWithoutValidation("X-Registry-Version", version);
            if (!string.IsNullOrWhiteSpace(name))
                request.Headers.TryAddWithoutValidation("X-Registry-Name", name);
            if (!string.IsNullOrWhiteSpace(description))
                request.Headers.TryAddWithoutValidation("X-Registry-Description", description);
            request.Headers.TryAddWithoutValidation("X-Registry-Content-Hash", HashHelper.SHA256Hash(formattedContent));
            request.Headers.TryAddWithoutValidation("X-Registry-Content-Algorithm", EnumHelper.Stringify(HashAlgorithm.SHA256));
            using var response = await this.HttpClient.SendAsync(request, cancellationToken);
            var json = await response.Content?.ReadAsStringAsync(cancellationToken)!;
            if (!response.IsSuccessStatusCode)
            {
                this.Logger.LogError("An error occured while creating a new artifact: the remote server responded with a non-success status code '{statusCode}'./r/Response content: {json}", json, response.StatusCode);
                response.EnsureSuccessStatusCode();
            }
            return await this.JsonSerializer.DeserializeAsync<Artifact>(json, cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async Task DeleteArtifactAsync(string artifactId, string groupId = "default", CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(artifactId))
                throw new ArgumentNullException(nameof(artifactId));
            if (string.IsNullOrWhiteSpace(groupId))
                throw new ArgumentNullException(nameof(groupId));
            using var request = new HttpRequestMessage(HttpMethod.Delete, $"{PathPrefix}/groups/{groupId}/artifacts/{artifactId}");
            using var response = await this.HttpClient.SendAsync(request, cancellationToken);
            var content = await response.Content?.ReadAsStringAsync(cancellationToken)!;
            if (!response.IsSuccessStatusCode)
            {
                this.Logger.LogError("An error occured while deleting the artifact with the specified id '{artifactId}': the remote server responded with a non-success status code '{statusCode}'./r/Response content: {content}", artifactId, response.StatusCode, content);
                response.EnsureSuccessStatusCode();
            }
        }

        /// <inheritdoc/>
        public virtual async Task DeleteAllArtifactsInGroupAsync(string groupId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(groupId))
                throw new ArgumentNullException(nameof(groupId));
            using var request = new HttpRequestMessage(HttpMethod.Delete, $"{PathPrefix}/groups/{groupId}/artifacts");
            using var response = await this.HttpClient.SendAsync(request, cancellationToken);
            var content = await response.Content?.ReadAsStringAsync(cancellationToken)!;
            if (!response.IsSuccessStatusCode)
            {
                this.Logger.LogError("An error occured while deleting artifacts belongs to the group with the specified id '{groupId}': the remote server responded with a non-success status code '{statusCode}'./r/Response content: {content}", groupId, response.StatusCode, content);
                response.EnsureSuccessStatusCode();
            }
        }

        #endregion

        #region Metadata

        /// <inheritdoc/>
        public virtual async Task<Artifact> GetArtifactMetadataAsync(string artifactId, string groupId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(artifactId))
                throw new ArgumentNullException(nameof(artifactId));
            if (string.IsNullOrWhiteSpace(groupId))
                throw new ArgumentNullException(nameof(groupId));
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{PathPrefix}/groups/{groupId}/artifacts/{artifactId}/meta");
            using var response = await this.HttpClient.SendAsync(request, cancellationToken);
            var json = await response.Content?.ReadAsStringAsync(cancellationToken)!;
            if (!response.IsSuccessStatusCode)
            {
                this.Logger.LogError("An error occured while retrieving the metadata of the artifact with the specified id '{artifactId}': the remote server responded with a non-success status code '{statusCode}'./r/Response content: {json}", artifactId, response.StatusCode, json);
                response.EnsureSuccessStatusCode();
            }
            return await this.JsonSerializer.DeserializeAsync<Artifact>(json, cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async Task<Artifact> GetArtifactVersionMetadataAsync(string artifactId, string groupId, string version, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(artifactId))
                throw new ArgumentNullException(nameof(artifactId));
            if (string.IsNullOrWhiteSpace(groupId))
                throw new ArgumentNullException(nameof(groupId));
            if (string.IsNullOrWhiteSpace(version))
                throw new ArgumentNullException(nameof(version));
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{PathPrefix}/groups/{groupId}/artifacts/{artifactId}/versions/{version}/meta");
            using var response = await this.HttpClient.SendAsync(request, cancellationToken);
            var json = await response.Content?.ReadAsStringAsync(cancellationToken)!;
            if (!response.IsSuccessStatusCode)
            {
                this.Logger.LogError("An error occured while retrieving the metadata of the artifact with the specified id '{artifactId}' and version '{version}': the remote server responded with a non-success status code '{statusCode}'./r/Response content: {json}", artifactId, version, response.StatusCode, json);
                response.EnsureSuccessStatusCode();
            }
            return await this.JsonSerializer.DeserializeAsync<Artifact>(json, cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async Task<Artifact> GetArtifactVersionMetadataByContentAsync(string artifactId, string groupId, string content, bool? canonical = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(artifactId))
                throw new ArgumentNullException(nameof(artifactId));
            if (string.IsNullOrWhiteSpace(groupId))
                throw new ArgumentNullException(nameof(groupId));
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentNullException(nameof(content));
            var formattedContent = await this.FormatAsync(content, cancellationToken);
            var uri = $"{PathPrefix}/groups/{groupId}/artifacts/{artifactId}/meta";
            if (canonical.HasValue)
                uri += $"?canonical={canonical}";
            using var httpContent = new StringContent(formattedContent, Encoding.UTF8);
            using var request = new HttpRequestMessage(HttpMethod.Post, uri) { Content = httpContent };
            using var response = await this.HttpClient.SendAsync(request, cancellationToken);
            var json = await response.Content?.ReadAsStringAsync(cancellationToken)!;
            if (!response.IsSuccessStatusCode)
            {
                this.Logger.LogError("An error occured while retrieving the metadata of the artifact with the specified id '{artifactId}': the remote server responded with a non-success status code '{statusCode}'./r/Response content: {json}", artifactId, response.StatusCode, json);
                response.EnsureSuccessStatusCode();
            }
            return await this.JsonSerializer.DeserializeAsync<Artifact>(json, cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async Task UpdateArtifactMetadataAsync(string artifactId, string groupId, string? name = null, string? description = null, ICollection<string>? labels = null, IDictionary<string, string>? properties = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(artifactId))
                throw new ArgumentNullException(nameof(artifactId));
            if (string.IsNullOrWhiteSpace(groupId))
                throw new ArgumentNullException(nameof(groupId));
            var command = new UpdateArtifactMetadataCommand()
            {
                Name = name,
                Description = description,
                Labels = labels,
                Properties = properties
            };
            var json = await this.JsonSerializer.SerializeAsync(command, cancellationToken);
            using var httpContent = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
            using var request = new HttpRequestMessage(HttpMethod.Put, $"{PathPrefix}/groups/{groupId}/artifacts/{artifactId}/meta") { Content = httpContent };
            using var response = await this.HttpClient.SendAsync(request, cancellationToken);
            var content = await response.Content?.ReadAsStringAsync(cancellationToken)!;
            if (!response.IsSuccessStatusCode)
            {
                this.Logger.LogError("An error occured while updating the metadata of the artifact with the specified id '{artifactId}': the remote server responded with a non-success status code '{statusCode}'./r/Response content: {json}", artifactId, response.StatusCode, content);
                response.EnsureSuccessStatusCode();
            }
        }

        /// <inheritdoc/>
        public virtual async Task UpdateArtifactVersionMetadataAsync(string artifactId, string groupId, string version, string? name = null, string? description = null, ICollection<string>? labels = null, IDictionary<string, string>? properties = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(artifactId))
                throw new ArgumentNullException(nameof(artifactId));
            if (string.IsNullOrWhiteSpace(groupId))
                throw new ArgumentNullException(nameof(groupId));
            var command = new UpdateArtifactMetadataCommand()
            {
                Name = name,
                Description = description,
                Labels = labels,
                Properties = properties
            };
            var json = await this.JsonSerializer.SerializeAsync(command, cancellationToken);
            using var httpContent = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
            using var request = new HttpRequestMessage(HttpMethod.Put, $"{PathPrefix}/groups/{groupId}/artifacts/{artifactId}/versions/{version}/meta") { Content = httpContent };
            using var response = await this.HttpClient.SendAsync(request, cancellationToken);
            var content = await response.Content?.ReadAsStringAsync(cancellationToken)!;
            if (!response.IsSuccessStatusCode)
            {
                this.Logger.LogError("An error occured while updating the metadata of the artifact with the specified id '{artifactId}' and version '{version}': the remote server responded with a non-success status code '{statusCode}'./r/Response content: {json}", artifactId, version, response.StatusCode, content);
                response.EnsureSuccessStatusCode();
            }
        }

        /// <inheritdoc/>
        public virtual async Task DeleteArtifactVersionMetadataAsync(string artifactId, string groupId, string version, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(artifactId))
                throw new ArgumentNullException(nameof(artifactId));
            if (string.IsNullOrWhiteSpace(groupId))
                throw new ArgumentNullException(nameof(groupId));
            using var request = new HttpRequestMessage(HttpMethod.Delete, $"{PathPrefix}/groups/{groupId}/artifacts/{artifactId}/versions/{version}/meta");
            using var response = await this.HttpClient.SendAsync(request, cancellationToken);
            var content = await response.Content?.ReadAsStringAsync(cancellationToken)!;
            if (!response.IsSuccessStatusCode)
            {
                this.Logger.LogError("An error occured while deleting the artifact with the specified id '{artifactId}' and version '{version}': the remote server responded with a non-success status code '{statusCode}'./r/Response content: {content}", artifactId, version, response.StatusCode, content);
                response.EnsureSuccessStatusCode();
            }
        }

        #endregion

        #region Versions

        /// <inheritdoc/>
        public virtual async Task<string> GetArtifactVersionAsync(string artifactId, string groupId, string version, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(artifactId))
                throw new ArgumentNullException(nameof(artifactId));
            if (string.IsNullOrWhiteSpace(groupId))
                throw new ArgumentNullException(nameof(groupId));
            if (string.IsNullOrWhiteSpace(version))
                throw new ArgumentNullException(nameof(version));
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{PathPrefix}/groups/{groupId}/artifacts/{artifactId}/versions/{version}");
            using var response = await this.HttpClient.SendAsync(request, cancellationToken);
            var content = await response.Content?.ReadAsStringAsync(cancellationToken)!;
            if (!response.IsSuccessStatusCode)
            {
                this.Logger.LogError("An error occured while retrieving the artifact with the specified id '{artifactId}' and '{version}': the remote server responded with a non-success status code '{statusCode}'./r/Response content: {json}", artifactId, version, response.StatusCode, content);
                response.EnsureSuccessStatusCode();
            }
            return content;
        }

        /// <inheritdoc/>
        public virtual async Task<ArtifactQueryResults> ListArtifactVersionsAsync(string artifactId, string groupId, long offset = 0, long limit = 20, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(artifactId))
                throw new ArgumentNullException(nameof(artifactId));
            if (string.IsNullOrWhiteSpace(groupId))
                throw new ArgumentNullException(nameof(groupId));
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{PathPrefix}/groups/{groupId}/artifacts/{artifactId}/versions?offset={offset}&limit={limit}");
            using var response = await this.HttpClient.SendAsync(request, cancellationToken);
            var json = await response.Content?.ReadAsStringAsync(cancellationToken)!;
            if (!response.IsSuccessStatusCode)
            {
                this.Logger.LogError("An error occured while listing artifacts belonging to the group with the specified id '{groupId}': the remote server responded with a non-success status code '{statusCode}'./r/Response content: {json}", groupId, response.StatusCode, json);
                response.EnsureSuccessStatusCode();
            }
            return await this.JsonSerializer.DeserializeAsync<ArtifactQueryResults>(json, cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async Task UpdateArtifactVersionStateAsync(string artifactId, string groupId, string version, ArtifactState state, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(artifactId))
                throw new ArgumentNullException(nameof(artifactId));
            if (string.IsNullOrWhiteSpace(groupId))
                throw new ArgumentNullException(nameof(groupId));
            if (string.IsNullOrWhiteSpace(version))
                throw new ArgumentNullException(nameof(version));
            var json = await this.JsonSerializer.SerializeAsync(new { state = EnumHelper.Stringify(state) }, cancellationToken);
            using var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{PathPrefix}/groups/{groupId}/artifacts/{artifactId}/versions/{version}/state") { Content = content };
            using var response = await this.HttpClient.SendAsync(request, cancellationToken);
            json = await response.Content?.ReadAsStringAsync(cancellationToken)!;
            if (!response.IsSuccessStatusCode)
            {
                this.Logger.LogError("An error occured while updating the state of the artifact with the specified id '{artifactId}': the remote server responded with a non-success status code '{statusCode}'./r/Response content: {json}", artifactId, response.StatusCode, content);
                response.EnsureSuccessStatusCode();
            }
        }

        /// <inheritdoc/>
        public virtual async Task<Artifact> CreateArtifactVersionAsync(string artifactId, string groupId, string content, string? version = null, string? name = null, string? description = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(groupId))
                throw new ArgumentNullException(nameof(groupId));
            var formattedContent = await this.FormatAsync(content, cancellationToken);
            using var httpContent = new StringContent(formattedContent, Encoding.UTF8);
            using var request = new HttpRequestMessage(HttpMethod.Post, $"{PathPrefix}/groups/{groupId}/artifacts/{artifactId}/versions") { Content = httpContent };
            if (!string.IsNullOrWhiteSpace(artifactId))
                request.Headers.TryAddWithoutValidation("X-Registry-ArtifactId", artifactId);
            if (!string.IsNullOrWhiteSpace(version))
                request.Headers.TryAddWithoutValidation("X-Registry-Version", version);
            if (!string.IsNullOrWhiteSpace(name))
                request.Headers.TryAddWithoutValidation("X-Registry-Name", name);
            if (!string.IsNullOrWhiteSpace(description))
                request.Headers.TryAddWithoutValidation("X-Registry-Description", description);
            using var response = await this.HttpClient.SendAsync(request, cancellationToken);
            var json = await response.Content?.ReadAsStringAsync(cancellationToken)!;
            if (!response.IsSuccessStatusCode)
            {
                this.Logger.LogError("An error occured while creating a new version of the artifact with the specified content hash '{contentHash}': the remote server responded with a non-success status code '{statusCode}'./r/Response content: {json}", json, response.StatusCode, content);
                response.EnsureSuccessStatusCode();
            }
            return await this.JsonSerializer.DeserializeAsync<Artifact>(json, cancellationToken);
        }

        #endregion

    }

}
