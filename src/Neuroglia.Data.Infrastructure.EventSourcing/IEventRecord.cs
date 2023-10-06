namespace Neuroglia.Data.Infrastructure.EventSourcing;

/// <summary>
/// Defines the fundamentals of an object used to describe a recorded event
/// </summary>
public interface IEventRecord
{

    /// <summary>
    /// Gets the id of the recorded event
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Gets the offset of the recorded event
    /// </summary>
    ulong Offset { get; }

    /// <summary>
    /// Gets the date and time at which the event has been recorded
    /// </summary>
    DateTimeOffset Timestamp { get; }

    /// <summary>
    /// Gets the type of the recorded event. Should be a non-versioned reverse uri made out alphanumeric, '-' and '.' characters
    /// </summary>
    string Type { get; }

    /// <summary>
    /// Gets the assembly qualified name of the recorded event type, if any
    /// </summary>
    string? ClrType { get; }

    /// <summary>
    /// Gets the data of the recorded event
    /// </summary>
    IDictionary<string, object>? Data { get; }

    /// <summary>
    /// Gets the metadata of the recorded event
    /// </summary>
    IDictionary<string, object>? Metadata { get; }

}
