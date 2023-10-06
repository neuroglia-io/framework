using System.Runtime.Serialization;

namespace Neuroglia.Data.Infrastructure.EventSourcing;

/// <summary>
/// Represents the default implementation of the <see cref="IEventStreamDescriptor"/> interface
/// </summary>
[DataContract]
public record EventStreamDescriptor
    : IEventStreamDescriptor
{

    /// <summary>
    /// Initializes a new <see cref="EventStreamDescriptor"/>
    /// </summary>
    /// <param name="id">The id of the described stream</param>
    /// <param name="length">The length of the stream</param>
    /// <param name="firstEventAt">The date and time at which the first event of the stream has been created</param>
    /// <param name="lastEventAt">The date and time at which the last event of the stream has been created</param>
    public EventStreamDescriptor(object id, long length, DateTime firstEventAt, DateTime lastEventAt)
    {
        this.Id = id;
        this.Length = length;
        this.FirstEventAt = firstEventAt;
        this.LastEventAt = lastEventAt;
    }

    /// <inheritdoc/>
    [DataMember]
    public virtual object Id { get; }

    /// <inheritdoc/>
    [DataMember]
    public virtual long Length { get; }

    /// <inheritdoc/>
    [DataMember]
    public virtual DateTimeOffset FirstEventAt { get; }

    /// <inheritdoc/>
    [DataMember]
    public virtual DateTimeOffset LastEventAt { get; }

}