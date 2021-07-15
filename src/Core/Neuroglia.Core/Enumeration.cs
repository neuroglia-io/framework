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

namespace Neuroglia
{

    /// <summary>
    /// Represents the base class for all enumeration classes
    /// </summary>
    public abstract class Enumeration
        : IEnumeration
    {

        /// <summary>
        /// Initializes a new <see cref="Enumeration"/>
        /// </summary>
        protected Enumeration()
        {

        }

        /// <summary>
        /// Initializes a new <see cref="Enumeration"/>
        /// </summary>
        /// <param name="value">The <see cref="Enumeration"/>'s value</param>
        /// <param name="name">The <see cref="Enumeration"/>'s name</param>
        protected Enumeration(int value, string name)
            : this()
        {
            this.Value = value;
            this.Name = name;
        }

        /// <summary>
        /// Gets the <see cref="Enumeration"/>'s value
        /// </summary>
        public int Value { get; private set; }

        /// <summary>
        /// Gets the <see cref="Enumeration"/>'s name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Determines whether or not the <see cref="Enumeration"/> equals another object
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>A boolean indicating whether or not the <see cref="Enumeration"/> equals another object</returns>
        public override bool Equals(object obj)
        {
            if (obj is not Enumeration otherValue)
                return false;
            bool typeMatches = this.GetType().Equals(obj.GetType());
            bool valueMatches = this.Value.Equals(otherValue.Value);
            return typeMatches && valueMatches;
        }

        /// <summary>
        /// Compares the <see cref="Enumeration"/> to another object
        /// </summary>
        /// <param name="other">The object to compare</param>
        /// <returns>The comparison result</returns>
        public int CompareTo(object other)
        {
            return this.Value.CompareTo(((Enumeration)other).Value);
        }

        /// <summary>
        /// Gets the <see cref="Enumeration"/>'s hashcode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        /// <summary>
        /// Gets the <see cref="Enumeration"/>'s string representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Name;
        }

        /// <summary>
        /// Gets all the values of the <see cref="Enumeration"/> of the specified type
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Enumeration"/> to list all values of</typeparam>
        /// <returns>All the values of the <see cref="Enumeration"/> of the specified type</returns>
        public static IEnumerable<T> GetAll<T>()
            where T : Enumeration
        {
            FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
            return fields.Select(f => f.GetValue(null)).Cast<T>();
        }

        /// <summary>
        /// Parses the specified value into an <see cref="Enumeration"/> class
        /// </summary>
        /// <typeparam name="T">The type of <see cref="Enumeration"/> to parse the value into</typeparam>
        /// <param name="value">The value to parse</param>
        /// <returns>A new <see cref="Enumeration"/> parsed from the specified value</returns>
        public static T FromValue<T>(int value)
            where T : Enumeration
        {
            return Enumeration.Parse<T, int>(value, "value", item => item.Value == value);
        }

        /// <summary>
        /// Parses the specified name into an <see cref="Enumeration"/> class
        /// </summary>
        /// <typeparam name="T">The type of <see cref="Enumeration"/> to parse the name into</typeparam>
        /// <param name="name">The value to parse</param>
        /// <returns>A new <see cref="Enumeration"/> parsed from the specified name</returns>
        public static T FromName<T>(string name)
            where T : Enumeration
        {
            return Parse<T, string>(name, "name", item => item.Name == name);
        }

        /// <summary>
        /// Converts the specified <see cref="Enumeration"/> into a string
        /// </summary>
        /// <param name="value">The <see cref="Enumeration"/> class to convert to its string representation</param>
        public static implicit operator string(Enumeration value)
        {
            return value?.Name;
        }

        /// <summary>
        /// Converts the specified <see cref="Enumeration"/> into an integer
        /// </summary>
        /// <param name="value">The <see cref="Enumeration"/> class to convert to its integer representation</param>
        public static implicit operator int(Enumeration value)
        {
            return value.Value;
        }

        private static T Parse<T, K>(K value, string description, Func<T, bool> predicate)
            where T : Enumeration
        {
            T matchingItem = Enumeration.GetAll<T>()
                .FirstOrDefault(predicate);
            if (matchingItem == null)
                throw new InvalidOperationException($"'{value}' is not a valid {description} in {typeof(T)}");
            return matchingItem;
        }

    }

}
