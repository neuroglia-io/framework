namespace Neuroglia.Data.Infrastructure.EventSourcing;

/// <summary>
/// Represents the default implementation of the <see cref="IEventRecord"/> interface
/// </summary>
public class EventRecord
    : IEventRecord
{

    /// <summary>
    /// Initializes a new <see cref="EventRecord"/>
    /// </summary>
    /// <param name="id">The id of the recorded event</param>
    /// <param name="offset">The offset of the recorded event</param>
    /// <param name="timestamp">The date and time at which the event has been recorded</param>
    /// <param name="type">The type of the recorded event. Should be a non-versioned reverse uri made out alphanumeric, '-' and '.' characters</param>
    /// <param name="clrType">The assembly qualified name of the recorded event type, if any</param>
    /// <param name="data">The data of the recorded event</param>
    /// <param name="metadata">The metadata of the recorded event</param>
    public EventRecord(string id, ulong offset, DateTimeOffset timestamp, string type, string? clrType = null, IDictionary<string, object>? data = null, IDictionary<string, object>? metadata = null)
    {
        this.Id = id;
        this.Offset = offset;
        this.Timestamp = timestamp;
        this.Type = type;
        this.ClrType = clrType;
        this.Data = data;
        this.Metadata = metadata;
    }

    /// <inheritdoc/>
    public string Id { get; }

    /// <inheritdoc/>
    public ulong Offset { get; }

    /// <inheritdoc/>
    public DateTimeOffset Timestamp { get; }

    /// <inheritdoc/>
    public string Type { get; }

    /// <inheritdoc/>
    public string? ClrType { get; }

    /// <inheritdoc/>
    public IDictionary<string, object>? Data { get; }

    /// <inheritdoc/>
    public IDictionary<string, object>? Metadata { get; }

}
