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

using Neuroglia.Data.Infrastructure.EventSourcing.Configuration;
using Neuroglia.Serialization;

namespace Neuroglia.Data.Infrastructure.EventSourcing.Services;

/// <summary>
/// Defines the fundamentals of a service used to build <see cref="EventStoreOptions"/>
/// </summary>
public interface IEventStoreOptionsBuilder
{

    /// <summary>
    /// Uses the specified <see cref="ISerializer"/> to serialize and deserialize events
    /// </summary>
    /// <typeparam name="TSerializer">The type of <see cref="ISerializer"/> to use to serialize and deserialize events</typeparam>
    /// <returns>The configured <see cref="IEventStoreOptionsBuilder"/></returns>
    IEventStoreOptionsBuilder UseSerializer<TSerializer>()
        where TSerializer : class, ISerializer;

    /// <summary>
    /// Uses the specified database
    /// </summary>
    /// <param name="databaseName">The name of the database to use</param>
    /// <returns>The configured <see cref="IEventStoreOptionsBuilder"/></returns>
    IEventStoreOptionsBuilder UseDatabase(string databaseName);

    /// <summary>
    /// Builds the <see cref="EventStoreOptions"/>
    /// </summary>
    /// <returns>A new <see cref="EventStoreOptions"/></returns>
    EventStoreOptions Build();

}
