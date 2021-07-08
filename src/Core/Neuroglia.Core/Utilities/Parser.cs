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

namespace Neuroglia
{

    /// <summary>
    /// Exposes helper methods to parse data into value types and other .NET types (TimeSpan, DateTime, ...)
    /// </summary>
    public static class Parser
    {

        static readonly Dictionary<Type, Func<string, object>> ParserMappings = new()
        {
            { typeof(bool), input => bool.Parse(input) },
            { typeof(byte), input => byte.Parse(input) },
            { typeof(char), input => char.Parse(input) },
            { typeof(decimal), input => decimal.Parse(input) },
            { typeof(double), input => double.Parse(input) },
            { typeof(float), input => float.Parse(input) },
            { typeof(int), input => int.Parse(input) },
            { typeof(long), input => long.Parse(input) },
            { typeof(sbyte), input => sbyte.Parse(input) },
            { typeof(short), input => short.Parse(input) },
            { typeof(string), input => input },
            { typeof(uint), input => uint.Parse(input) },
            { typeof(ulong), input => ulong.Parse(input) },
            { typeof(ushort), input => ushort.Parse(input) },
            { typeof(DateTime), input => DateTime.Parse(input) },
            { typeof(DateTimeOffset), input => DateTimeOffset.Parse(input) },
            { typeof(Guid), input => Guid.Parse(input) },
            { typeof(TimeSpan), input => TimeSpan.Parse(input) }
        };

        /// <summary>
        /// Parses a string into the specified type
        /// </summary>
        /// <param name="input">The input to parse</param>
        /// <param name="expectedType">The type to parse the input to</param>
        /// <returns>The parsed input</returns>
        public static object Parse(string input, Type expectedType)
        {
            if (expectedType == null)
                throw new ArgumentNullException(nameof(expectedType));
            if (string.IsNullOrWhiteSpace(input))
                return expectedType.GetDefaultValue();
            if (expectedType.IsNullable())
                expectedType = expectedType.GetNullableType();
            if (!ParserMappings.TryGetValue(expectedType, out Func<string, object> parser))
                throw new FormatException($"Failed to parse the specified input '{input}' into expected type '{expectedType.Name}'");
            return parser(input);
        }

        /// <summary>
        /// Attempts to parses a string into the specified type
        /// </summary>
        /// <param name="input">The input to parse</param>
        /// <param name="expectedType">The type to parse the input to</param>
        /// <param name="value">The parsed input</param>
        /// <returns>A boolean indicating whether or not the specified input could be parsed</returns>
        public static bool TryParse(string input, Type expectedType, out object value)
        {
            value = expectedType.GetDefaultValue();
            if (expectedType == null)
                throw new ArgumentNullException(nameof(expectedType));
            try
            {
                value = Parse(input, expectedType);
                return true;
            }
            catch
            {
                return false;
            }
         
        }

        /// <summary>
        /// Parses a string into the specified type
        /// </summary>
        /// <typeparam name="T">The type to parse the input to</typeparam>
        /// <param name="input">The input to parse</param>
        /// <returns>The parsed input</returns>
        public static T Parse<T>(string input)
        {
            return (T)Parse(input, typeof(T));
        }

        /// <summary>
        /// Attempts to parses a string into the specified type
        /// </summary>
        /// <typeparam name="T">The type to parse the input to</typeparam>
        /// <param name="input">The input to parse</param>
        /// <param name="value">The parsed input</param>
        /// <returns>A boolean indicating whether or not the specified input could be parsed</returns>
        public static bool TryParse<T>(string input, out object value)
        {
            return TryParse(input, typeof(T), out value);
        }

    }

}
