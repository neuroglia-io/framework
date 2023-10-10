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
    /// Builds the <see cref="EventStoreOptions"/>
    /// </summary>
    /// <returns>A new <see cref="EventStoreOptions"/></returns>
    EventStoreOptions Build();

}
