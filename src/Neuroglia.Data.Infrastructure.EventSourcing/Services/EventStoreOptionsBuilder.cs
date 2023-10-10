using Neuroglia.Data.Infrastructure.EventSourcing.Configuration;
using Neuroglia.Serialization;

namespace Neuroglia.Data.Infrastructure.EventSourcing.Services;

/// <summary>
/// Represents the default implementation of the <see cref="IEventStoreOptionsBuilder"/> interface
/// </summary>
public class EventStoreOptionsBuilder
    : IEventStoreOptionsBuilder
{

    /// <summary>
    /// Gets the <see cref="EventStoreOptions"/> to configure
    /// </summary>
    protected EventStoreOptions Options { get; } = new();

    /// <inheritdoc/>
    public virtual IEventStoreOptionsBuilder UseSerializer<TSerializer>()
        where TSerializer : class, ISerializer
    {
        this.Options.SerializerType = typeof(TSerializer);
        return this;
    }

    /// <inheritdoc/>
    public virtual EventStoreOptions Build() => this.Options;

}
