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

namespace Neuroglia;

/// <summary>
/// Defines extensions for <see cref="IDictionary{TKey, TValue}"/> instances
/// </summary>
public static class IDictionaryExtensions
{

    /// <summary>
    /// Gets the value at the specified key
    /// </summary>
    /// <typeparam name="TKey">The type of the <see cref="IDictionary{TKey, TValue}"/> keys</typeparam>
    /// <typeparam name="TValue">The type of the <see cref="IDictionary{TKey, TValue}"/> values</typeparam>
    /// <param name="dictionary">The extended <see cref="IDictionary{TKey, TValue}"/></param>
    /// <param name="key">The key of the value to get</param>
    /// <returns>The value with the specified key</returns>
    public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) => dictionary[key];

}
