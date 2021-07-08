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
using MongoDB.Driver;

namespace Neuroglia.Data
{

    /// <summary>
    /// Represents the options used to configure an <see cref="IMongoDbContext"/>
    /// </summary>
    public class MongoDbContextOptions
    {

        /// <summary>
        /// Gets/sets the connection string used to connect to the remote MongDB
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets/sets the name of the database to connect to
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// Gets/sets the default <see cref="MongoCollectionSettings"/>
        /// </summary>
        public MongoCollectionSettings DefaultCollectionSettings { get; set; }

    }

    /// <summary>
    /// Represents the options used to configure a <see cref="MongoDbContext"/>
    /// </summary>
    /// <typeparam name="TDbContext">The type of <see cref="IMongoDbContext"/> to configure</typeparam>
    public class MongoDbContextOptions<TDbContext>
        : MongoDbContextOptions
        where TDbContext : class, IMongoDbContext
    {



    }

}
