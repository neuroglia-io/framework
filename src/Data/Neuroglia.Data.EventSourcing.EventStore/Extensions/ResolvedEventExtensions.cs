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
using EventStore.ClientAPI;
using System;

namespace Neuroglia.Data.EventSourcing
{

    /// <summary>
    /// Defines extensions for <see cref="ResolvedEvent"/>s
    /// </summary>
    public static class ResolvedEventExtensions
    {

        /// <summary>
        /// Abstracts the <see cref="ResolvedEvent"/>
        /// </summary>
        /// <typeparam name="TKey">The type of key used to uniquely identify the <see cref="ResolvedEvent"/>'s abstraction</typeparam>
        /// <param name="e">The <see cref="ResolvedEvent"/> to abstract</param>
        /// <returns>A new <see cref="IEvent{TKey}"/> used to abstract the specified <see cref="ResolvedEvent"/></returns>
        public static IEvent<TKey> AsAbstraction<TKey>(this ResolvedEvent e)
            where TKey : IEquatable<TKey>
        {
            return new Event<TKey>(Parser.Parse<TKey>(e.Event.EventId.ToString()), e.Event.EventNumber, e.Event.Created, e.Event.EventType, e.Event.Data, e.Event.Metadata);
        }

    }

}
