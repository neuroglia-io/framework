// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License"),
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Neuroglia.Data.Guards.Properties;
using System.Text.RegularExpressions;

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
    public static IGuardClause<string> WhenNullOrWhitespace(this IGuardClause<string> guard) => guard.WhenNullOrWhitespace(GuardExceptionMessages.when_null_or_whitespace);

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
    public static IGuardClause<string> WhenStartsWith(this IGuardClause<string> guard, string prefix, StringComparison comparisonType = StringComparison.CurrentCulture) => guard.WhenStartsWith(prefix, StringFormatter.Format(GuardExceptionMessages.when_starts_with, prefix), comparisonType);

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
    public static IGuardClause<string> WhenNotStartsWith(this IGuardClause<string> guard, string prefix, StringComparison comparisonType = StringComparison.CurrentCulture) => guard.WhenNotStartsWith(prefix, StringFormatter.Format(GuardExceptionMessages.when_not_starts_with, prefix), comparisonType);

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
    /// <param name="suffix">The prefix the value must not end with</param>
    /// <param name="comparisonType">The type of comparison to perform</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<string> WhenEndsWith(this IGuardClause<string> guard, string suffix, StringComparison comparisonType = StringComparison.CurrentCulture) => guard.WhenEndsWith(suffix, StringFormatter.Format(GuardExceptionMessages.when_ends_with, suffix), comparisonType);

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
    /// <param name="suffix">The prefix the value must not end with</param>
    /// <param name="comparisonType">The type of comparison to perform</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<string> WhenNotEndsWith(this IGuardClause<string> guard, string suffix, StringComparison comparisonType = StringComparison.CurrentCulture) => guard.WhenNotEndsWith(suffix, StringFormatter.Format(GuardExceptionMessages.when_not_ends_with, suffix), comparisonType);

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
    public static IGuardClause<string> WhenEquivalentTo(this IGuardClause<string> guard, string value) => guard.WhenEquivalentTo(value, StringFormatter.Format(GuardExceptionMessages.when_equivalent_to, value));

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
    public static IGuardClause<string> WhenNotEquivalentTo(this IGuardClause<string> guard, string value) => guard.WhenNotEquivalentTo(value, StringFormatter.Format(GuardExceptionMessages.when_not_equivalent_to, value));

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
    public static IGuardClause<string> WhenContains(this IGuardClause<string> guard, string value, StringComparison comparisonType = StringComparison.CurrentCulture) => guard.WhenContains(value, StringFormatter.Format(GuardExceptionMessages.when_contains, value), comparisonType);

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
    public static IGuardClause<string> WhenNotContains(this IGuardClause<string> guard, string value, StringComparison comparisonType = StringComparison.CurrentCulture) => guard.WhenNotContains(value, StringFormatter.Format(GuardExceptionMessages.when_not_contains, value), comparisonType);

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

    /// <summary>
    /// Throws when the value matches the specified regular expression
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="pattern">The regular expression pattern the value must not match</param>
    /// <returns>The configured <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<string> WhenMatches(this IGuardClause<string> guard, string pattern) => guard.WhenMatches(pattern, StringFormatter.Format(GuardExceptionMessages.when_matches, pattern));

    /// <summary>
    /// Throws when the value matches the specified regular expression
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="pattern">The regular expression pattern the value must not match</param>
    /// <param name="message">The exception message</param>
    /// <returns>The configured <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<string> WhenMatches(this IGuardClause<string> guard, string pattern, string message) => guard.WhenMatches(pattern, new GuardException(message, guard.ArgumentName));

    /// <summary>
    /// Throws when the value matches the specified regular expression
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="pattern">The regular expression pattern the value must not match</param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <returns>The configured <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<string> WhenMatches(this IGuardClause<string> guard, string pattern, GuardException ex)
    {
        if (string.IsNullOrWhiteSpace(guard.Value) || Regex.IsMatch(guard.Value, pattern)) throw ex;
        return guard;
    }

    /// <summary>
    /// Throws when the value does not match the specified regular expression
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="pattern">The regular expression pattern the value must match</param>
    /// <returns>The configured <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<string> WhenNotMatches(this IGuardClause<string> guard, string pattern) => guard.WhenNotMatches(pattern, StringFormatter.Format(GuardExceptionMessages.when_not_matches, pattern));

    /// <summary>
    /// Throws when the value does not match the specified regular expression
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="pattern">The regular expression pattern the value must match</param>
    /// <param name="message">The exception message</param>
    /// <returns>The configured <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<string> WhenNotMatches(this IGuardClause<string> guard, string pattern, string message) => guard.WhenNotMatches(pattern, new GuardException(message, guard.ArgumentName));

    /// <summary>
    /// Throws when the value does not match the specified regular expression
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="pattern">The regular expression pattern the value must match</param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <returns>The configured <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<string> WhenNotMatches(this IGuardClause<string> guard, string pattern, GuardException ex)
    {
        if (string.IsNullOrWhiteSpace(guard.Value) || !Regex.IsMatch(guard.Value, pattern)) throw ex;
        return guard;
    }

}
