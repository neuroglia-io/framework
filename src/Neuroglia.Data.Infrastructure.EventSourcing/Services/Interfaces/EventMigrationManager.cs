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
