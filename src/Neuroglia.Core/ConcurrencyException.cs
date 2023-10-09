namespace Neuroglia;

/// <summary>
/// Represents the <see cref="Exception"/> thrown when a concurrency error occurs
/// </summary>
public class ConcurrencyException
     : Exception
{

    /// <summary>
    /// Initializes a new <see cref="ConcurrencyException"/>
    /// </summary>
    public ConcurrencyException() { }

    /// <summary>
    /// Initializes a new <see cref="ConcurrencyException"/>
    /// </summary>
    /// <param name="message">The exception message</param>
    public ConcurrencyException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new <see cref="ConcurrencyException"/>
    /// </summary>
    /// <param name="message">The exception message</param>
    /// <param name="innerException">The inner exception</param>
    public ConcurrencyException(string message, Exception innerException) : base(message, innerException) { }

}
