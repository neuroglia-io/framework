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
    /// Represents the results of an <see cref="Artifact"/> query
    /// </summary>
    public class ArtifactQueryResults
    {

        /// <summary>
        /// Gets/sets an <see cref="ICollection{T}"/> containing <see cref="Artifact"/> results
        /// </summary>
        [Newtonsoft.Json.JsonProperty("artifacts")]
        [System.Text.Json.Serialization.JsonPropertyName("artifacts")]
        public virtual ICollection<Artifact> Artifacts { get; set; } = new List<Artifact>();

        /// <summary>
        /// Gets/sets the result count
        /// </summary>
        [Newtonsoft.Json.JsonProperty("count")]
        [System.Text.Json.Serialization.JsonPropertyName("count")]
        public virtual long Count { get; set; }

    }

}
