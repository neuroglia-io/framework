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

namespace Neuroglia.Data.Infrastructure.EventSourcing.Services;

/// <summary>
/// Defines the fundamentals of a service used to migrate events
/// </summary>
public interface IEventMigrationManager
{

    /// <summary>
    /// Registers a new migration
    /// </summary>
    /// <param name="eventType">The type to migrate from</param>
    /// <param name="handler">A <see cref="Func{T, TResult}"/> used to handle the event's migration</param>
    void RegisterEventMigration(Type eventType, Func<IServiceProvider, object, object> handler);

    /// <summary>
    /// Migrates the specified event to its latest version
    /// </summary>
    /// <param name="e">The event to migrate</param>
    object MigrateEventToLatest(object e);

}
