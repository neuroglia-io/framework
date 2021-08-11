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
using System.Collections.Generic;
using System.Dynamic;

namespace Neuroglia
{

    /// <summary>
    /// Exposes extensions for <see cref="IDictionary{TKey, TValue}"/>
    /// </summary>
    public static class IDictionaryExtensions
    {

        /// <summary>
        /// Converts the <see cref="IDictionary{TKey, TValue}"/> into a new <see cref="ExpandoObject"/>
        /// </summary>
        /// <param name="dictionary">The extended <see cref="IDictionary{TKey, TValue}"/></param>
        /// <returns>A new <see cref="ExpandoObject"/></returns>
        public static ExpandoObject ToExpandoObject<T>(this IDictionary<string, T> dictionary)
        {
            ExpandoObject expandoObject = new();
            ICollection<KeyValuePair<string, object>> expandoProperties = expandoObject;
            foreach (KeyValuePair<string, T> kvp in dictionary)
            {
                expandoProperties.Add(new KeyValuePair<string, object>(kvp.Key, kvp.Value));
            }
            return expandoObject;
        }

    }

}
