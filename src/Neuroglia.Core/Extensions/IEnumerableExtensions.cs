using System.Collections;
using System.Reflection;

namespace Neuroglia;

/// <summary>
/// Defines extensions for <see cref="IEnumerable"/>s
/// </summary>
public static class IEnumerableExtensions
{

    static readonly MethodInfo OfTypeMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.OfType))!;

    /// <summary>
    /// Converts the <see cref="IEnumerable{T}"/> into a new <see cref="EquatableList{T}"/>
    /// </summary>
    /// <typeparam name="T">The type of items</typeparam>
    /// <param name="source">The <see cref="IEnumerable{T}"/> to convert</param>
    /// <returns>A new <see cref="EquatableList{T}"/></returns>
    public static EquatableList<T> WithValueSemantics<T>(this IEnumerable<T> source) => new(source);
    
    /// <summary>
    /// Filters the elements of a sequence based on a specified type
    /// </summary>
    /// <param name="enumerable">The enumerable to filter</param>
    /// <param name="type">The type to filter the sequence by</param>
    /// <returns>A new <see cref="IEnumerable"/></returns>
    public static IEnumerable OfType(this IEnumerable enumerable, Type type) => (IEnumerable)OfTypeMethod.MakeGenericMethod(type).Invoke(null, new object[] { enumerable })!;

}
