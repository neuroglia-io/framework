using System.Runtime.CompilerServices;

namespace Neuroglia.Data.Guards;

/// <summary>
/// Defines methods to guard against invalid data
/// </summary>
public static class Guard
{

    /// <summary>
    /// Guards against the value
    /// </summary>
    /// <typeparam name="T">The type of value to validate</typeparam>
    /// <param name="value">The value to validate</param>
    /// <returns>The configured <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<T> Against<T>(T? value) => new GuardClause<T>(value);

    /// <summary>
    /// Guards against the value
    /// </summary>
    /// <typeparam name="T">The type of value to validate</typeparam>
    /// <param name="value">The value to validate</param>
    /// <param name="argumentName">The name of the argument to validate</param>
    /// <returns>The configured <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<T> AgainstArgument<T>(T value, [CallerArgumentExpression(nameof(value))] string? argumentName = null) => new GuardClause<T>(value, argumentName);

}
