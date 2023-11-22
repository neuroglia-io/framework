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

using Neuroglia.Data.Infrastructure.EventSourcing.Services;
using Neuroglia.Serialization;

namespace Neuroglia.Data.Infrastructure.EventSourcing.Configuration;

/// <summary>
/// Represents the options used to configure an <see cref="IEventStore"/>
/// </summary>
public class EventStoreOptions
{

    /// <summary>
    /// Gets/sets the name of the database to use, if any
    /// </summary>
    public string? DatabaseName { get; set; }

    /// <summary>
    /// Gets/sets the type of <see cref="ISerializer"/> to use to serialize and deserialize events. If not set, will use the first registered <see cref="ISerializer"/>
    /// </summary>
    public Type? SerializerType { get; set; }

}
