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
using System;
using System.Collections.Generic;
using System.Linq;
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
            var result = base.CreateProperty(member, memberSerialization);
            switch (member)
            {
                case PropertyInfo property:
                    result.Writable |= property.CanWrite;
                    result.Ignored |= !property.CanRead;
                    break;
            }
            return result;
        }

        /// <inheritdoc/>
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var propertiesByInheritancePriority = type.GetProperties()
               .ToDictionary(p => p.Name.ToLowerInvariant(), p => GetBaseTypes(p.DeclaringType).Count() * 100);
            var properties = base.CreateProperties(type, memberSerialization);
            foreach (var property in properties)
            {
                var propertyName = property.PropertyName.ToLowerInvariant();
                propertiesByInheritancePriority.TryGetValue(propertyName, out var baseIndex);
                if (baseIndex == 0)
                    baseIndex = 999999;
                int order;
                if (property.Order == null)
                    order = propertiesByInheritancePriority.Keys.ToList().IndexOf(propertyName);
                else
                    order = property.Order.Value;
                property.Order = baseIndex + order;
            }
            return properties.OrderBy(p => p.Order).ToList();
        }

        static IEnumerable<Type> GetBaseTypes(Type type)
        {
            while (type != null)
            {
                yield return type;
                type = type.BaseType;
            }
        }

    }

}
