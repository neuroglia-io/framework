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
    /// Represents the command used to update the metadata of an <see cref="Artifact"/>
    /// </summary>
    public class UpdateArtifactMetadataCommand
    {

        /// <summary>
        /// Gets/sets the <see cref="Artifact"/>'s name
        /// </summary>
        [Newtonsoft.Json.JsonProperty("name")]
        [System.Text.Json.Serialization.JsonPropertyName("name")]
        public virtual string? Name { get; set; }

        /// <summary>
        /// Gets/sets the <see cref="Artifact"/>'s description
        /// </summary>
        [Newtonsoft.Json.JsonProperty("description")]
        [System.Text.Json.Serialization.JsonPropertyName("description")]
        public virtual string? Description { get; set; }

        /// <summary>
        /// Gets/sets a collection containing the <see cref="Artifact"/>'s labels
        /// </summary>
        [Newtonsoft.Json.JsonProperty("labels")]
        [System.Text.Json.Serialization.JsonPropertyName("labels")]
        public virtual ICollection<string>? Labels { get; set; }

        /// <summary>
        /// Gets/sets the <see cref="Artifact"/>'s user-defined name/value pairs
        /// </summary>
        [Newtonsoft.Json.JsonProperty("properties")]
        [System.Text.Json.Serialization.JsonPropertyName("properties")]
        public virtual IDictionary<string, string>? Properties { get; set; }

    }

}
