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

using System.Runtime.CompilerServices;

namespace Neuroglia.Data.Guards;

/// <summary>
/// Defines methods to guard against invalid data
/// </summary>
public static class Guard
{

    /// <summary>
    /// Guards against the value
    /// </summary>
    /// <typeparam name="T">The type of value to validate</typeparam>
    /// <param name="value">The value to validate</param>
    /// <returns>The configured <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<T> Against<T>(T? value) => new GuardClause<T>(value);

    /// <summary>
    /// Guards against the value
    /// </summary>
    /// <typeparam name="T">The type of value to validate</typeparam>
    /// <param name="value">The value to validate</param>
    /// <param name="argumentName">The name of the argument to validate</param>
    /// <returns>The configured <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<T> AgainstArgument<T>(T value, [CallerArgumentExpression(nameof(value))] string? argumentName = null) => new GuardClause<T>(value, argumentName);

}
