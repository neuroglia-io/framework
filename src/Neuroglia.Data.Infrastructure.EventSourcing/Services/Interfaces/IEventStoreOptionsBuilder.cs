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
    /// Uses the specified <see cref="IEventAggregatorFactory"/> to create <see cref="IEventAggregator"/>s
    /// </summary>
    /// <typeparam name="TFactory">The type of <see cref="IEventAggregatorFactory"/> to use to create <see cref="IEventAggregator"/>s</typeparam>
    /// <returns>The configured <see cref="IEventStoreOptionsBuilder"/></returns>
    IEventStoreOptionsBuilder UseAggregatorFactory<TFactory>()
        where TFactory : class, IEventAggregatorFactory;

    /// <summary>
    /// Uses the specified <see cref="IEventMigrationManager"/> to manage event migrations
    /// </summary>
    /// <typeparam name="TFactory">The type of <see cref="IEventMigrationManager"/> to use</typeparam>
    /// <returns>The configured <see cref="IEventStoreOptionsBuilder"/></returns>
    IEventStoreOptionsBuilder UseMigrationManager<TMigrationManager>()
        where TMigrationManager : class, IEventMigrationManager;

    /// <summary>
    /// Builds the <see cref="EventStoreOptions"/>
    /// </summary>
    /// <returns>A new <see cref="EventStoreOptions"/></returns>
    EventStoreOptions Build();

}
