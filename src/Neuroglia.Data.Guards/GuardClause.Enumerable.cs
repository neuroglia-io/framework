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
using System.Collections;

namespace Neuroglia.Data.Guards;

/// <summary>
/// Defines <see cref="IEnumerable{T}"/>-related guard clauses
/// </summary>
public static class EnumerableGuardClauses
{

    /// <summary>
    /// Throws when the value to guard against is null or empty
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TEnumerable> WhenNullOrEmpty<TEnumerable>(this IGuardClause<TEnumerable> guard) where TEnumerable : IEnumerable => guard.WhenNullOrEmpty(GuardExceptionMessages.when_null_or_empty);

    /// <summary>
    /// Throws when the value to guard against is null or empty
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="message">The exception message</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TEnumerable> WhenNullOrEmpty<TEnumerable>(this IGuardClause<TEnumerable> guard, string message) where TEnumerable : IEnumerable => guard.WhenNullOrEmpty(new GuardException(message, guard.ArgumentName));

    /// <summary>
    /// Throws when the value to guard against is null or empty
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TEnumerable> WhenNullOrEmpty<TEnumerable>(this IGuardClause<TEnumerable> guard, GuardException ex) 
        where TEnumerable : IEnumerable
    {
        if (guard.Value == null || !guard.Value.Cast<object>().Any()) throw ex;
        return guard;
    }

    /// <summary>
    /// Throws when the value contains less than a specified amount of items
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="minimum">The minimum amount of items the value should contain</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TEnumerable> WhenCountLowerThan<TEnumerable>(this IGuardClause<TEnumerable> guard, int minimum) where TEnumerable : IEnumerable => guard.WhenCountLowerThan(minimum, StringFormatter.Format(GuardExceptionMessages.when_count_lower_than, minimum));

    /// <summary>
    /// Throws when the value contains less than a specified amount of items
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="minimum">The minimum amount of items the value should contain</param>
    /// <param name="message">The <see cref="Exception"/> message</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TEnumerable> WhenCountLowerThan<TEnumerable>(this IGuardClause<TEnumerable> guard, int minimum, string message) where TEnumerable : IEnumerable => guard.WhenCountLowerThan(minimum, new GuardException(message, guard.ArgumentName));

    /// <summary>
    /// Throws when the value contains less than a specified amount of items
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="minimum">The minimum amount of items the value should contain</param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TEnumerable> WhenCountLowerThan<TEnumerable>(this IGuardClause<TEnumerable> guard, int minimum, GuardException ex)
        where TEnumerable : IEnumerable
    {
        if (guard.Value == null || guard.Value.Cast<object>().Count() < minimum) throw ex;
        return guard;
    }

    /// <summary>
    /// Throws when the value contains more than a specified amount of items
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="maximum">The maximum amount of items the value should contain</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TEnumerable> WhenCountHigherThan<TEnumerable>(this IGuardClause<TEnumerable> guard, int maximum) where TEnumerable : IEnumerable => guard.WhenCountHigherThan(maximum, StringFormatter.Format(GuardExceptionMessages.when_higher_than, maximum));

    /// <summary>
    /// Throws when the value contains more than a specified amount of items
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="maximum">The maximum amount of items the value should contain</param>
    /// <param name="message">The <see cref="Exception"/> message</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TEnumerable> WhenCountHigherThan<TEnumerable>(this IGuardClause<TEnumerable> guard, int maximum, string message) where TEnumerable : IEnumerable => guard.WhenCountHigherThan(maximum, new GuardException(message, guard.ArgumentName));

    /// <summary>
    /// Throws when the value contains more than a specified amount of items
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="maximum">The maximum amount of items the value should contain</param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TEnumerable> WhenCountHigherThan<TEnumerable>(this IGuardClause<TEnumerable> guard, int maximum, GuardException ex)
        where TEnumerable : IEnumerable
    {
        if (guard.Value == null || guard.Value.Cast<object>().Count() > maximum) throw ex;
        return guard;
    }

    /// <summary>
    /// Throws when the value contains a specific amount of items
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="count">The amount of items the value should contain</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TEnumerable> WhenCountEquals<TEnumerable>(this IGuardClause<TEnumerable> guard, int count) where TEnumerable : IEnumerable => guard.WhenCountEquals(count, StringFormatter.Format(GuardExceptionMessages.when_count_equals, count));

