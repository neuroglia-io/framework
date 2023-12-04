// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License"),
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Neuroglia.Eventing.CloudEvents;

/// <summary>
/// Enumerates all supported version of the <see href="https://cloudevents.io/">Cloud Event spec</see>
/// </summary>
public static class CloudEventSpecVersion
{

    /// <summary>
    /// Exposes information about the <see href="https://cloudevents.io/">Cloud Event spec</see> version 1.0
    /// </summary>
    public static class V1
    {

        /// <summary>
        /// Gets the '1.0' version of the <see href="https://cloudevents.io/">Cloud Event spec</see>
        /// </summary>
        public const string Version = "1.0";
        /// <summary>
        /// Gets the <see cref="Uri"/> that references the JSON Schema of the <see href="https://cloudevents.io/">Cloud Event spec</see> version 1.0
        /// </summary>
        public static readonly Uri SchemaUri = new("https://raw.githubusercontent.com/cloudevents/spec/v1.0.1/spec.json");

    }

}