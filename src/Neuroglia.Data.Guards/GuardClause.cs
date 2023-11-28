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
/// Represents the default implementation of the <see cref="IGuardClause{T}"/> interface
/// </summary>
/// <typeparam name="T">The type of value to guard against</typeparam>
/// <remarks>
/// Initializes a new <see cref="GuardClause{T}"/>
/// </remarks>
/// <param name="value">The value to guard against</param>
/// <param name="argumentName">The name of the argument to guard against, if any</param>
public class GuardClause<T>(T? value, string? argumentName = null)
    : IGuardClause<T>
{

    /// <inheritdoc/>
    public T? Value { get; } = value;

    /// <inheritdoc/>
    public string? ArgumentName { get; } = argumentName;

    /// <inheritdoc/>
    public IGuardClause<T> WhenNull() => this.WhenNull(GuardExceptionMessages.when_null);

    /// <inheritdoc/>
    public IGuardClause<T> WhenNull(string message) => this.WhenNull(new GuardException(message, this.ArgumentName));

    /// <inheritdoc/>
    public IGuardClause<T> WhenNull(GuardException ex)
    {
        if (this.Value == null) throw ex;
        return this;
    }

    /// <inheritdoc/>
    public IGuardClause<T> WhenNotNull() => this.WhenNotNull(GuardExceptionMessages.when_not_null);

    /// <inheritdoc/>
    public IGuardClause<T> WhenNotNull(string message) => this.WhenNotNull(new GuardException(message, this.ArgumentName));

    /// <inheritdoc/>
    public IGuardClause<T> WhenNotNull(GuardException ex)
    {
        if (this.Value != null) throw ex;
        return this;
    }

    /// <inheritdoc/>
    public IGuardClause<T> WhenNullReference<TKey>(TKey key, string keyName) => this.WhenNullReference(key, new GuardException(StringFormatter.NamedFormat(GuardExceptionMessages.when_null_reference, new { type = typeof(T).Name.ToCamelCase(), key, keyName }), this.ArgumentName));

    /// <inheritdoc/>
    public IGuardClause<T> WhenNullReference<TKey>(TKey key, GuardException ex)
    {
        if (this.Value == null) throw ex;
        return this;
    }

}
