namespace Neuroglia.Data.Guards;

/// <summary>
/// Defines string-related guard clauses
/// </summary>
public static class StringGuardClauses
{

    /// <summary>
    /// Throws when the value is null or whitespace
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<string> WhenNullOrWhitespace(this IGuardClause<string> guard) => guard.WhenNullOrWhitespace("The specified value cannot be null or whitespace");

    /// <summary>
    /// Throws when the value is null or whitespace
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="message">The exception message</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<string> WhenNullOrWhitespace(this IGuardClause<string> guard, string message) => guard.WhenNullOrWhitespace(new GuardException(message, guard.ArgumentName));

    /// <summary>
    /// Throws when the value is null or whitespace
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<string> WhenNullOrWhitespace(this IGuardClause<string> guard, GuardException ex)
    {
        if (string.IsNullOrWhiteSpace(guard.Value)) throw ex;
        return guard;
    }

    /// <summary>
    /// Throws when the value starts with the specified string
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="prefix">The prefix the value must not start with</param>
    /// <param name="comparisonType">The type of comparison to perform</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<string> WhenStartsWith(this IGuardClause<string> guard, string prefix, StringComparison comparisonType = StringComparison.CurrentCulture) => guard.WhenStartsWith(prefix, $"The specified value must not start with '{prefix}'", comparisonType);

    /// <summary>
    /// Throws when the value starts with the specified string
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="prefix">The prefix the value must not start with</param>
    /// <param name="comparisonType">The type of comparison to perform</param>
    /// <param name="message">The <see cref="Exception"/> message</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<string> WhenStartsWith(this IGuardClause<string> guard, string prefix, string message, StringComparison comparisonType = StringComparison.CurrentCulture) => guard.WhenStartsWith(prefix, new GuardException(message, guard.ArgumentName), comparisonType);

    /// <summary>
    /// Throws when the value starts with the specified string
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="prefix">The prefix the value must not start with</param>
    /// <param name="comparisonType">The type of comparison to perform</param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<string> WhenStartsWith(this IGuardClause<string> guard, string prefix, GuardException ex, StringComparison comparisonType = StringComparison.CurrentCulture)
    {
        if (string.IsNullOrWhiteSpace(guard.Value) || guard.Value.StartsWith(prefix, comparisonType)) throw ex;
        return guard;
    }

    /// <summary>
    /// Throws when the value does not start with the specified string
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="prefix">The prefix the value must not start with</param>
    /// <param name="comparisonType">The type of comparison to perform</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<string> WhenNotStartsWith(this IGuardClause<string> guard, string prefix, StringComparison comparisonType = StringComparison.CurrentCulture) => guard.WhenNotStartsWith(prefix, $"The specified value must start with '{prefix}'", comparisonType);

    /// <summary>
    /// Throws when the value does not start with the specified string
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="prefix">The prefix the value must not start with</param>
    /// <param name="message">The <see cref="Exception"/> message</param>
    /// <param name="comparisonType">The type of comparison to perform</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<string> WhenNotStartsWith(this IGuardClause<string> guard, string prefix, string message, StringComparison comparisonType = StringComparison.CurrentCulture) => guard.WhenNotStartsWith(prefix, new GuardException(message, guard.ArgumentName), comparisonType);

    /// <summary>
    /// Throws when the value does not start with the specified string
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="prefix">The prefix the value must not start with</param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <param name="comparisonType">The type of comparison to perform</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<string> WhenNotStartsWith(this IGuardClause<string> guard, string prefix, GuardException ex, StringComparison comparisonType = StringComparison.CurrentCulture)
    {
        if (string.IsNullOrWhiteSpace(guard.Value) || !guard.Value.StartsWith(prefix, comparisonType)) throw ex;
        return guard;
    }

    /// <summary>
    /// Throws when the value ends with the specified string
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="prefix">The prefix the value must not end with</param>
    /// <param name="comparisonType">The type of comparison to perform</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<string> WhenEndsWith(this IGuardClause<string> guard, string prefix, StringComparison comparisonType = StringComparison.CurrentCulture) => guard.WhenEndsWith(prefix, $"The specified value must not end with '{prefix}'", comparisonType);

    /// <summary>
    /// Throws when the value ends with the specified string
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="prefix">The prefix the value must not end with</param>
    /// <param name="message">The <see cref="Exception"/> message</param>
    /// <param name="comparisonType">The type of comparison to perform</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<string> WhenEndsWith(this IGuardClause<string> guard, string prefix, string message, StringComparison comparisonType = StringComparison.CurrentCulture) => guard.WhenEndsWith(prefix, new GuardException(message, guard.ArgumentName), comparisonType);

    /// <summary>
    /// Throws when the value ends with the specified string
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="prefix">The prefix the value must not end with</param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <param name="comparisonType">The type of comparison to perform</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<string> WhenEndsWith(this IGuardClause<string> guard, string prefix, GuardException ex, StringComparison comparisonType = StringComparison.CurrentCulture)
    {
        if (string.IsNullOrWhiteSpace(guard.Value) || guard.Value.EndsWith(prefix, comparisonType)) throw ex;
        return guard;
    }

    /// <summary>
    /// Throws when the value does not end with the specified string
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="prefix">The prefix the value must not end with</param>
    /// <param name="comparisonType">The type of comparison to perform</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<string> WhenNotEndsWith(this IGuardClause<string> guard, string prefix, StringComparison comparisonType = StringComparison.CurrentCulture) => guard.WhenNotEndsWith(prefix, $"The specified value must end with '{prefix}'", comparisonType);

