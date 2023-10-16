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

using MongoDB.Driver;
using Neuroglia.Data.Infrastructure.Mongo.Services;

namespace Neuroglia.Data.Infrastructure.Mongo.Configuration;

/// <summary>
/// Represents the options used to configure an <see cref="MongoRepository{TEntity, TKey}"/>
/// </summary>
public class MongoRepositoryOptions
{

    /// <summary>
    /// Gets/sets the <see cref="MongoRepository{TEntity, TKey}"/>'s <see cref="MongoCollectionSettings"/>
    /// </summary>
    public virtual MongoCollectionSettings CollectionSettings { get; set; } = new();

}

/// <summary>
/// Represents the options used to configure an <see cref="MongoRepository{TEntity, TKey}"/>
/// </summary>
/// <typeparam name="TEntity">The type of entities managed by the repository</typeparam>
/// <typeparam name="TKey">The type of key used to uniquely identify entities managed by the repository</typeparam>
public class MongoRepositoryOptions<TEntity, TKey>
    : MongoRepositoryOptions
{



}
