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

namespace Neuroglia.Data.Infrastructure.EventSourcing;

/// <summary>
/// Represents the default implementation of the <see cref="IEventDescriptor"/> interface
/// </summary>
public class EventDescriptor
    : IEventDescriptor
{

    /// <summary>
    /// Initializes a new <see cref="EventDescriptor"/>
    /// </summary>
    /// <param name="type">The type of the described event</param>
    /// <param name="data">The data, if any, of the described event</param>
    /// <param name="metadata">The metadata, if any, associated to the described event</param>
    public EventDescriptor(string type, object? data, IDictionary<string, object>? metadata = null)
    {
        if (string.IsNullOrWhiteSpace(type)) throw new ArgumentNullException(nameof(type));
        this.Type = type;
        this.Data = data;
        this.Metadata = metadata;
    }

    /// <inheritdoc/>
    public virtual string Type { get; }

    /// <inheritdoc/>
    public virtual object? Data { get; }

    /// <inheritdoc/>
    public virtual IDictionary<string, object>? Metadata { get; }

}
