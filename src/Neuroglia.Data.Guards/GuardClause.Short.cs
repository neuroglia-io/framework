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

namespace Neuroglia.Data.Guards;

/// <summary>
/// Defines <see cref="short"/>-related guard clauses
/// </summary>
public static class ShortGuardClause
{

    /// <summary>
    /// Throws when the value is negative
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<short> WhenNegative(this IGuardClause<short> guard) => guard.WhenNegative("The value must not be negative");

    /// <summary>
    /// Throws when the value is negative
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="message">The <see cref="Exception"/> message</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<short> WhenNegative(this IGuardClause<short> guard, string message) => guard.WhenNegative(new GuardException(message));

    /// <summary>
    /// Throws when the value is negative
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<short> WhenNegative(this IGuardClause<short> guard, GuardException ex)
    {
        if (guard.Value < 0) throw ex;
        return guard;
    }

    /// <summary>
    /// Throws when the value is positive
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<short> WhenPositive(this IGuardClause<short> guard) => guard.WhenPositive("The value must not be positive");

    /// <summary>
    /// Throws when the value is positive
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="message">The <see cref="Exception"/> message</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<short> WhenPositive(this IGuardClause<short> guard, string message) => guard.WhenPositive(new GuardException(message));

    /// <summary>
    /// Throws when the value is positive
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<short> WhenPositive(this IGuardClause<short> guard, GuardException ex)
    {
        if (guard.Value > 0) throw ex;
        return guard;
    }

    /// <summary>
    /// Throws when the value is lower than a specified short
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="minimum">The minimum value allowed</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<short> WhenLowerThan(this IGuardClause<short> guard, short minimum) => guard.WhenLowerThan(minimum, $"The value must be higher or equal to '{minimum}'");

    /// <summary>
    /// Throws when the value is lower than a specified short
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="minimum">The minimum value allowed</param>
    /// <param name="message">The <see cref="Exception"/> message</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<short> WhenLowerThan(this IGuardClause<short> guard, short minimum, string message) => guard.WhenLowerThan(minimum, new GuardException(message));

    /// <summary>
    /// Throws when the value is lower than a specified short
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="minimum">The minimum value allowed</param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<short> WhenLowerThan(this IGuardClause<short> guard, short minimum, GuardException ex)
    {
        if (guard.Value < minimum) throw ex;
        return guard;
    }

    /// <summary>
    /// Throws when the value is higher than a specified short
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="maximum">The maximum value allowed</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<short> WhenHigherThan(this IGuardClause<short> guard, short maximum) => guard.WhenHigherThan(maximum, $"The value must be lower or equal to '{maximum}'");

    /// <summary>
    /// Throws when the value is higher than a specified short
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="maximum">The maximum value allowed</param>
    /// <param name="message">The <see cref="Exception"/> message</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<short> WhenHigherThan(this IGuardClause<short> guard, short maximum, string message) => guard.WhenHigherThan(maximum, new GuardException(message));

    /// <summary>
    /// Throws when the value is higher than a specified short
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="maximum">The maximum value allowed</param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<short> WhenHigherThan(this IGuardClause<short> guard, short maximum, GuardException ex)
    {
        if (guard.Value > maximum) throw ex;
        return guard;
    }

    /// <summary>
    /// Throws when the value falls within a specified range
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="minimum">The exclusive lower bounds of the range</param>
    /// <param name="maximum">The exclusive upper bounds of the range</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<short> WhenWithinRange(this IGuardClause<short> guard, short minimum, short maximum) => guard.WhenWithinRange(minimum, maximum, $"The value should not fall within a range beginning at '{minimum}' and extending up to '{maximum}'");

    /// <summary>
    /// Throws when the value falls within a specified range
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="minimum">The exclusive lower bounds of the range</param>
    /// <param name="maximum">The exclusive upper bounds of the range</param>
    /// <param name="message">The <see cref="Exception"/> message</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<short> WhenWithinRange(this IGuardClause<short> guard, short minimum, short maximum, string message) => guard.WhenWithinRange(minimum, maximum, new GuardException(message));

    /// <summary>
    /// Throws when the value falls within a specified range
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="minimum">The exclusive lower bounds of the range</param>
    /// <param name="maximum">The exclusive upper bounds of the range</param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<short> WhenWithinRange(this IGuardClause<short> guard, short minimum, short maximum, GuardException ex)
    {
        if (guard.Value > minimum && guard.Value < maximum) throw ex;
        return guard;
    }

    /// <summary>
    /// Throws when the value does not fall within a specified range
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="minimum">The exclusive lower bounds of the range</param>
    /// <param name="maximum">The exclusive upper bounds of the range</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<short> WhenNotWithinRange(this IGuardClause<short> guard, short minimum, short maximum) => guard.WhenNotWithinRange(minimum, maximum, $"The value should fall within a range beginning at '{minimum}' and extending up to '{maximum}'");

    /// <summary>
    /// Throws when the value does not fall within a specified range
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="minimum">The exclusive lower bounds of the range</param>
    /// <param name="maximum">The exclusive upper bounds of the range</param>
    /// <param name="message">The <see cref="Exception"/> message</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<short> WhenNotWithinRange(this IGuardClause<short> guard, short minimum, short maximum, string message) => guard.WhenNotWithinRange(minimum, maximum, new GuardException(message));

    /// <summary>
    /// Throws when the value does not fall within a specified range
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="minimum">The exclusive lower bounds of the range</param>
    /// <param name="maximum">The exclusive upper bounds of the range</param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<short> WhenNotWithinRange(this IGuardClause<short> guard, short minimum, short maximum, GuardException ex)
    {
        if (guard.Value < minimum || guard.Value > maximum) throw ex;
        return guard;
    }

}
