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
/// Defines extensions for <see cref="IOperationResult"/>s
/// </summary>
public static class IOperationResultExtensions
{

    /// <summary>
    /// Determines whether the <see cref="IOperationResult"/> indicates success.
    /// </summary>
    /// <param name="operationResult">The <see cref="IOperationResult"/> to check</param>
    /// <returns>True if the <see cref="IOperationResult"/> indicates success; otherwise, false.</returns>
    public static bool IsSuccess(this IOperationResult operationResult) => operationResult.Status >= 200 && operationResult.Status < 300;

    /// <summary>
    /// Converts the <see cref="OperationResult"/> to a new <see cref="OperationResult{TContent}"/>
    /// </summary>
    /// <typeparam name="TContent">The type of content wrapped by the <see cref="OperationResult"/></typeparam>
    /// <param name="result">The <see cref="OperationResult"/> to convert</param>
    /// <returns>A new <see cref="OperationResult{TContent}"/></returns>
    public static OperationResult<TContent> OfType<TContent>(this OperationResult result)
    {
        ArgumentNullException.ThrowIfNull(result);
        return new(result.Status, result.Data);
    }

}
