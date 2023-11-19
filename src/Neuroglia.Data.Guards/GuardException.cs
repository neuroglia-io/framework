namespace Neuroglia.Data.Guards;

/// <summary>
/// Represents an exception thrown due to invalid data that has been guarded against
/// </summary>
/// <remarks>
/// Initializes a new <see cref="GuardException"/>
/// </remarks>
/// <param name="message">The <see cref="GuardException"/>'s message</param>
public class GuardException(string? message, string? argumentName = null)
    : Exception(message)
{

    /// <summary>
    /// Gets the name of the argument, if any, that has been guarded against
    /// </summary>
    public virtual string? ArgumentName { get; } = argumentName;

}