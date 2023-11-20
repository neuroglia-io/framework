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

using Neuroglia.Data.Guards.Properties;

namespace Neuroglia.Data.Guards;

/// <summary>
/// Defines <see cref="Guid"/>-related guard clauses
/// </summary>
public static class GuidGuardClause
{

    /// <summary>
    /// Throws when the value is an empty <see cref="Guid"/>
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<Guid> WhenEmpty(this IGuardClause<Guid> guard) => guard.WhenEmpty(GuardExceptionMessages.when_null_or_empty);

    /// <summary>
    /// Throws when the value is an empty <see cref="Guid"/>
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="message">The <see cref="Exception"/> message</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<Guid> WhenEmpty(this IGuardClause<Guid> guard, string message) => guard.WhenEmpty(new GuardException(message, guard.ArgumentName));

    /// <summary>
    /// Throws when the value is an empty <see cref="Guid"/>
    /// </summary>
    /// <param name="guard">The extended <see cref="IGuardClause{T}"/></param>
    /// <param name="ex">The <see cref="Exception"/> to throw</param>
    /// <returns>The configure <see cref="IGuardClause{T}"/></returns>
    public static IGuardClause<Guid> WhenEmpty(this IGuardClause<Guid> guard, GuardException ex)
    {
        if (guard.Value == Guid.Empty) throw ex;
        return guard;
    }

}
