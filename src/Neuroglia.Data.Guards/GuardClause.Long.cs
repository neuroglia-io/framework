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

namespace Neuroglia.Data.Guards;

/// <summary>
/// Defines <see cref="long"/>-related guard clauses
/// </summary>
public static class LongGuardClause
{

    /// <summary>
    /// Throws when the value is negative
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<long> WhenNegative(this IGuardClause<long> guard) => guard.WhenNegative(GuardExceptionMessages.when_negative);

    /// <summary>
    /// Throws when the value is negative
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="message">The <see cref="Exception"/> message</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<long> WhenNegative(this IGuardClause<long> guard, string message) => guard.WhenNegative(new GuardException(message, guard.ArgumentName));

    /// <summary>
    /// Throws when the value is negative
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<long> WhenNegative(this IGuardClause<long> guard, GuardException ex)
    {
        if (guard.Value < 0) throw ex;
        return guard;
    }

    /// <summary>
    /// Throws when the value is positive
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<long> WhenPositive(this IGuardClause<long> guard) => guard.WhenPositive(GuardExceptionMessages.when_positive);

    /// <summary>
    /// Throws when the value is positive
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="message">The <see cref="Exception"/> message</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<long> WhenPositive(this IGuardClause<long> guard, string message) => guard.WhenPositive(new GuardException(message, guard.ArgumentName));

    /// <summary>
    /// Throws when the value is positive
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<long> WhenPositive(this IGuardClause<long> guard, GuardException ex)
    {
        if (guard.Value > 0) throw ex;
        return guard;
    }

    /// <summary>
    /// Throws when the value is lower than a specified long
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="minimum">The minimum value allowed</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<long> WhenLowerThan(this IGuardClause<long> guard, long minimum) => guard.WhenLowerThan(minimum, StringFormatter.Format(GuardExceptionMessages.when_lower_than, minimum));

    /// <summary>
    /// Throws when the value is lower than a specified long
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="minimum">The minimum value allowed</param>
    /// <param name="message">The <see cref="Exception"/> message</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<long> WhenLowerThan(this IGuardClause<long> guard, long minimum, string message) => guard.WhenLowerThan(minimum, new GuardException(message, guard.ArgumentName));

    /// <summary>
    /// Throws when the value is lower than a specified long
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="minimum">The minimum value allowed</param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<long> WhenLowerThan(this IGuardClause<long> guard, long minimum, GuardException ex)
    {
        if (guard.Value < minimum) throw ex;
        return guard;
    }

    /// <summary>
    /// Throws when the value is lower or equal to a specified long
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="minimum">The minimum value allowed</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<long> WhenLowerOrEqualTo(this IGuardClause<long> guard, long minimum) => guard.WhenLowerOrEqualTo(minimum, StringFormatter.Format(GuardExceptionMessages.when_lower_than, minimum));

    /// <summary>
    /// Throws when the value is lower or equal to a specified long
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="minimum">The minimum value allowed</param>
    /// <param name="message">The <see cref="Exception"/> message</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<long> WhenLowerOrEqualTo(this IGuardClause<long> guard, long minimum, string message) => guard.WhenLowerOrEqualTo(minimum, new GuardException(message, guard.ArgumentName));

    /// <summary>
    /// Throws when the value is lower or equal to a specified long
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="minimum">The minimum value allowed</param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<long> WhenLowerOrEqualTo(this IGuardClause<long> guard, long minimum, GuardException ex)
    {
        if (guard.Value <= minimum) throw ex;
        return guard;
    }

    /// <summary>
    /// Throws when the value is higher than a specified long
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="maximum">The maximum value allowed</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<long> WhenHigherThan(this IGuardClause<long> guard, long maximum) => guard.WhenHigherThan(maximum, StringFormatter.Format(GuardExceptionMessages.when_higher_than, maximum));

