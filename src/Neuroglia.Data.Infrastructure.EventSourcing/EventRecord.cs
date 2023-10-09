using System.Runtime.Serialization;

namespace Neuroglia.Data.Infrastructure.EventSourcing;

/// <summary>
/// Represents the default implementation of the <see cref="IEventRecord"/> interface
/// </summary>
[DataContract]
public class EventRecord
    : IEventRecord
{

    /// <summary>
    /// Initializes a new <see cref="EventRecord"/>
    /// </summary>
    protected EventRecord() { }

    /// <summary>
    /// Initializes a new <see cref="EventRecord"/>
    /// </summary>
    /// <param name="id">The id of the recorded event</param>
    /// <param name="offset">The offset of the recorded event</param>
    /// <param name="timestamp">The date and time at which the event has been recorded</param>
    /// <param name="type">The type of the recorded event. Should be a non-versioned reverse uri made out alphanumeric, '-' and '.' characters</param>
    /// <param name="data">The data of the recorded event</param>
    /// <param name="metadata">The metadata of the recorded event</param>
    public EventRecord(string id, ulong offset, DateTimeOffset timestamp, string type, object? data = null, IDictionary<string, object>? metadata = null)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));
        this.Id = id;
        this.Offset = offset;
        this.Timestamp = timestamp;
        this.Type = type;
        this.Data = data;
        this.Metadata = metadata;
    }

    /// <inheritdoc/>
    [DataMember]
    public virtual string Id { get; protected set; } = null!;

    /// <inheritdoc/>
    [DataMember]
    public virtual ulong Offset { get; protected set; }

    /// <inheritdoc/>
    [DataMember]
    public virtual DateTimeOffset Timestamp { get; protected set; }

    /// <inheritdoc/>
    [DataMember]
    public virtual string Type { get; protected set; } = null!;

    /// <inheritdoc/>
    [DataMember]
    public virtual object? Data { get; protected set; }

    /// <inheritdoc/>
    [DataMember]
    public virtual IDictionary<string, object>? Metadata { get; protected set; }

}
