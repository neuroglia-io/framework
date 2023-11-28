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
    public static IGuardClause<T> WhenEquals<T>(this IGuardClause<T> guard, T value) where T : IEquatable<T> => guard.WhenEquals(value, GuardExceptionMessages.when_equals);

    /// <summary>
    /// Throws when the value to guard against equals the specified value
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="value">The value to compare to</param>
    /// <param name="message">The exception message</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<T> WhenEquals<T>(this IGuardClause<T> guard, T value, string message) where T : IEquatable<T> => guard.WhenEquals(value, new GuardException(message, guard.ArgumentName));

    /// <summary>
    /// Throws when the value to guard against equals the specified value
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="value">The value to compare to</param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<T> WhenEquals<T>(this IGuardClause<T> guard, T value, GuardException ex) where T : IEquatable<T>
    {
        if (guard.Value?.Equals(value) == true) throw ex;
        return guard;
    }

    /// <summary>
    /// Throws when the value to guard against is not equal to the specified value
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="value">The value to compare to</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<T> WhenNotEquals<T>(this IGuardClause<T> guard, T value) where T : IEquatable<T> => guard.WhenNotEquals(value, GuardExceptionMessages.when_not_equals);

    /// <summary>
    /// Throws when the value to guard against is not equal to the specified value
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="value">The value to compare to</param>
    /// <param name="message">The exception message</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<T> WhenNotEquals<T>(this IGuardClause<T> guard, T value, string message) where T : IEquatable<T> => guard.WhenNotEquals(value, new GuardException(message, guard.ArgumentName));

    /// <summary>
    /// Throws when the value to guard against is not equal to the specified value
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="value">The value to compare to</param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<T> WhenNotEquals<T>(this IGuardClause<T> guard, T value, GuardException ex) where T : IEquatable<T>
    {
        if (guard.Value?.Equals(value) != true) throw ex;
        return guard;
    }

}
