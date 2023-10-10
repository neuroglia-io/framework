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

}
