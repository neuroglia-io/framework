// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License"),
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Neuroglia;

/// <summary>
/// Defines the fundamentals of an object used to describe the result of an operation
/// </summary>
public interface IOperationResult
{

    /// <summary>
    /// Gets a value that describes the status of the operation result. A value between 199 and 300 indicates success
    /// </summary>
    int Status { get; }

    /// <summary>
    /// Gets the data, if any, returned by the operation, in case of success
    /// </summary>
    object? Data { get; }

    /// <summary>
    /// Gets a list containing the errors that have occurred, if any, during the execution of the operation
    /// </summary>
    IReadOnlyCollection<Error>? Errors { get; }

}

/// <summary>
/// Defines the fundamentals of an object used to describe the result of an operation
/// </summary>
/// <typeparam name="T">The type of data, if any, returned by the operation</typeparam>
public interface IOperationResult<T>
    : IOperationResult
{

    /// <summary>
    /// Gets the data, if any, returned by the operation, in case of success
    /// </summary>
    new T? Data { get; }

}
