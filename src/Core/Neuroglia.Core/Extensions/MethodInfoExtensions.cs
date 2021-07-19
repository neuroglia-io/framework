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
using System.Reflection;
using System.Threading.Tasks;

namespace Neuroglia
{
    /// <summary>
    /// Defines extensions for <see cref="MethodInfo"/>s
    /// </summary>
    public static class MethodInfoExtensions
    {

        /// <summary>
        /// Invokes the <see cref="MethodInfo"/> asynchronously
        /// </summary>
        /// <param name="method">The <see cref="MethodInfo"/> to invoke</param>
        /// <param name="obj">The object on which to invoke the <see cref="MethodInfo"/></param>
        /// <param name="parameters">An array containing the <see cref="MethodInfo"/>'s parameters</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public static async Task<object> InvokeAsync(this MethodInfo method, object obj, params object[] parameters)
        {
            dynamic awaitable = method.Invoke(obj, parameters);
            await awaitable;
            return awaitable.GetAwaiter().GetResult();
        }

    }

}