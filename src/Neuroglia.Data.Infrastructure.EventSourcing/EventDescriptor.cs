namespace Neuroglia.Data.Infrastructure.EventSourcing;

/// <summary>
/// Represents the default implementation of the <see cref="IEventDescriptor"/> interface
/// </summary>
public class EventDescriptor
    : IEventDescriptor
{

    /// <summary>
    /// Initializes a new <see cref="EventDescriptor"/>
    /// </summary>
    /// <param name="type">The type of the described event</param>
    /// <param name="data">The data, if any, of the described event</param>
    /// <param name="metadata">The metadata, if any, associated to the described event</param>
    public EventDescriptor(string type, object? data, IDictionary<string, object>? metadata = null)
    {
        if (string.IsNullOrWhiteSpace(type)) throw new ArgumentNullException(nameof(type));
        Type = type;
        Data = data;
        Metadata = metadata;
    }

    /// <inheritdoc/>
    public virtual string Type { get; }

    /// <inheritdoc/>
    public virtual object? Data { get; }

    /// <inheritdoc/>
    public virtual IDictionary<string, object>? Metadata { get; }

}
