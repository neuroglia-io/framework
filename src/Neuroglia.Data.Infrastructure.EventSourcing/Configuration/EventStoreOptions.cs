using Neuroglia.Data.Infrastructure.EventSourcing.Services;
using Neuroglia.Serialization;

namespace Neuroglia.Data.Infrastructure.EventSourcing.Configuration;

/// <summary>
/// Represents the options used to configure an <see cref="IEventStore"/>
/// </summary>
public class EventStoreOptions
{

    /// <summary>
    /// Gets/sets the type of <see cref="ISerializer"/> to use to serialize and deserialize events. If not set, will use the first registered <see cref="ISerializer"/>
    /// </summary>
    public Type? SerializerType { get; set; }

    /// <summary>
    /// Gets/sets the type of <see cref="IEventAggregatorFactory"/> to use to create <see cref="IEventAggregator"/>s
    /// </summary>
    public Type AggregatorFactoryType { get; set; } = typeof(EventAggregatorFactory);

    /// <summary>
    /// Gets/sets the type of <see cref="IEventMigrationManager"/> to use to migrate events
    /// </summary>
    public Type MigrationManagerType { get; set; } = typeof(EventMigrationManager);

}
