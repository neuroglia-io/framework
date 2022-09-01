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

using System.Runtime.Serialization;

namespace Neuroglia.Data.SchemaRegistry
{
    /// <summary>
    /// Enumerates all supported artifact states
    /// </summary>
    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]

    public enum ArtifactState
    {
        /// <summary>
        /// Indicates the the artifact is enabled
        /// </summary>
        [EnumMember(Value = "ENABLED")]
        Enabled,
        /// <summary>
        /// Indicates the the artifact is disabled
        /// </summary>
        [EnumMember(Value = "DISABLED")]
        Disabled,
        /// <summary>
        /// Indicates the the artifact has been deprecated
        /// </summary>
        [EnumMember(Value = "DEPRECATED")]
        Deprecated
    }

}