    /// <summary>
    /// Throws when the value is higher than a specified long
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="maximum">The maximum value allowed</param>
    /// <param name="message">The <see cref="Exception"/> message</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<long> WhenHigherThan(this IGuardClause<long> guard, long maximum, string message) => guard.WhenHigherThan(maximum, new GuardException(message, guard.ArgumentName));

    /// <summary>
    /// Throws when the value is higher than a specified long
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="maximum">The maximum value allowed</param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<long> WhenHigherThan(this IGuardClause<long> guard, long maximum, GuardException ex)
    {
        if (guard.Value > maximum) throw ex;
        return guard;
    }

    /// <summary>
    /// Throws when the value is higher or equal to a specified long
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="maximum">The maximum value allowed</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<long> WhenHigherOrEqualTo(this IGuardClause<long> guard, long maximum) => guard.WhenHigherOrEqualTo(maximum, StringFormatter.Format(GuardExceptionMessages.when_higher_than, maximum));

    /// <summary>
    /// Throws when the value is higher or equal to a specified long
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="maximum">The maximum value allowed</param>
    /// <param name="message">The <see cref="Exception"/> message</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<long> WhenHigherOrEqualTo(this IGuardClause<long> guard, long maximum, string message) => guard.WhenHigherOrEqualTo(maximum, new GuardException(message, guard.ArgumentName));

    /// <summary>
    /// Throws when the value is higher or equal to a specified long
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="maximum">The maximum value allowed</param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<long> WhenHigherOrEqualTo(this IGuardClause<long> guard, long maximum, GuardException ex)
    {
        if (guard.Value >= maximum) throw ex;
        return guard;
    }

    /// <summary>
    /// Throws when the value falls within a specified range
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="minimum">The exclusive lower bounds of the range</param>
    /// <param name="maximum">The exclusive upper bounds of the range</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<long> WhenWithinRange(this IGuardClause<long> guard, long minimum, long maximum) => guard.WhenWithinRange(minimum, maximum, StringFormatter.Format(GuardExceptionMessages.when_within_range, minimum, maximum));

    /// <summary>
    /// Throws when the value falls within a specified range
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="minimum">The exclusive lower bounds of the range</param>
    /// <param name="maximum">The exclusive upper bounds of the range</param>
    /// <param name="message">The <see cref="Exception"/> message</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<long> WhenWithinRange(this IGuardClause<long> guard, long minimum, long maximum, string message) => guard.WhenWithinRange(minimum, maximum, new GuardException(message, guard.ArgumentName));

    /// <summary>
    /// Throws when the value falls within a specified range
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="minimum">The exclusive lower bounds of the range</param>
    /// <param name="maximum">The exclusive upper bounds of the range</param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<long> WhenWithinRange(this IGuardClause<long> guard, long minimum, long maximum, GuardException ex)
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
    public static IGuardClause<long> WhenNotWithinRange(this IGuardClause<long> guard, long minimum, long maximum) => guard.WhenNotWithinRange(minimum, maximum, StringFormatter.Format(GuardExceptionMessages.when_not_within_range, minimum, maximum));

    /// <summary>
    /// Throws when the value does not fall within a specified range
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="minimum">The exclusive lower bounds of the range</param>
    /// <param name="maximum">The exclusive upper bounds of the range</param>
    /// <param name="message">The <see cref="Exception"/> message</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<long> WhenNotWithinRange(this IGuardClause<long> guard, long minimum, long maximum, string message) => guard.WhenNotWithinRange(minimum, maximum, new GuardException(message, guard.ArgumentName));

    /// <summary>
    /// Throws when the value does not fall within a specified range
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="minimum">The exclusive lower bounds of the range</param>
    /// <param name="maximum">The exclusive upper bounds of the range</param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<long> WhenNotWithinRange(this IGuardClause<long> guard, long minimum, long maximum, GuardException ex)
    {
        if (guard.Value < minimum || guard.Value > maximum) throw ex;
        return guard;
    }

}