using System.Collections;

namespace Neuroglia;

/// <summary>
/// Defines extensions for <see cref="IEnumerable"/>s
/// </summary>
public static class IEnumerableExtensions
{

    /// <summary>
    /// Converts the <see cref="IEnumerable{T}"/> into a new <see cref="EquatableList{T}"/>
    /// </summary>
    /// <typeparam name="T">The type of items</typeparam>
    /// <param name="source">The <see cref="IEnumerable{T}"/> to convert</param>
    /// <returns>A new <see cref="EquatableList{T}"/></returns>
    public static EquatableList<T> WithValueSemantics<T>(this IEnumerable<T> source) => new(source);

}
