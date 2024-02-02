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

using System.Collections;
using System.Reflection;

namespace Neuroglia;

/// <summary>
/// Defines extensions for <see cref="IEnumerable"/>s
/// </summary>
public static class IEnumerableExtensions
{

    static readonly MethodInfo OfTypeMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.OfType))!;
    static readonly MethodInfo ToListMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.ToList))!;

    /// <summary>
    /// Converts the <see cref="IEnumerable{T}"/> into a new <see cref="EquatableList{T}"/>
    /// </summary>
    /// <typeparam name="T">The type of items</typeparam>
    /// <param name="source">The <see cref="IEnumerable{T}"/> to convert</param>
    /// <returns>A new <see cref="EquatableList{T}"/></returns>
    public static EquatableList<T> WithValueSemantics<T>(this IEnumerable<T> source) => new(source);

    /// <summary>
    /// Gets the element at the specified index
    /// </summary>
    /// <param name="enumerable">The extended collection</param>
    /// <param name="index">The index of the element to get</param>
    /// <returns>The element at the specified index</returns>
    public static object GetElementAt(this IEnumerable enumerable, int index)
    {
        var i = 0;
        foreach (var item in enumerable)
        {
            if (i == index) return item;
        }
        throw new ArgumentOutOfRangeException(nameof(index));
    }

    /// <summary>
    /// Joins the values of the <see cref="IEnumerable{T}"/> with the specified character
    /// </summary>
    /// <param name="values">The values to join</param>
    /// <param name="separator">The separator char</param>
    /// <returns>A new string that consists of the joined values, separated by the specified char</returns>
    public static string Join(this IEnumerable<string> values, char separator) => string.Join(separator, values);

    /// <summary>
    /// Filters the elements of a sequence based on a specified type
    /// </summary>
    /// <param name="enumerable">The enumerable to filter</param>
    /// <param name="type">The type to filter the sequence by</param>
    /// <returns>A new <see cref="IEnumerable"/></returns>
    public static IEnumerable OfType(this IEnumerable enumerable, Type type) => (IEnumerable)OfTypeMethod.MakeGenericMethod(type).Invoke(null, new object[] { enumerable })!;

    /// <summary>
    /// Converts the <see cref="IEnumerable"/> into a new <see cref="IList"/>
    /// </summary>
    /// <param name="enumerable">The <see cref="IEnumerable"/> to convert</param>
    /// <returns>A new <see cref="IList"/></returns>
    public static IList ToNonGenericList(this IEnumerable enumerable) => (IList)ToListMethod.MakeGenericMethod(enumerable.GetType().GetEnumerableElementType()).Invoke(null, new object[] { enumerable })!;

}
