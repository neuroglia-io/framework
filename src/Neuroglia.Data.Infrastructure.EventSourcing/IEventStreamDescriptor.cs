namespace Neuroglia.Data.Infrastructure.EventSourcing;

/// <summary>
/// Defines the fundamentals of a service used to describe a stream of events
/// </summary>
public interface IEventStreamDescriptor
    : IIdentifiable
{

    /// <summary>
    /// Gets the stream's length, or events count
    /// </summary>
    long Length { get; }

    /// <summary>
    /// Gets the date and time at which the first event has been created
    /// </summary>
    DateTimeOffset? FirstEventAt { get; }

    /// <summary>
    /// Gets the date and time at which the last event has been created
    /// </summary>
    DateTimeOffset? LastEventAt { get; }

}
