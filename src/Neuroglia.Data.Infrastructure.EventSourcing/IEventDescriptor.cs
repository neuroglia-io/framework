namespace Neuroglia.Data.Infrastructure.EventSourcing;

/// <summary>
/// Defines the fundamentals of an object used to describe an event to persist
/// </summary>
public interface IEventDescriptor
{

    /// <summary>
    /// Gets the type of the described event
    /// </summary>
    string Type { get; }

    /// <summary>
    /// Gets the data, if any, associated to the described event
    /// </summary>
    object? Data { get; }

    /// <summary>
    /// Gets the metadata, if any, associated to the described event
    /// </summary>
    IDictionary<string, object>? Metadata { get; }

}
