/*
 * Copyright © 2021 Neuroglia SPRL. All rights reserved.
 * <p>
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * <p>
 * http://www.apache.org/licenses/LICENSE-2.0
 * <p>
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */

using Neuroglia;
using System.Collections.Generic;

namespace System.Collections
{
    /// <summary>
    /// Defines extensions for <see cref="ICollection"/>s
    /// </summary>
    public static class ICollectionExtensions
    {

        /// <summary>
        /// Adds the specified value to the <see cref="ICollection"/>
        /// </summary>
        /// <param name="collection">The <see cref="ICollection"/> to add a value to</param>
        /// <param name="value">The value to add</param>
        public static void Add(this ICollection collection, object value)
        {
            typeof(ICollection<>).MakeGenericType(collection.GetType().GetEnumerableElementType()).GetMethod(nameof(Add)).Invoke(collection, new object[] { value });
        }

        /// <summary>
        /// Adds the specified values to the <see cref="ICollection"/>
        /// </summary>
        /// <param name="collection">The <see cref="ICollection"/> to add values to</param>
        /// <param name="values">The values to add</param>
        public static void AddRange(this ICollection collection, IEnumerable values)
        {
            var method = typeof(ICollection<>).MakeGenericType(collection.GetType().GetEnumerableElementType()).GetMethod(nameof(Add));
            foreach (var value in values)
            {
                method.Invoke(collection, new object[] { value });
            }
        }

    }

}
