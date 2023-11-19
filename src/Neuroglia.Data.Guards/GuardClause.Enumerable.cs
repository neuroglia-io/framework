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
    public static IGuardClause<TEnumerable> WhenNullOrEmpty<TEnumerable>(this IGuardClause<TEnumerable> guard) where TEnumerable : IEnumerable => guard.WhenNullOrEmpty("The specified value cannot be null or empty");

    /// <summary>
    /// Throws when the value to guard against is null or empty
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="message">The exception message</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TEnumerable> WhenNullOrEmpty<TEnumerable>(this IGuardClause<TEnumerable> guard, string message) where TEnumerable : IEnumerable => guard.WhenNullOrEmpty(new GuardException(message));

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
    public static IGuardClause<TEnumerable> WhenCountLowerThan<TEnumerable>(this IGuardClause<TEnumerable> guard, int minimum) where TEnumerable : IEnumerable => guard.WhenCountLowerThan(minimum, $"The specified must have at least '{minimum}' items");

    /// <summary>
    /// Throws when the value contains less than a specified amount of items
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="minimum">The minimum amount of items the value should contain</param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TEnumerable> WhenCountLowerThan<TEnumerable>(this IGuardClause<TEnumerable> guard, int minimum, string message) where TEnumerable : IEnumerable => guard.WhenCountLowerThan(minimum, new GuardException(message));

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
    public static IGuardClause<TEnumerable> WhenCountHigherThan<TEnumerable>(this IGuardClause<TEnumerable> guard, int maximum) where TEnumerable : IEnumerable => guard.WhenCountHigherThan(maximum, $"The specified must have a maximum of '{maximum}' items");

    /// <summary>
    /// Throws when the value contains more than a specified amount of items
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="maximum">The maximum amount of items the value should contain</param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TEnumerable> WhenCountHigherThan<TEnumerable>(this IGuardClause<TEnumerable> guard, int maximum, string message) where TEnumerable : IEnumerable => guard.WhenCountHigherThan(maximum, new GuardException(message));

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
    public static IGuardClause<TEnumerable> WhenCountEquals<TEnumerable>(this IGuardClause<TEnumerable> guard, int count) where TEnumerable : IEnumerable => guard.WhenCountEquals(count, $"The specified value must not contain {count} items");

    /// <summary>
    /// Throws when the value contains a specific amount of items
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="count">The amount of items the value should contain</param>
    /// <param name="message">The <see cref="Exception"/> message</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TEnumerable> WhenCountEquals<TEnumerable>(this IGuardClause<TEnumerable> guard, int count, string message) where TEnumerable : IEnumerable => guard.WhenCountEquals(count, new GuardException(message));

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
    public static IGuardClause<TEnumerable> WhenCountNotEquals<TEnumerable>(this IGuardClause<TEnumerable> guard, int count) where TEnumerable : IEnumerable => guard.WhenCountNotEquals(count, $"The specified value does not contain exactly {count} items");

    /// <summary>
    /// Throws when the value does not contain a specific amount of items
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="count">The amount of items the value should contain</param>
    /// <param name="message">The <see cref="Exception"/> message</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TEnumerable> WhenCountNotEquals<TEnumerable>(this IGuardClause<TEnumerable> guard, int count, string message) where TEnumerable : IEnumerable => guard.WhenCountNotEquals(count, new GuardException(message));

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
    public static IGuardClause<TEnumerable> WhenContains<TEnumerable, TItem>(this IGuardClause<TEnumerable> guard, TItem item) where TEnumerable : IEnumerable<TItem> => guard.WhenContains(item, $"The specified value must not contain '{item}'");

    /// <summary>
    /// Throws when the value contains a specific item
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="item">The item the value must contain</param>
    /// <param name="message">The <see cref="Exception"/> message</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TEnumerable> WhenContains<TEnumerable, TItem>(this IGuardClause<TEnumerable> guard, TItem item, string message) where TEnumerable : IEnumerable<TItem> => guard.WhenContains(item, new GuardException(message));

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
    public static IGuardClause<TEnumerable> WhenNotContains<TEnumerable, TItem>(this IGuardClause<TEnumerable> guard, TItem item) where TEnumerable : IEnumerable<TItem> => guard.WhenNotContains(item, $"The specified value must contain '{item}'");

    /// <summary>
    /// Throws when the value does not contain a specific item
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="item">The item the value must contain</param>
    /// <param name="message">The <see cref="Exception"/> message</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<TEnumerable> WhenNotContains<TEnumerable, TItem>(this IGuardClause<TEnumerable> guard, TItem item, string message) where TEnumerable : IEnumerable<TItem> => guard.WhenNotContains(item, new GuardException(message));

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
    public static IGuardClause<TEnumerable> WhenAny<TEnumerable, TItem>(this IGuardClause<TEnumerable> guard, Func<TItem, bool> predicate, string message) where TEnumerable : IEnumerable<TItem> => guard.WhenAny(predicate, new GuardException(message));

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
    public static IGuardClause<TEnumerable> WhenNone<TEnumerable, TItem>(this IGuardClause<TEnumerable> guard, Func<TItem, bool> predicate, string message) where TEnumerable : IEnumerable<TItem> => guard.WhenNone(predicate, new GuardException(message));

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
    public static IGuardClause<TEnumerable> WhenAll<TEnumerable, TItem>(this IGuardClause<TEnumerable> guard, Func<TItem, bool> predicate, string message) where TEnumerable : IEnumerable<TItem> => guard.WhenAll(predicate, new GuardException(message));

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
    public static IGuardClause<TEnumerable> WhenNotAll<TEnumerable, TItem>(this IGuardClause<TEnumerable> guard, Func<TItem, bool> predicate, string message) where TEnumerable : IEnumerable<TItem> => guard.WhenNotAll(predicate, new GuardException(message));

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