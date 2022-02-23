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
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Dynamic;

namespace Neuroglia.Serialization
{
    /// <summary>
    /// Defines extensions for <see cref="JToken"/>s
    /// </summary>
    public static class JTokenExtensions
    {

        /// <summary>
        /// Converts the specified <see cref="JToken"/> into a new object
        /// </summary>
        /// <param name="token"></param>
        /// <returns>A new object</returns>
        public static object? ToObject(this JToken token)
        {
            switch (token)
            {
                case JArray jarray:
                    var list = jarray.ToObject<IList>()!.OfType<object>();
                    if (list.Any()
                        && list.First() is JToken)
                        return list.OfType<JToken>().Select(t => t.ToObject());
                    return list;
                case JObject jobject:
                    return jobject.ToObject<ExpandoObject>();
                default:
                    return token.ToObject<object>();
            }
        }

    }

}
