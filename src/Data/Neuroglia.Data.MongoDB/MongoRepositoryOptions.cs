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

namespace Neuroglia.Data
{

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

}
