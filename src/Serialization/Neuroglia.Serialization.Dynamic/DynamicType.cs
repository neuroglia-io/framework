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
namespace Neuroglia.Serialization
{
    /// <summary>
    /// Enumerates all supported dynamic types
    /// </summary>
    public enum DynamicType
    {
        /// <summary>
        /// Represents a null value
        /// </summary>
        Null,
        /// <summary>
        /// Represents a char or a string
        /// </summary>
        String,
        /// <summary>
        /// Represents an enum value
        /// </summary>
        Enum,
        /// <summary>
        /// Represents a boolean
        /// </summary>
        Boolean,
        /// <summary>
        /// Represents a timestamp
        /// </summary>
        Timestamp,
        /// <summary>
        /// Represents a duration
        /// </summary>
        Duration,
        /// <summary>
        /// Represents an integer number
        /// </summary>
        Integer,
        /// <summary>
        /// Represents an long number
        /// </summary>
        Long,
        /// <summary>
        /// Represents a double, a float or a decimal
        /// </summary>
        Number,
        /// <summary>
        /// Represents an array
        /// </summary>
        Array,
        /// <summary>
        /// Represents a complex object
        /// </summary>
        Object
    }

}