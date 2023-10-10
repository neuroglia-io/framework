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
    /// <param name="to">The type to migrate to</param>
    /// <param name="handler">A <see cref="Func{T, TResult}"/> used to handle the event's migration</param>
    void RegisterEventMigration(Type eventType, Func<IServiceProvider, object, object> handler);

    /// <summary>
    /// Migrates the specified event to its latest version
    /// </summary>
    /// <param name="e">The event to migrate</param>
    object MigrateEventToLatest(object e);

}
