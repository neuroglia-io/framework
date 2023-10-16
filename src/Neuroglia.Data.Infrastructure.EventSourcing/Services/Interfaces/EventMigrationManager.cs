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

using System.Collections.Concurrent;

namespace Neuroglia.Data.Infrastructure.EventSourcing.Services;

/// <summary>
/// Represents the default implementation of the <see cref="IEventMigrationManager"/>
/// </summary>
public class EventMigrationManager
    : IEventMigrationManager
{

    /// <summary>
    /// Initializes a new <see cref="EventMigrationManager"/>
    /// </summary>
    /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
    public EventMigrationManager(IServiceProvider serviceProvider)
    {
        this.ServiceProvider = serviceProvider;
    }

    /// <summary>
    /// Gets the current <see cref="IServiceProvider"/>
    /// </summary>
    protected IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Gets a type/handler mapping of events for which a migration has been registered 
    /// </summary>
    protected ConcurrentDictionary<Type, Func<IServiceProvider, object, object>> Migrations { get; } = new();

    /// <inheritdoc/>
    public virtual void RegisterEventMigration(Type sourceType, Func<IServiceProvider, object, object> handler)
    {
        if (sourceType == null) throw new ArgumentNullException(nameof(sourceType));
        if (handler == null) throw new ArgumentNullException(nameof(handler));
        this.Migrations.AddOrUpdate(sourceType, handler, (key, current) => handler);
    }

    /// <inheritdoc/>
    public virtual object MigrateEventToLatest(object e)
    {
        if (e == null) throw new ArgumentNullException(nameof(e));
        if (!this.Migrations.TryGetValue(e.GetType(), out var handler) || handler == null) return e;
        return this.MigrateEventToLatest(handler.Invoke(this.ServiceProvider, e));
    }

}
