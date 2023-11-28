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

namespace Neuroglia.Data.Guards;

/// <summary>
/// Defines the fundamentals of a service used to guard against invalid data in a streamlined manner
/// </summary>
/// <typeparam name="T">The type of values to validate</typeparam>
public interface IGuardClause<T>
{

    /// <summary>
    /// Gets the value to guard against
    /// </summary>
    T? Value { get; }

    /// <summary>
    /// Gets the name of the argument to guard against, if any
    /// </summary>
    string? ArgumentName { get; }

    /// <summary>
    /// Throws when the value is null
    /// </summary>
    /// <returns>The configured <see cref="IGuardClause{T}"/></returns>
    IGuardClause<T> WhenNull();

    /// <summary>
    /// Throws when the value is null
    /// </summary>
    /// <param name="message">The custom exception message</param>
    /// <returns>The configured <see cref="IGuardClause{T}"/></returns>
    IGuardClause<T> WhenNull(string message);

    /// <summary>
    /// Throws when the value is null
    /// </summary>
    /// <param name="ex">The <see cref="GuardException"/> to throw</param>
    /// <returns>The configured <see cref="IGuardClause{T}"/></returns>
    IGuardClause<T> WhenNull(GuardException ex);

    /// <summary>
    /// Throws when the value is not null
    /// </summary>
    /// <returns>The configured <see cref="IGuardClause{T}"/></returns>
    IGuardClause<T> WhenNotNull();

    /// <summary>
    /// Throws when the value is not null
    /// </summary>
    /// <param name="message">The custom exception message</param>
    /// <returns>The configured <see cref="IGuardClause{T}"/></returns>
    IGuardClause<T> WhenNotNull(string message);

    /// <summary>
    /// Throws when the value is not null
    /// </summary>
    /// <param name="ex">The <see cref="GuardException"/> to throw</param>
    /// <returns>The configured <see cref="IGuardClause{T}"/></returns>
    IGuardClause<T> WhenNotNull(GuardException ex);

    /// <summary>
    /// Throws when the value is null due to an invalid reference
    /// </summary>
    /// <param name="reference">The reference to the guarded value</param>
    /// <param name="keyName">The name of the key used to reference the guarded value</param>
    /// <returns>The configured <see cref="IGuardClause{T}"/></returns>
    IGuardClause<T> WhenNullReference<TKey>(TKey reference, string keyName = "id");

    /// <summary>
    /// Throws when the value is null due to an invalid reference
    /// </summary>
    /// <param name="reference">The reference to the guarded value</param>
    /// <param name="ex">The <see cref="GuardException"/> to throw</param>
    /// <returns>The configured <see cref="IGuardClause{T}"/></returns>
    IGuardClause<T> WhenNullReference<TKey>(TKey reference, GuardException ex);

}
