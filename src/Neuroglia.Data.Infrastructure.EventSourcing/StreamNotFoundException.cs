namespace Neuroglia.Data.Infrastructure.EventSourcing;

/// <summary>
/// Represents an exception thrown when an event stream could not be found
/// </summary>
public class StreamNotFoundException
    : Exception
{

    /// <summary>
    /// Initializes a new <see cref="StreamNotFoundException"/>
    /// </summary>
    public StreamNotFoundException() : base("Failed to find the specified stream") { }

    /// <summary>
    /// Initializes a new <see cref="StreamNotFoundException"/>
    /// </summary>
    /// <param name="streamId">The id of the stream that could not be found</param>
    public StreamNotFoundException(string streamId) : this() { this.StreamId = streamId; }

    /// <summary>
    /// Gets the id of the stream that could not be found
    /// </summary>
    public virtual string? StreamId { get; protected set; }

}
