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

namespace Neuroglia.Eventing.CloudEvents.Infrastructure.Configuration;

/// <summary>
/// Represents the options used to configure the application's <see cref="CloudEvent"/>s
/// </summary>
public class CloudEventOptions
{

    /// <summary>
    /// Initializes a new <see cref="CloudEventOptions"/>
    /// </summary>
    public CloudEventOptions()
    {
        var env = Environment.GetEnvironmentVariable(EnvironmentVariables.Sink);
        if (!string.IsNullOrWhiteSpace(env) && Uri.TryCreate(env, new UriCreationOptions(), out var sink) && sink != null) this.Sink = sink;

        env = Environment.GetEnvironmentVariable(EnvironmentVariables.Source);
        if (!string.IsNullOrWhiteSpace(env) && Uri.TryCreate(env, new UriCreationOptions(), out var source) && source != null) this.Source = source;

        env = Environment.GetEnvironmentVariable(EnvironmentVariables.SchemaRegistry);
        if (!string.IsNullOrWhiteSpace(env) && Uri.TryCreate(env, new UriCreationOptions(), out var schemaRegistry) && schemaRegistry != null) this.SchemaRegistry = schemaRegistry;
    }

    /// <summary>
    /// Gets/sets the <see cref="Uri"/> of the endpoint to post published <see cref="CloudEvent"/>s to, if any
    /// </summary>
    public virtual Uri? Sink { get; set; }

    /// <summary>
    /// Gets/sets the <see cref="Uri"/> to use as source of all published <see cref="CloudEvent"/>s
    /// </summary>
    public virtual Uri Source { get; set; } = new("https://product-manager.posm.io/");

    /// <summary>
    /// Gets/sets the <see cref="Uri"/> of the application's <see cref="CloudEvent"/> schemas registry
    /// </summary>
    public virtual Uri? SchemaRegistry { get; set; }

    /// <summary>
    /// Exposes the environments variables used to configure the <see cref="CloudEventOptions"/>
    /// </summary>
    public static class EnvironmentVariables
    {

        const string Prefix = "CLOUDEVENTS_";
        /// <summary>
        /// Gets the name of the environment variable used to set the <see cref="Uri"/> of the endpoint to post published <see cref="CloudEvent"/>s to, if any
        /// </summary>
        public const string Sink = Prefix + "SINK";
        /// <summary>
        /// Gets the name of the environment variable used to get the source of published <see cref="CloudEvent"/>s
        /// </summary>
        public const string Source = Prefix + "SOURCE";
        /// <summary>
        /// Gets the name of the environment variable used to get the <see cref="Uri"/> of the application's <see cref="CloudEvent"/> schemas registry
        /// </summary>
        public const string SchemaRegistry = Prefix + "SCHEMA_REGISTRY";

    }

}
