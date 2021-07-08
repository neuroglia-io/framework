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
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.Serialization
{

    /// <summary>
    /// Defines the fundamentals of a service used to serialize/deserialize text
    /// </summary>
    public interface ITextSerializer
        : ISerializer
    {

        /// <summary>
        /// Serializes the specified value
        /// </summary>
        /// <param name="value">The value to serialize</param>
        /// <returns>The resulting text</returns>
        new string Serialize(object value);

        /// <summary>
        /// Serializes the specified value
        /// </summary>
        /// <param name="value">The value to serialize</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The resulting text</returns>
        new Task<string> SerializeAsync(object value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Serializes the specified value
        /// </summary>
        /// <param name="value">The value to serialize</param>
        /// <param name="type">The type of the value to serialize</param>
        /// <returns>The resulting text</returns>
        new string Serialize(object value, Type type);

        /// <summary>
        /// Serializes the specified value
        /// </summary>
        /// <param name="value">The value to serialize</param>
        /// <param name="type">The type of the value to serialize</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The resulting text</returns>
        new Task<string> SerializeAsync(object value, Type type, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deserializes the specified text
        /// </summary>
        /// <param name="input">The text to deserialize</param>
        /// <param name="returnType">The expected return type</param>
        /// <returns>The deserialized value</returns>
        object Deserialize(string input, Type returnType);

        /// <summary>
        /// Deserializes the specified text
        /// </summary>
        /// <param name="input">The text to deserialize</param>
        /// <param name="returnType">The expected return type</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The deserialized value</returns>
        Task<object> DeserializeAsync(string input, Type returnType, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deserializes the specified text
        /// </summary>
        /// <typeparam name="T">The expected return type</typeparam>
        /// <param name="input">The text to deserialize</param>
        /// <returns>The deserialized value</returns>
        T Deserialize<T>(string input);

        /// <summary>
        /// Deserializes the specified text
        /// </summary>
        /// <typeparam name="T">The expected return type</typeparam>
        /// <param name="input">The text to deserialize</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The deserialized value</returns>
        Task<T> DeserializeAsync<T>(string input, CancellationToken cancellationToken = default);

    }

}