    /// <summary>
    /// Throws when the value contains a specific amount of items
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="count">The amount of items the value should contain</param>
    /// <param name="message">The <see cref="Exception"/> message</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TEnumerable> WhenCountEquals<TEnumerable>(this IGuardClause<TEnumerable> guard, int count, string message) where TEnumerable : IEnumerable => guard.WhenCountEquals(count, new GuardException(message, guard.ArgumentName));

    /// <summary>
    /// Throws when the value contains a specific amount of items
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="count">The amount of items the value should contain</param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TEnumerable> WhenCountEquals<TEnumerable>(this IGuardClause<TEnumerable> guard, int count, GuardException ex)
        where TEnumerable : IEnumerable
    {
        if (guard.Value == null || guard.Value.Cast<object>().Count() == count) throw ex;
        return guard;
    }

    /// <summary>
    /// Throws when the value does not contain a specific amount of items
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="count">The amount of items the value should contain</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TEnumerable> WhenCountNotEquals<TEnumerable>(this IGuardClause<TEnumerable> guard, int count) where TEnumerable : IEnumerable => guard.WhenCountNotEquals(count, StringFormatter.Format(GuardExceptionMessages.when_count_not_equals, count));

    /// <summary>
    /// Throws when the value does not contain a specific amount of items
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="count">The amount of items the value should contain</param>
    /// <param name="message">The <see cref="Exception"/> message</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TEnumerable> WhenCountNotEquals<TEnumerable>(this IGuardClause<TEnumerable> guard, int count, string message) where TEnumerable : IEnumerable => guard.WhenCountNotEquals(count, new GuardException(message, guard.ArgumentName));

    /// <summary>
    /// Throws when the value does not contain a specific amount of items
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="count">The amount of items the value should contain</param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TEnumerable> WhenCountNotEquals<TEnumerable>(this IGuardClause<TEnumerable> guard, int count, GuardException ex)
        where TEnumerable : IEnumerable
    {
        if (guard.Value == null || guard.Value.Cast<object>().Count() != count) throw ex;
        return guard;
    }

    /// <summary>
    /// Throws when the value contains a specific item
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="item">The item the value must contain</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TEnumerable> WhenContains<TEnumerable, TItem>(this IGuardClause<TEnumerable> guard, TItem item) where TEnumerable : IEnumerable<TItem> => guard.WhenContains(item, StringFormatter.Format(GuardExceptionMessages.when_contains, item!));

    /// <summary>
    /// Throws when the value contains a specific item
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="item">The item the value must contain</param>
    /// <param name="message">The <see cref="Exception"/> message</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TEnumerable> WhenContains<TEnumerable, TItem>(this IGuardClause<TEnumerable> guard, TItem item, string message) where TEnumerable : IEnumerable<TItem> => guard.WhenContains(item, new GuardException(message, guard.ArgumentName));

    /// <summary>
    /// Throws when the value contains a specific item
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="item">The item the value must contain</param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TEnumerable> WhenContains<TEnumerable, TItem>(this IGuardClause<TEnumerable> guard, TItem item, GuardException ex)
        where TEnumerable : IEnumerable<TItem>
    {
        if (guard.Value == null || (item != null && guard.Value.Cast<object>().Contains(item!))) throw ex;
        return guard;
    }

    /// <summary>
    /// Throws when the value does not contain a specific item
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="item">The item the value must contain</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TEnumerable> WhenNotContains<TEnumerable, TItem>(this IGuardClause<TEnumerable> guard, TItem item) where TEnumerable : IEnumerable<TItem> => guard.WhenNotContains(item, StringFormatter.Format(GuardExceptionMessages.when_not_contains, item!));

    /// <summary>
    /// Throws when the value does not contain a specific item
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="item">The item the value must contain</param>
    /// <param name="message">The <see cref="Exception"/> message</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TEnumerable> WhenNotContains<TEnumerable, TItem>(this IGuardClause<TEnumerable> guard, TItem item, string message) where TEnumerable : IEnumerable<TItem> => guard.WhenNotContains(item, new GuardException(message, guard.ArgumentName));

