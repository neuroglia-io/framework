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

namespace Newtonsoft.Json.Serialization
{

    /// <summary>
    /// Represents an <see cref="IContractResolver"/> used to resolve properties with non-public setters
    /// </summary>
    public class NonPublicSetterContractResolver
        : CamelCasePropertyNamesContractResolver
    {

        /// <inheritdoc/>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty result = base.CreateProperty(member, memberSerialization);
            switch (member)
            {
                case PropertyInfo property:
                    result.Writable |= property.CanWrite;
                    result.Ignored |= !property.CanRead;
                    break;
            }
            return result;
        }

    }

}
