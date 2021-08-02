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
using System.Collections.Generic;

namespace Neuroglia
{

    /// <summary>
    /// Defines the fundamentals of an object used to describe the result of an operation
    /// </summary>
    public interface IOperationResult
    {

        /// <summary>
        /// Gets the <see cref="IOperationResult"/>'s code
        /// </summary>
        string Code { get; }

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> containing the <see cref="Error"/>s that have occured during the operation's execution
        /// </summary>
        IReadOnlyCollection<Error> Errors { get; }

        /// <summary>
        /// Gets a boolean indicating whether or not the operation has been successfully executed
        /// </summary>
        bool Succeeded { get; }

        /// <summary>
        /// Gets a boolean indicating whether or not the operation has returned data
        /// </summary>
        bool Returned { get; }

        /// <summary>
        /// Gets the data returned by the operation
        /// </summary>
        object GetData();

        /// <summary>
        /// Adds an error to the <see cref="IOperationResult"/>
        /// </summary>
        /// <param name="code">The error code</param>
        /// <param name="message">The error message</param>
        /// <returns>A new <see cref="IOperationResult"/></returns>
        IOperationResult AddError(string code, string message);

    }

    /// <summary>
    /// Defines the fundamentals of an object used to describe the result of an operation
    /// </summary>
    /// <typeparam name="T">The type of data returned by the operation</typeparam>
    public interface IOperationResult<T>
        : IOperationResult
    {

        /// <summary>
        /// Gets the data returned by the operation
        /// </summary>
        T Data { get; }

        /// <summary>
        /// Adds an error to the <see cref="IOperationResult"/>
        /// </summary>
        /// <param name="code">The error code</param>
        /// <param name="message">The error message</param>
        /// <returns>A new <see cref="IOperationResult"/></returns>
        new IOperationResult<T> AddError(string code, string message);

    }

}
