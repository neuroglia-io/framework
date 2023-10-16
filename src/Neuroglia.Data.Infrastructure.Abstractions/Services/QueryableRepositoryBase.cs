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

namespace Neuroglia.Data.Infrastructure.Services;

/// <summary>
/// Represents the default base class implementation of the <see cref="IRepository{TEntity, TKey}"/> interface
/// </summary>
/// <typeparam name="TEntity">The type of the managed entities</typeparam>
/// <typeparam name="TKey">The type of key used to identify and distinct managed entities</typeparam>
public abstract class QueryableRepositoryBase<TEntity, TKey>
    : RepositoryBase<TEntity, TKey>, IQueryableRepository<TEntity, TKey>
    where TEntity : class, IIdentifiable<TKey>
    where TKey : IEquatable<TKey>
{

    /// <inheritdoc/>
    public abstract IQueryable<TEntity> AsQueryable();

    IQueryable IQueryableRepository.AsQueryable() => this.AsQueryable();

}

