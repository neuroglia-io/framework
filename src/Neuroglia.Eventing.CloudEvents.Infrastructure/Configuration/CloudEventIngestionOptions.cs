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
/// Represents the options used to configure how incoming <see cref="CloudEvent"/>s are to be ingested
/// </summary>
public class CloudEventIngestionOptions
{

    /// <summary>
    /// Gets/sets a list containing the objects used to configure how to ingest specific types of <see cref="CloudEvent"/>s
    /// </summary>
    public virtual List<CloudEventIngestionConfiguration> Events { get; set; } = [];

    /// <summary>
    /// Configures the <see cref="CloudEventIngestionOptions"/> to ingest <see cref="CloudEvent"/>s of the specified type
    /// </summary>
    /// <param name="dataType">The type to deserialize the <see cref="CloudEvent"/>'s data to</param>
    /// <param name="type">The type of <see cref="CloudEvent"/>s to ingest</param>
    public virtual void IngestEventsOfType(Type dataType, string? type = null)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            if (dataType.TryGetCustomAttribute<CloudEventAttribute>(out var cloudEventAttribute) && cloudEventAttribute != null) type = cloudEventAttribute.Type;
            else throw new ArgumentNullException(nameof(type));
        }
        this.Events.Add(new() { Type = type, DataType = dataType });
    }

    /// <summary>
    /// Configures the <see cref="CloudEventIngestionOptions"/> to ingest <see cref="CloudEvent"/>s of the specified type
    /// </summary>
    /// <typeparam name="TData">The type to deserialize the <see cref="CloudEvent"/>'s data to</typeparam>
    /// <param name="type">The type of <see cref="CloudEvent"/>s to ingest</param>
    public virtual void IngestEventsOfType<TData>(string? type = null) => this.IngestEventsOfType(typeof(TData), type);

}