    /// <summary>
    /// Throws when the value does not end with the specified string
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="prefix">The prefix the value must not end with</param>
    /// <param name="message">The <see cref="Exception"/> message</param>
    /// <param name="comparisonType">The type of comparison to perform</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<string> WhenNotEndsWith(this IGuardClause<string> guard, string prefix, string message, StringComparison comparisonType = StringComparison.CurrentCulture) => guard.WhenNotEndsWith(prefix, new GuardException(message, guard.ArgumentName), comparisonType);

    /// <summary>
    /// Throws when the value does not end with the specified string
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="prefix">The prefix the value must not end with</param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <param name="comparisonType">The type of comparison to perform</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<string> WhenNotEndsWith(this IGuardClause<string> guard, string prefix, GuardException ex, StringComparison comparisonType = StringComparison.CurrentCulture)
    {
        if (string.IsNullOrWhiteSpace(guard.Value) || !guard.Value.EndsWith(prefix, comparisonType)) throw ex;
        return guard;
    }

    /// <summary>
    /// Throws when the value is equivalent to the specified string
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="value">The string the value must not be equivalent to</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<string> WhenEquivalentTo(this IGuardClause<string> guard, string value) => guard.WhenEquivalentTo(value, $"The specified value must not be equivalent to '{value}'");

    /// <summary>
    /// Throws when the value is equivalent to the specified string
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="value">The string the value must not be equivalent to</param>
    /// <param name="message">The <see cref="Exception"/> message</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<string> WhenEquivalentTo(this IGuardClause<string> guard, string value, string message) => guard.WhenEquivalentTo(value, new GuardException(message, guard.ArgumentName));

    /// <summary>
    /// Throws when the value is equivalent to the specified string
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="value">The string the value must not be equivalent to</param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<string> WhenEquivalentTo(this IGuardClause<string> guard, string value, GuardException ex)
    {
        if (string.IsNullOrWhiteSpace(guard.Value) || guard.Value.Equals(value, StringComparison.OrdinalIgnoreCase)) throw ex;
        return guard;
    }

    /// <summary>
    /// Throws when the value is not equivalent to the specified string
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="value">The string the value must be equivalent to</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<string> WhenNotEquivalentTo(this IGuardClause<string> guard, string value) => guard.WhenNotEquivalentTo(value, $"The specified value must be equivalent to '{value}'");

    /// <summary>
    /// Throws when the value is not equivalent to the specified string
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="value">The string the value must be equivalent to</param>
    /// <param name="message">The <see cref="Exception"/> message</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<string> WhenNotEquivalentTo(this IGuardClause<string> guard, string value, string message) => guard.WhenNotEquivalentTo(value, new GuardException(message, guard.ArgumentName));

    /// <summary>
    /// Throws when the value is not equivalent to the specified string
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="value">The string the value must be equivalent to</param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<string> WhenNotEquivalentTo(this IGuardClause<string> guard, string value, GuardException ex)
    {
        if (string.IsNullOrWhiteSpace(guard.Value) || !guard.Value.Equals(value, StringComparison.OrdinalIgnoreCase)) throw ex;
        return guard;
    }

    /// <summary>
    /// Throws when the value contains the specified string
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="value">The item the value must contain</param>
    /// <param name="comparisonType">The type of comparison to perform</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<string> WhenContains(this IGuardClause<string> guard, string value, StringComparison comparisonType = StringComparison.CurrentCulture) => guard.WhenContains(value, $"The specified value must not contain '{value}'", comparisonType);

    /// <summary>
    /// Throws when the value contains a specific item
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="value">The item the value must contain</param>
    /// <param name="message">The <see cref="Exception"/> message</param>
    /// <param name="comparisonType">The type of comparison to perform</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<string> WhenContains(this IGuardClause<string> guard, string value, string message, StringComparison comparisonType = StringComparison.CurrentCulture) => guard.WhenContains(value, new GuardException(message, guard.ArgumentName), comparisonType);

    /// <summary>
    /// Throws when the value contains a specific item
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="value">The item the value must contain</param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <param name="comparisonType">The type of comparison to perform</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<string> WhenContains(this IGuardClause<string> guard, string value, GuardException ex, StringComparison comparisonType = StringComparison.CurrentCulture)
    {
        if (guard.Value == null || (value != null && guard.Value.Contains(value!, comparisonType))) throw ex;
        return guard;
    }

    /// <summary>
    /// Throws when the value does not contain the specified string
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="value">The item the value must contain</param>
    /// <param name="comparisonType">The type of comparison to perform</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<string> WhenNotContains(this IGuardClause<string> guard, string value, StringComparison comparisonType = StringComparison.CurrentCulture) => guard.WhenNotContains(value, $"The specified value must not contain '{value}'", comparisonType);

    /// <summary>
    /// Throws when the value does not contain a specific item
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="value">The item the value must contain</param>
    /// <param name="message">The <see cref="Exception"/> message</param>
    /// <param name="comparisonType">The type of comparison to perform</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<string> WhenNotContains(this IGuardClause<string> guard, string value, string message, StringComparison comparisonType = StringComparison.CurrentCulture) => guard.WhenNotContains(value, new GuardException(message, guard.ArgumentName), comparisonType);

    /// <summary>
    /// Throws when the value does not contain a specific item
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="value">The item the value must contain</param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <param name="comparisonType">The type of comparison to perform</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<string> WhenNotContains(this IGuardClause<string> guard, string value, GuardException ex, StringComparison comparisonType = StringComparison.CurrentCulture)
    {
        if (guard.Value == null || value == null || !guard.Value.Contains(value!, comparisonType)) throw ex;
        return guard;
    }

}
