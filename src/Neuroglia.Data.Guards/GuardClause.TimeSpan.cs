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
/// Defines <see cref="TimeSpan"/>-related guard clauses
/// </summary>
public static class TimeSpanGuardClause
{

    /// <summary>
    /// Throws when the value is lower than the specified one
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="timeSpan">The timespan the guarded value must be higher than</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TimeSpan> WhenLowerThan(this IGuardClause<TimeSpan> guard, TimeSpan timeSpan) => guard.WhenLowerThan(timeSpan, StringFormatter.Format(GuardExceptionMessages.when_lower_than, timeSpan));

    /// <summary>
    /// Throws when the value is lower than the specified one
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="timeSpan">The timespan the guarded value must be higher than</param>
    /// <param name="message">The <see cref="Exception"/> message</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TimeSpan> WhenLowerThan(this IGuardClause<TimeSpan> guard, TimeSpan timeSpan, string message) => guard.WhenLowerThan(timeSpan, new GuardException(message, guard.ArgumentName));

    /// <summary>
    /// Throws when the value is lower than the specified one
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="timeSpan">The timespan the guarded value must be higher than</param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TimeSpan> WhenLowerThan(this IGuardClause<TimeSpan> guard, TimeSpan timeSpan, GuardException ex)
    {
        if (guard.Value < timeSpan) throw ex;
        return guard;
    }

    /// <summary>
    /// Throws when the value is lower or equals to the specified one
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="timeSpan">The timespan the guarded value must be higher or equal to</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TimeSpan> WhenLowerOrEqualsTo(this IGuardClause<TimeSpan> guard, TimeSpan timeSpan) => guard.WhenLowerOrEqualsTo(timeSpan, StringFormatter.Format(GuardExceptionMessages.when_lower_than, timeSpan));

    /// <summary>
    /// Throws when the value is lower or equals to the specified one
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="timeSpan">The timespan the guarded value must be higher or equal to</param>
    /// <param name="message">The <see cref="Exception"/> message</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TimeSpan> WhenLowerOrEqualsTo(this IGuardClause<TimeSpan> guard, TimeSpan timeSpan, string message) => guard.WhenLowerOrEqualsTo(timeSpan, new GuardException(message, guard.ArgumentName));

    /// <summary>
    /// Throws when the value is lower or equals to the specified one
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="timeSpan">The timespan the guarded value must be higher or equal to</param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TimeSpan> WhenLowerOrEqualsTo(this IGuardClause<TimeSpan> guard, TimeSpan timeSpan, GuardException ex)
    {
        if (guard.Value <= timeSpan) throw ex;
        return guard;
    }

    /// <summary>
    /// Throws when the value is higher than the specified one
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="timeSpan">The timespan the guarded value must be higher than</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TimeSpan> WhenHigherThan(this IGuardClause<TimeSpan> guard, TimeSpan timeSpan) => guard.WhenHigherThan(timeSpan, StringFormatter.Format(GuardExceptionMessages.when_higher_than, timeSpan));

    /// <summary>
    /// Throws when the value is higher than the specified one
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="timeSpan">The timespan the guarded value must be higher than</param>
    /// <param name="message">The <see cref="Exception"/> message</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TimeSpan> WhenHigherThan(this IGuardClause<TimeSpan> guard, TimeSpan timeSpan, string message) => guard.WhenHigherThan(timeSpan, new GuardException(message, guard.ArgumentName));

    /// <summary>
    /// Throws when the value is higher than the specified one
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="timeSpan">The timespan the guarded value must be higher than</param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TimeSpan> WhenHigherThan(this IGuardClause<TimeSpan> guard, TimeSpan timeSpan, GuardException ex)
    {
        if (guard.Value > timeSpan) throw ex;
        return guard;
    }

    /// <summary>
    /// Throws when the value is higher or equals to the specified one
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="timeSpan">The timespan the guarded value must be higher or equal to</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TimeSpan> WhenHigherOrEqualsTo(this IGuardClause<TimeSpan> guard, TimeSpan timeSpan) => guard.WhenHigherOrEqualsTo(timeSpan, GuardExceptionMessages.when_higher_than);

    /// <summary>
    /// Throws when the value is higher or equals to the specified one
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="timeSpan">The timespan the guarded value must be higher or equal to</param>
    /// <param name="message">The <see cref="Exception"/> message</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TimeSpan> WhenHigherOrEqualsTo(this IGuardClause<TimeSpan> guard, TimeSpan timeSpan, string message) => guard.WhenHigherOrEqualsTo(timeSpan, new GuardException(message, guard.ArgumentName));

    /// <summary>
    /// Throws when the value is higher or equals to the specified one
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="timeSpan">The timespan the guarded value must be higher or equal to</param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TimeSpan> WhenHigherOrEqualsTo(this IGuardClause<TimeSpan> guard, TimeSpan timeSpan, GuardException ex)
    {
        if (guard.Value >= timeSpan) throw ex;
        return guard;
    }

}
