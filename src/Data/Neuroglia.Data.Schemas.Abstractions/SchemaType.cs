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

namespace Neuroglia.Data
{

    /// <summary>
    /// Enumerates all supported schema types
    /// </summary>
    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
    public enum SchemaType
    {
        /// <summary>
        /// Indicates a JSON schema
        /// </summary>
        [EnumMember(Value = "json")]
        Json = 1,
        /// <summary>
        /// Indicates a JSON Form schema
        /// </summary>
        [EnumMember(Value = "jsonform")]
        JsonForm = 2,
        /// <summary>
        /// Indicates a Google Protobuf schema
        /// </summary>
        [EnumMember(Value = "proto")]
        Proto = 4,
        /// <summary>
        /// Indicates an OpenAPI schema
        /// </summary>
        [EnumMember(Value = "openapi")]
        OpenApi = 8,
        /// <summary>
        /// Indicates an AsyncAPI schela
        /// </summary>
        [EnumMember(Value = "asyncapi")]
        AsyncApi = 16,
        /// <summary>
        /// Indicates an ODATA schema
        /// </summary>
        [EnumMember(Value = "odata")]
        OData = 32,
        /// <summary>
        /// Indicates a GraphQL schema
        /// </summary>
        [EnumMember(Value = "graphql")]
        GraphQL = 64
    }

}
