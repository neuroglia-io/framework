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
/// Defines <see cref="DateTime"/>-related guard clauses
/// </summary>
public static class DateTimeGuardClause
{

    /// <summary>
    /// Throws when the value is a date/time before the specified one
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="dateTime">The date and time after which the guarded value must be</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<DateTime> WhenBefore(this IGuardClause<DateTime> guard, DateTime dateTime) => guard.WhenBefore(dateTime, StringFormatter.Format(GuardExceptionMessages.when_before, dateTime));

    /// <summary>
    /// Throws when the value is a date/time before the specified one
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="dateTime">The date and time after which the guarded value must be</param>
    /// <param name="message">The <see cref="Exception"/> message</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<DateTime> WhenBefore(this IGuardClause<DateTime> guard, DateTime dateTime, string message) => guard.WhenBefore(dateTime, new GuardException(message, guard.ArgumentName));

    /// <summary>
    /// Throws when the value is a date/time before the specified one
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="dateTime">The date and time after which the guarded value must be</param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<DateTime> WhenBefore(this IGuardClause<DateTime> guard, DateTime dateTime, GuardException ex)
    {
        if (guard.Value < dateTime) throw ex;
        return guard;
    }

    /// <summary>
    /// Throws when the value is a date/time after the specified one
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="dateTime">The date and time before which the guarded value must be</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<DateTime> WhenAfter(this IGuardClause<DateTime> guard, DateTime dateTime) => guard.WhenAfter(dateTime, StringFormatter.Format(GuardExceptionMessages.when_after, dateTime));

    /// <summary>
    /// Throws when the value is a date/time after the specified one
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="dateTime">The date and time before which the guarded value must be</param>
    /// <param name="message">The <see cref="Exception"/> message</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<DateTime> WhenAfter(this IGuardClause<DateTime> guard, DateTime dateTime, string message) => guard.WhenAfter(dateTime, new GuardException(message, guard.ArgumentName));

    /// <summary>
    /// Throws when the value is a date/time after the specified one
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="dateTime">The date and time before which the guarded value must be</param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<DateTime> WhenAfter(this IGuardClause<DateTime> guard, DateTime dateTime, GuardException ex)
    {
        if (guard.Value > dateTime) throw ex;
        return guard;
    }

}
