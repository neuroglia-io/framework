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

namespace Neuroglia.Data.SchemaRegistry.Services
{

    /// <summary>
    /// Defines the fundamentals of an Api Curio Registry client
    /// </summary>
    public interface IApiCurioRegistryClient
    {

        #region Artifacts

        /// <summary>
        /// Returns the latest version of the <see cref="Artifact"/> in its raw form. The Content-Type of the response depends on the <see cref="Artifact"/> type. In most cases, this is application/json, but for some types it may be different (for example, PROTOBUF).
        /// </summary>
        /// <param name="artifactId">The id of the <see cref="Artifact"/> to get</param>
        /// <param name="groupId">The id of the group of the <see cref="Artifact"/> to get</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The latest version of the specified <see cref="Artifact"/></returns>
        Task<string> GetLatestArtifactAsync(string artifactId, string groupId = "default", CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the content for an <see cref="Artifact"/> version in the registry using the unique content identifier for that content. This content ID may be shared by multiple artifact versions in the case where the <see cref="Artifact"/> versions are identical.
        /// </summary>
        /// <param name="contentId">The global identifier of the <see cref="Artifact"/> content to get</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Artifact"/> version with the specified global id</returns>
        Task<string?> GetArtifactContentByIdAsync(string contentId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the content for an <see cref="Artifact"/> version in the registry using its globally unique identifier
        /// </summary>
        /// <param name="globalId">The global identifier of the <see cref="Artifact"/> version to get</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Artifact"/> version with the specified global id</returns>
        Task<string?> GetArtifactContentByGlobalIdAsync(string globalId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the content for an <see cref="Artifact"/> version in the registry using the SHA-256 hash of the content. This content hash may be shared by multiple <see cref="Artifact"/> versions in the case where the artifact versions have identical content.
        /// </summary>
        /// <param name="contentHash">The SHA-256 hash of the content to get the <see cref="Artifact"/> version for</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Artifact"/> version with the specified SHA-256 hash</returns>
        Task<string?> GetArtifactContentBySha256HashAsync(string contentHash, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns a paginated list of all <see cref="Artifact"/>s that match the provided filter criteria.
        /// </summary>
        /// <param name="query">The query to perform</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new <see cref="ArtifactQueryResults"/></returns>
        Task<ArtifactQueryResults> SearchForArtifactsAsync(SearchForArtifactsQuery? query = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns a paginated list of all <see cref="Artifact"/>s with at least one version that matches the posted content.
        /// </summary>
        /// <param name="content">The content to search <see cref="Artifact"/>s by</param>
        /// <param name="query">The query to perform</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new <see cref="ArtifactQueryResults"/></returns>
        Task<ArtifactQueryResults> SearchForArtifactsByContentAsync(string content, SearchForArtifactsByContentQuery? query = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lists all <see cref="Artifact"/>s in the specified group
        /// </summary>
        /// <param name="groupId">The id of the group to list <see cref="Artifact"/>s for</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new <see cref="ArtifactQueryResults"/></returns>
        Task<ArtifactQueryResults> ListArtifactsInGroupAsync(string groupId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the state of the <see cref="Artifact"/>. For example, you can use this to mark the latest version of an <see cref="Artifact"/> as DEPRECATED. The operation changes the state of the latest version of the a<see cref="Artifact"/>tifact. If multiple versions exist, only the most recent is changed.
        /// </summary>
        /// <param name="groupId">The <see cref="Artifact"/> group ID. Must be a string provided by the client, representing the name of the grouping of <see cref="Artifact"/>s.</param>
        /// <param name="artifactId">The <see cref="Artifact"/> ID. Can be a string (client-provided) or UUID (server-generated), representing the unique <see cref="Artifact"/> identifier.</param>
        /// <param name="state">The state of an <see cref="Artifact"/> or <see cref="Artifact"/> version</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        Task UpdateArtifactStateAsync(string artifactId, ArtifactState state, string groupId = "default", CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an <see cref="Artifact"/> by uploading new content. The body of the request should be the raw content of the artifact. 
        /// This is typically in JSON format for most of the supported types, but may be in another format for a few (for example, PROTOBUF). The type of the content should be compatible with the <see cref="Artifact"/>'s type (it would be an error to update an AVRO artifact with new OPENAPI content, for example).
        /// </summary>
        /// <param name="groupId">The <see cref="Artifact"/> group ID. Must be a string provided by the client, representing the name of the grouping of <see cref="Artifact"/>s.</param>
        /// <param name="artifactId">The <see cref="Artifact"/> ID. Can be a string (client-provided) or UUID (server-generated), representing the unique <see cref="Artifact"/> identifier.</param>
        /// <param name="version">The version number of this new version of the <see cref="Artifact"/> content. This would typically be a simple integer or a SemVer value. If not provided, the server will assign a version number automatically.</param>
        /// <param name="name">The <see cref="Artifact"/> name of this new version of the <see cref="Artifact"/> content. Name must be ASCII-only string. If this is not provided, the server will extract the name from the <see cref="Artifact"/> content.</param>
        /// <param name="description">The <see cref="Artifact"/> description of this new version of the <see cref="Artifact"/> content. Description must be ASCII-only string. If this is not provided, the server will extract the description from the <see cref="Artifact"/> content.</param>
        /// <param name="content">The new content of the <see cref="Artifact"/></param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The updated <see cref="Artifact"/></returns>
        Task<Artifact> UpdateArtifactAsync(string artifactId, string content, string groupId = "default", string? version = null, string? name = null, string? description = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new artifact by posting the <see cref="Artifact"/> content. The body of the request should be the raw content of the <see cref="Artifact"/>. This is typically in JSON format for most of the supported types, but may be in another format for a few (for example, PROTOBUF).
        /// </summary>
        /// <param name="groupId">The unique ID of an <see cref="Artifact"/> group.</param>
        /// <param name="artifactType">The type of the <see cref="Artifact"/> to create</param>
        /// <param name="artifactId">A globally unique identifier for the <see cref="Artifact"/> to create</param>
        /// <param name="content">The content of the <see cref="Artifact"/> to create</param>
        /// <param name="ifExists">The option used to instruct the server on what to do if the <see cref="Artifact"/> already exists.</param>
        /// <param name="canonical">Used only when the ifExists query parameter is set to RETURN_OR_UPDATE, this parameter can be set to true to indicate that the server should "canonicalize" the content when searching for a matching version. 
        /// The canonicalization algorithm is unique to each <see cref="Artifact"/> type, but typically involves removing extra whitespace and formatting the content in a consistent manner.</param>
        /// <param name="version">The version number of this initial version of the <see cref="Artifact"/> content. This would typically be a simple integer or a SemVer value. If not provided, the server will assign a version number automatically (starting with version 1).</param>
        /// <param name="description">The description of <see cref="Artifact"/> being added. Description must be ASCII-only string. If this is not provided, the server will extract the description from the <see cref="Artifact"/> content.</param>
        /// <param name="name">The name of <see cref="Artifact"/> being added. Name must be ASCII-only string. If this is not provided, the server will extract the name from the <see cref="Artifact"/> content</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The newly created <see cref="Artifact"/></returns>
        Task<Artifact> CreateArtifactAsync(ArtifactType artifactType, string content, IfArtifactExistsAction ifExists, string? artifactId = null, string groupId = "default", bool canonical = false, string? version = null, string? name = null, string? description = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes the <see cref="Artifact"/> with the specified id
        /// </summary>
        /// <param name="artifactId">The id of the <see cref="Artifact"/> to delete</param>
        /// <param name="groupId">The id of the group the <see cref="Artifact"/> to delete belongs to</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        Task DeleteArtifactAsync(string artifactId, string groupId = "default", CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes all <see cref="Artifact"/>s in the specified group
        /// </summary>
        /// <param name="groupId">The id of the group to remove all <see cref="Artifact"/>s from</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        Task DeleteAllArtifactsInGroupAsync(string groupId, CancellationToken cancellationToken = default);

        #endregion

        #region Metadata

        /// <summary>
        /// Gets the metadata of the <see cref="Artifact"/> with the specified id
        /// </summary>
        /// <param name="artifactId">The id of the <see cref="Artifact"/> to get the metadata of</param>
        /// <param name="groupId">The id of the group the <see cref="Artifact"/> to get the metadata of belongs to</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The metadata of the <see cref="Artifact"/> with the specified id</returns>
        Task<Artifact> GetArtifactMetadataAsync(string artifactId, string groupId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the metadata of a specific version of an <see cref="Artifact"/>
        /// </summary>
        /// <param name="artifactId">The id of the <see cref="Artifact"/> to get the metadata of</param>
        /// <param name="groupId">The id of the group the <see cref="Artifact"/> to get the metadata of belongs to</param>
        /// <param name="version">The version of the <see cref="Artifact"/> to get the metadata of</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The metadata of the <see cref="Artifact"/> with the specified id</returns>
        Task<Artifact> GetArtifactVersionMetadataAsync(string artifactId, string groupId, string version, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the metadata of a specific version of an <see cref="Artifact"/>
        /// </summary>
        /// <param name="artifactId">The id of the <see cref="Artifact"/> to get the metadata of</param>
        /// <param name="groupId">The id of the group the <see cref="Artifact"/> to get the metadata of belongs to</param>
        /// <param name="content">The content of the <see cref="Artifact"/> version to update</param>
        /// <param name="canonical">The content of the <see cref="Artifact"/> version to get</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The metadata of the <see cref="Artifact"/> with the specified id</returns>
        Task<Artifact> GetArtifactVersionMetadataByContentAsync(string artifactId, string groupId, string content, bool? canonical = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the metadata of the <see cref="Artifact"/> with the specified id
        /// </summary>
        /// <param name="artifactId">The id of the <see cref="Artifact"/> to update</param>
        /// <param name="groupId">The id of the group the <see cref="Artifact"/> to update belongs to</param>
        /// <param name="name">The updated <see cref="Artifact"/> name</param>
        /// <param name="description">The updated <see cref="Artifact"/> description</param>
        /// <param name="labels">The updated <see cref="Artifact"/> labels</param>
        /// <param name="properties">The updated <see cref="Artifact"/> user-defined properties</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        Task UpdateArtifactMetadataAsync(string artifactId, string groupId, string? name = null, string? description = null, ICollection<string>? labels = null, IDictionary<string, string>? properties = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the metadata of a specific version of an <see cref="Artifact"/>
        /// </summary>
        /// <param name="artifactId">The id of the <see cref="Artifact"/> to update</param>
        /// <param name="groupId">The id of the group the <see cref="Artifact"/> to update belongs to</param>
        /// <param name="version">The version of the <see cref="Artifact"/> to update the metadata of</param>
        /// <param name="name">The updated <see cref="Artifact"/> name</param>
        /// <param name="description">The updated <see cref="Artifact"/> description</param>
        /// <param name="labels">The updated <see cref="Artifact"/> labels</param>
        /// <param name="properties">The updated <see cref="Artifact"/> user-defined properties</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        Task UpdateArtifactVersionMetadataAsync(string artifactId, string groupId, string version, string? name = null, string? description = null, ICollection<string>? labels = null, IDictionary<string, string>? properties = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes the metadata of a specific version of an <see cref="Artifact"/>
        /// </summary>
        /// <param name="version">The id of the <see cref="Artifact"/> to delete the metadata of</param>
        /// <param name="artifactId">The id of the group the <see cref="Artifact"/> to delete the metadata of belongs to</param>
        /// <param name="cancellationToken">The version of the <see cref="Artifact"/> to delete the metadata of</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        Task DeleteArtifactVersionMetadataAsync(string artifactId, string groupId, string version, CancellationToken cancellationToken = default);

        #endregion

        #region Versions

        /// <summary>
        /// Gets the specified <see cref="Artifact"/> version
        /// </summary>
        /// <param name="artifactId">The id of the <see cref="Artifact"/> to get</param>
        /// <param name="groupId">The id of the group the <see cref="Artifact"/> to get belongs to</param>
        /// <param name="version">The version of the <see cref="Artifact"/> to get</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The content of the <see cref="Artifact"/> with the specified id and version</returns>
        Task<string> GetArtifactVersionAsync(string artifactId, string groupId, string version, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lists all the versions of the specified <see cref="Artifact"/>
        /// </summary>
        /// <param name="artifactId">The id of the <see cref="Artifact"/> to get the versions of</param>
        /// <param name="groupId">The id of the group the <see cref="Artifact"/> to get the versions of belongs to</param>
        /// <param name="offset">The number of versions to skip before starting to collect the result set. Defaults to 0.</param>
        /// <param name="limit">The number of versions to return. Defaults to 20.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new <see cref="ArtifactQueryResults"/></returns>
        Task<ArtifactQueryResults> ListArtifactVersionsAsync(string artifactId, string groupId, long offset = 0, long limit = 20, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the state of the specified <see cref="Artifact"/> version
        /// </summary>
        /// <param name="groupId">The <see cref="Artifact"/> group ID. Must be a string provided by the client, representing the name of the grouping of <see cref="Artifact"/>s.</param>
        /// <param name="artifactId">The <see cref="Artifact"/> ID. Can be a string (client-provided) or UUID (server-generated), representing the unique <see cref="Artifact"/> identifier.</param>
        /// <param name="version">The version of the <see cref="Artifact"/> to update the state of</param>
        /// <param name="state">The state of an <see cref="Artifact"/> or <see cref="Artifact"/> version</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        Task UpdateArtifactVersionStateAsync(string artifactId, string groupId, string version, ArtifactState state, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new version of an <see cref="Artifact"/> by posting the <see cref="Artifact"/> content. The body of the request should be the raw content of the <see cref="Artifact"/>. This is typically in JSON format for most of the supported types, but may be in another format for a few (for example, PROTOBUF).
        /// </summary>
        /// <param name="groupId">The unique ID of an <see cref="Artifact"/> group.</param>
        /// <param name="artifactId">A globally unique identifier for the <see cref="Artifact"/> to create</param>
        /// <param name="content">The content of the <see cref="Artifact"/> to create</param>
        /// <param name="canonical">Used only when the ifExists query parameter is set to RETURN_OR_UPDATE, this parameter can be set to true to indicate that the server should "canonicalize" the content when searching for a matching version. 
        /// The canonicalization algorithm is unique to each <see cref="Artifact"/> type, but typically involves removing extra whitespace and formatting the content in a consistent manner.</param>
        /// <param name="version">The version number of this initial version of the <see cref="Artifact"/> content. This would typically be a simple integer or a SemVer value. If not provided, the server will assign a version number automatically (starting with version 1).</param>
        /// <param name="description">The description of <see cref="Artifact"/> being added. Description must be ASCII-only string. If this is not provided, the server will extract the description from the <see cref="Artifact"/> content.</param>
        /// <param name="name">The name of <see cref="Artifact"/> being added. Name must be ASCII-only string. If this is not provided, the server will extract the name from the <see cref="Artifact"/> content</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The newly created <see cref="Artifact"/></returns>
        Task<Artifact> CreateArtifactVersionAsync(string artifactId, string groupId, string content, string? version = null, string? name = null, string? description = null, CancellationToken cancellationToken = default);

        #endregion

        #region Artifact rules


        #endregion region

        #region Global rules


        #endregion

        #region Search


        #endregion

        #region Admin


        #endregion

        #region System


        #endregion

        #region Users


        #endregion

    }

}
