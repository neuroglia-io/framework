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

namespace Neuroglia.Data.EventSourcing
{

    /// <summary>
    /// Represents a position in a <see cref="EventStream"/>
    /// </summary>
    public struct EventStreamPosition
    {

        /// <summary>
        /// Gets an <see cref="EventStreamPosition"/> that represents the beginning (i.e., the first event) of a stream
        /// </summary>
        public static readonly EventStreamPosition Start = 0;
        /// <summary>
        /// Gets an <see cref="EventStreamPosition"/> that represents the end of a stream. Use this when reading a stream backwards, or subscribing live to a stream
        /// </summary>
        public static readonly EventStreamPosition End = -1;

        /// <summary>
        /// Initializes a new <see cref="EventStreamPosition"/>
        /// </summary>
        /// <param name="value">The <see cref="EventStreamPosition"/>'s value</param>
        public EventStreamPosition(long value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the <see cref="EventStreamPosition"/>'s value
        /// </summary>
        public long Value { get; }

        /// <summary>
        /// Implicitly converts the specified value into a new <see cref="EventStreamPosition"/>
        /// </summary>
        /// <param name="value">The value of the <see cref="EventStreamPosition"/> to create</param>
        public static implicit operator EventStreamPosition(long value)
        {
            return new(value);
        }

        /// <summary>
        /// Implicitly converts the specified <see cref="EventStreamPosition"/> into a <see cref="long"/>
        /// </summary>
        /// <param name="position">The <see cref="EventStreamPosition"/> to convert</param>
        public static implicit operator long(EventStreamPosition position)
        {
            return position.Value;
        }

    }

}
