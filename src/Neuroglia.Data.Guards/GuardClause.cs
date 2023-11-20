namespace Neuroglia.Data.Guards;

/// <summary>
/// Represents the default implementation of the <see cref="IGuardClause{T}"/> interface
/// </summary>
/// <typeparam name="T">The type of value to guard against</typeparam>
/// <remarks>
/// Initializes a new <see cref="GuardClause{T}"/>
/// </remarks>
/// <param name="value">The value to guard against</param>
/// <param name="argumentName">The name of the argument to guard against, if any</param>
public class GuardClause<T>(T value, string? argumentName = null)
    : IGuardClause<T>
{

    /// <inheritdoc/>
    public T Value { get; } = value;

    /// <inheritdoc/>
    public string? ArgumentName { get; } = argumentName;

    /// <inheritdoc/>
    public IGuardClause<T> WhenNull() => this.WhenNull("The specified value cannot be null");

    /// <inheritdoc/>
    public IGuardClause<T> WhenNull(string message) => this.WhenNull(new GuardException(message, this.ArgumentName));

    /// <inheritdoc/>
    public IGuardClause<T> WhenNull(GuardException ex)
    {
        if (this.Value == null) throw ex;
        return this;
    }

    /// <inheritdoc/>
    public IGuardClause<T> WhenNotNull() => this.WhenNotNull("The specified value cannot be null");

    /// <inheritdoc/>
    public IGuardClause<T> WhenNotNull(string message) => this.WhenNotNull(new GuardException(message, this.ArgumentName));

    /// <inheritdoc/>
    public IGuardClause<T> WhenNotNull(GuardException ex)
    {
        if (this.Value != null) throw ex;
        return this;
    }

}
