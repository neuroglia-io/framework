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
/// Defines extensions for <see cref="IList"/>s
/// </summary>
public static class IListExtensions
{

    static readonly MethodInfo InsertMethod = typeof(IList<>).GetMethod(nameof(Insert))!;

    /// <summary>
    /// Inserts the specified item in the list
    /// </summary>
    /// <param name="list">The extended <see cref="IList"/></param>
    /// <param name="index">The index to insert the specified item at</param>
    /// <param name="item">The item to insert</param>
    public static void Insert(this IList list, int index, object item)
    {
        var itemType = list.GetType().GetEnumerableElementType();
        InsertMethod.MakeGenericMethod(itemType).Invoke(list, new object[] { index, item });
    }

}