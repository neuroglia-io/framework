namespace Neuroglia;

/// <summary>
/// Represents the <see cref="Exception"/> thrown when a pessimistic concurrency error occurs
/// </summary>
public class PessimisticConcurrencyException
    : ConcurrencyException
{

    /// <summary>
    /// Initializes a new <see cref="PessimisticConcurrencyException"/>
    /// </summary>
    public PessimisticConcurrencyException() { }

    /// <summary>
    /// Initializes a new <see cref="PessimisticConcurrencyException"/>
    /// </summary>
    /// <param name="message">The exception message</param>
    public PessimisticConcurrencyException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new <see cref="PessimisticConcurrencyException"/>
    /// </summary>
    /// <param name="message">The exception message</param>
    /// <param name="innerException">The inner exception</param>
    public PessimisticConcurrencyException(string message, Exception innerException) : base(message, innerException) { }

}