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

namespace Neuroglia.Data.SchemaRegistry.Models
{

    /// <summary>
    /// Describes an artifact
    /// </summary>
    public class Artifact
    {

        /// <summary>
        /// Gets/sets the <see cref="Artifact"/>'s id
        /// </summary>
        [Newtonsoft.Json.JsonProperty("id")]
        [System.Text.Json.Serialization.JsonPropertyName("id")]
        public virtual string Id { get; set; } = null!;

        /// <summary>
        /// Gets/sets the <see cref="Artifact"/>'s global id
        /// </summary>
        [Newtonsoft.Json.JsonProperty("globalId")]
        [System.Text.Json.Serialization.JsonPropertyName("globalId")]
        public virtual string? GlobalId { get; set; } = null!;

        /// <summary>
        /// Gets/sets the <see cref="Artifact"/>'s contentId
        /// </summary>
        [Newtonsoft.Json.JsonProperty("contentId")]
        [System.Text.Json.Serialization.JsonPropertyName("contentId")]
        public virtual string? ContentId { get; set; } = null!;

        /// <summary>
        /// Gets/sets the <see cref="Artifact"/>'s name
        /// </summary>
        [Newtonsoft.Json.JsonProperty("name")]
        [System.Text.Json.Serialization.JsonPropertyName("name")]
        public virtual string Name { get; set; } = null!;

        /// <summary>
        /// Gets/sets the <see cref="Artifact"/>'s description
        /// </summary>
        [Newtonsoft.Json.JsonProperty("description")]
        [System.Text.Json.Serialization.JsonPropertyName("description")]
        public virtual string Description { get; set; } = null!;

        /// <summary>
        /// Gets/sets date and time at which the <see cref="Artifact"/> has been created
        /// </summary>
        [Newtonsoft.Json.JsonProperty("createdOn")]
        [System.Text.Json.Serialization.JsonPropertyName("createdOn")]
        public virtual DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets/sets the user the <see cref="Artifact"/> has been created by
        /// </summary>
        [Newtonsoft.Json.JsonProperty("createdBy")]
        [System.Text.Json.Serialization.JsonPropertyName("createdBy")]
        public virtual string CreatedBy { get; set; } = null!;

        /// <summary>
        /// Gets/sets <see cref="Artifact"/>'s type
        /// </summary>
        [Newtonsoft.Json.JsonProperty("type")]
        [System.Text.Json.Serialization.JsonPropertyName("type")]
        public virtual ArtifactType Type { get; set; }

        /// <summary>
        /// Gets/sets a collection containing the <see cref="Artifact"/>'s labels
        /// </summary>
        [Newtonsoft.Json.JsonProperty("labels")]
        [System.Text.Json.Serialization.JsonPropertyName("labels")]
        public virtual ICollection<string>? Labels { get; set; }

        /// <summary>
        /// Gets/sets the <see cref="Artifact"/>'s state
        /// </summary>
        [Newtonsoft.Json.JsonProperty("state")]
        [System.Text.Json.Serialization.JsonPropertyName("state")]
        public virtual ArtifactState State { get; set; }

        /// <summary>
        /// Gets the date and time the <see cref="Artifact"/> has last been modified on
        /// </summary>
        [Newtonsoft.Json.JsonProperty("modifiedOn")]
        [System.Text.Json.Serialization.JsonPropertyName("modifiedOn")]
        public virtual DateTime? ModifiedOn { get; set; }

        /// <summary>
        /// Gets/sets the user the <see cref="Artifact"/> has last been modified by
        /// </summary>
        [Newtonsoft.Json.JsonProperty("modifiedBy")]
        [System.Text.Json.Serialization.JsonPropertyName("modifiedBy")]
        public virtual string? ModifiedBy { get; set; }

        /// <summary>
        /// Gets/sets the id of the <see cref="Artifact"/>'s group
        /// </summary>
        [Newtonsoft.Json.JsonProperty("groupId")]
        [System.Text.Json.Serialization.JsonPropertyName("groupId")]
        public virtual string? GroupId { get; set; }

        /// <summary>
        /// Gets/sets the <see cref="Artifact"/>'s version
        /// </summary>
        [Newtonsoft.Json.JsonProperty("version")]
        [System.Text.Json.Serialization.JsonPropertyName("version")]
        public virtual string? Version { get; set; }

        /// <summary>
        /// Gets/sets the <see cref="Artifact"/>'s user-defined name/value pairs
        /// </summary>
        [Newtonsoft.Json.JsonProperty("properties")]
        [System.Text.Json.Serialization.JsonPropertyName("properties")]
        public virtual IDictionary<string, string>? Properties { get; set; }

    }

}
