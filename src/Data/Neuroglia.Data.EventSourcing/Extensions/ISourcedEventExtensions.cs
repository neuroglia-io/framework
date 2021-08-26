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
using System;

namespace Neuroglia.Data.EventSourcing
{

    /// <summary>
    /// Defines extensions for <see cref="ISourcedEvent"/>s
    /// </summary>
    public static class ISourcedEventExtensions
    {

        /// <summary>
        /// Converts the <see cref="ISourcedEvent"/> into a new <see cref="IDomainEvent"/>
        /// </summary>
        /// <param name="e">The <see cref="ISourcedEvent"/> to convert</param>
        /// <returns>A new <see cref="IDomainEvent"/></returns>
        public static IDomainEvent AsDomainEvent(this ISourcedEvent e)
        {
            if (e.Metadata is not JObject metadata)
                return null;
            string typeName = metadata.Property(EventSourcingDefaults.Metadata.RuntimeTypeName).Value.ToString();
            if (string.IsNullOrWhiteSpace(typeName))
                return null;
            if (e.Data is not JObject data)
                return null;
            Type type = Type.GetType(typeName);
            return data.ToObject(type) as IDomainEvent;
        }

    }

}
