namespace Neuroglia;

/// <summary>
/// Represents the <see cref="Exception"/> thrown when an optimistic concurrency error occurs
/// </summary>
public class OptimisticConcurrencyException
    : ConcurrencyException
{

    /// <summary>
    /// Initializes a new <see cref="OptimisticConcurrencyException"/>
    /// </summary>
    public OptimisticConcurrencyException() { }

    /// <summary>
    /// Initializes a new <see cref="OptimisticConcurrencyException"/>
    /// </summary>
    /// <param name="actualVersion">The expected version</param>
    /// <param name="expectedVersion">The actual version</param>
    public OptimisticConcurrencyException(long? expectedVersion, long? actualVersion)
    {
        this.ExpectedVersion = expectedVersion;
        this.ActualVersion = actualVersion;
    }

    /// <summary>
    /// Initializes a new <see cref="OptimisticConcurrencyException"/>
    /// </summary>
    /// <param name="message">The exception message</param>
    public OptimisticConcurrencyException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new <see cref="OptimisticConcurrencyException"/>
    /// </summary>
    /// <param name="message">The exception message</param>
    /// <param name="actualVersion">The expected version</param>
    /// <param name="expectedVersion">The actual version</param>
    public OptimisticConcurrencyException(string message, long? expectedVersion, long? actualVersion)
        : base(message)
    {
        this.ExpectedVersion = expectedVersion;
        this.ActualVersion = actualVersion;
    }

    /// <summary>
    /// Initializes a new <see cref="OptimisticConcurrencyException"/>
    /// </summary>
    /// <param name="message">The exception message</param>
    /// <param name="innerException">The inner exception</param>
    public OptimisticConcurrencyException(string message, Exception innerException) : base(message, innerException) { }

    /// <summary>
    /// Initializes a new <see cref="OptimisticConcurrencyException"/>
    /// </summary>
    /// <param name="message">The exception message</param>
    /// <param name="innerException">The inner exception</param>
    /// <param name="actualVersion">The expected version</param>
    /// <param name="expectedVersion">The actual version</param>
    public OptimisticConcurrencyException(string message, Exception innerException, long expectedVersion, long actualVersion)
        : base(message, innerException)
    {
        this.ExpectedVersion = expectedVersion;
        this.ActualVersion = actualVersion;
    }

    /// <summary>
    /// Gets the expected version
    /// </summary>
    public virtual long? ExpectedVersion { get; protected set; }

    /// <summary>
    /// Gets the actual version
    /// </summary>
    public virtual long? ActualVersion { get; protected set; }

}