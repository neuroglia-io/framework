namespace Neuroglia.Data.Guards;

/// <summary>
/// Defines <see cref="IEquatable{T}"/>-related guard clauses
/// </summary>
public static class EquatableGuardClauses
{

    /// <summary>
    /// Throws when the value to guard against equals the specified value
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="value">The value to compare to</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<T> WhenEquals<T>(this IGuardClause<T> guard, T value) where T : IEquatable<T> => guard.WhenEquals(value, "");

    /// <summary>
    /// Throws when the value to guard against equals the specified value
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="value">The value to compare to</param>
    /// <param name="message">The exception message</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<T> WhenEquals<T>(this IGuardClause<T> guard, T value, string message) where T : IEquatable<T> => guard.WhenEquals(value, new GuardException(message));

    /// <summary>
    /// Throws when the value to guard against equals the specified value
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="value">The value to compare to</param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<T> WhenEquals<T>(this IGuardClause<T> guard, T value, GuardException ex) where T : IEquatable<T>
    {
        if (guard.Value.Equals(value)) throw ex;
        return guard;
    }

    /// <summary>
    /// Throws when the value to guard against is not equal to the specified value
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="value">The value to compare to</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<T> WhenNotEquals<T>(this IGuardClause<T> guard, T value) where T : IEquatable<T> => guard.WhenNotEquals(value, "");

    /// <summary>
    /// Throws when the value to guard against is not equal to the specified value
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="value">The value to compare to</param>
    /// <param name="message">The exception message</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<T> WhenNotEquals<T>(this IGuardClause<T> guard, T value, string message) where T : IEquatable<T> => guard.WhenNotEquals(value, new GuardException(message));

    /// <summary>
    /// Throws when the value to guard against is not equal to the specified value
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="value">The value to compare to</param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<T> WhenNotEquals<T>(this IGuardClause<T> guard, T value, GuardException ex) where T : IEquatable<T>
    {
        if (!guard.Value.Equals(value)) throw ex;
        return guard;
    }

}