    /// <summary>
    /// Throws when the value does not contain a specific item
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="item">The item the value must contain</param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TEnumerable> WhenNotContains<TEnumerable, TItem>(this IGuardClause<TEnumerable> guard, TItem item, GuardException ex)
        where TEnumerable : IEnumerable<TItem>
    {
        if (guard.Value == null || item == null || !guard.Value.Cast<object>().Contains(item!)) throw ex;
        return guard;
    }

    /// <summary>
    /// Throws when the value contains any item matching the specified predicate
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="predicate">The predicate no item should match</param>
    /// <param name="message">The <see cref="Exception"/> message</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TEnumerable> WhenAny<TEnumerable, TItem>(this IGuardClause<TEnumerable> guard, Func<TItem, bool> predicate, string message) where TEnumerable : IEnumerable<TItem> => guard.WhenAny(predicate, new GuardException(message, guard.ArgumentName));

    /// <summary>
    /// Throws when the value contains any item matching the specified predicate
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="predicate">The predicate no item should match</param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TEnumerable> WhenAny<TEnumerable, TItem>(this IGuardClause<TEnumerable> guard, Func<TItem, bool> predicate, GuardException ex)
        where TEnumerable : IEnumerable<TItem>
    {
        if (guard.Value == null || predicate == null || guard.Value.Any(predicate)) throw ex;
        return guard;
    }

    /// <summary>
    /// Throws when the value does not contain any item matching the specified predicate
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="predicate">The predicate at least one item should match</param>
    /// <param name="message">The <see cref="Exception"/> message</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TEnumerable> WhenNone<TEnumerable, TItem>(this IGuardClause<TEnumerable> guard, Func<TItem, bool> predicate, string message) where TEnumerable : IEnumerable<TItem> => guard.WhenNone(predicate, new GuardException(message, guard.ArgumentName));

    /// <summary>
    /// Throws when the value does not contain any item matching the specified predicate
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="predicate">The predicate at least one item should match</param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TEnumerable> WhenNone<TEnumerable, TItem>(this IGuardClause<TEnumerable> guard, Func<TItem, bool> predicate, GuardException ex)
        where TEnumerable : IEnumerable<TItem>
    {
        if (guard.Value == null || predicate == null || !guard.Value.Any(predicate)) throw ex;
        return guard;
    }

    /// <summary>
    /// Throws when all the items the value is made out of match the specified predicate
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="predicate">The predicate no item should match</param>
    /// <param name="message">The <see cref="Exception"/> message</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TEnumerable> WhenAll<TEnumerable, TItem>(this IGuardClause<TEnumerable> guard, Func<TItem, bool> predicate, string message) where TEnumerable : IEnumerable<TItem> => guard.WhenAll(predicate, new GuardException(message, guard.ArgumentName));

    /// <summary>
    /// Throws when all the items the value is made out of match the specified predicate
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="predicate">The predicate no item should match</param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TEnumerable> WhenAll<TEnumerable, TItem>(this IGuardClause<TEnumerable> guard, Func<TItem, bool> predicate, GuardException ex)
        where TEnumerable : IEnumerable<TItem>
    {
        if (guard.Value == null || predicate == null || guard.Value.All(predicate)) throw ex;
        return guard;
    }

    /// <summary>
    /// Throws when all the items the value is made out of do not match the specified predicate
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="predicate">The predicate at least one item should match</param>
    /// <param name="message">The <see cref="Exception"/> message</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TEnumerable> WhenNotAll<TEnumerable, TItem>(this IGuardClause<TEnumerable> guard, Func<TItem, bool> predicate, string message) where TEnumerable : IEnumerable<TItem> => guard.WhenNotAll(predicate, new GuardException(message, guard.ArgumentName));

    /// <summary>
    /// Throws when all the items the value is made out of do not match the specified predicate
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="predicate">The predicate at least one item should match</param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TEnumerable> WhenNotAll<TEnumerable, TItem>(this IGuardClause<TEnumerable> guard, Func<TItem, bool> predicate, GuardException ex)
        where TEnumerable : IEnumerable<TItem>
    {
        if (guard.Value == null || predicate == null || !guard.Value.All(predicate)) throw ex;
        return guard;
    }

}