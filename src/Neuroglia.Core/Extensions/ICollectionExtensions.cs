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

namespace Neuroglia;

/// <summary>
/// Defines extensions for <see cref="ICollection"/>s
/// </summary>
public static class ICollectionExtensions
{

    /// <summary>
    /// Adds the specified item to the collection
    /// </summary>
    /// <param name="collection">The extended <see cref="ICollection"/></param>
    /// <param name="item">The item to add</param>
    public static void Add(this ICollection collection, object item)
    {
        var itemType = collection.GetType().GetEnumerableElementType();
        typeof(ICollection<>).MakeGenericType(itemType).GetMethod(nameof(Add))!.Invoke(collection, new object[] { item });
    }

    /// <summary>
    /// Determines whether or not the collection contains the specified item
    /// </summary>
    /// <param name="collection">The extended <see cref="ICollection"/></param>
    /// <param name="item">The item to check</param>
    /// <returns>A boolean indicating whether or not the collection contains the specified item</returns>
    public static bool Contains(this ICollection collection, object item)
    {
        var itemType = collection.GetType().GetEnumerableElementType();
        return (bool)typeof(ICollection<>).MakeGenericType(itemType).GetMethod(nameof(Contains))!.Invoke(collection, new object[] { item })!;
    }

    /// <summary>
    /// Removes the specified item from the collection
    /// </summary>
    /// <param name="collection">The extended <see cref="ICollection"/></param>
    /// <param name="item">The item to remove</param>
    /// <returns>A boolean indicating whether or not the specified item was removed from the collection</returns>
    public static bool Remove(this ICollection collection, object item)
    {
        var itemType = collection.GetType().GetEnumerableElementType();
        return (bool)typeof(ICollection<>).MakeGenericType(itemType).GetMethod(nameof(Remove))!.Invoke(collection, new object[] { item })!;
    }

    /// <summary>
    /// Clears the collection
    /// </summary>
    /// <param name="collection">The extended <see cref="ICollection"/></param>
    public static void Clear(this ICollection collection)
    {
        var itemType = collection.GetType().GetEnumerableElementType();
        typeof(ICollection<>).MakeGenericType(itemType).GetMethod(nameof(Clear))!.Invoke(collection, Array.Empty<object>());
    }

}
