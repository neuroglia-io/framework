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

namespace Neuroglia.Data.MongoDB
{

    /// <summary>
    /// Defines the fundamentals of a service used to build <see cref="MongoDbContextOptions"/>
    /// </summary>
    public interface IMongoDbContextOptionsBuilder
    {

        /// <summary>
        /// Uses the specified connection string
        /// </summary>
        /// <param name="connectionString">The connection string to use to connect to the remote MongDb</param>
        /// <returns>The configured <see cref="IMongoDbContextOptionsBuilder"/></returns>
        IMongoDbContextOptionsBuilder UseConnectionString(string connectionString);

        /// <summary>
        /// Uses the specified database
        /// </summary>
        /// <param name="database">The database to connect to</param>
        /// <returns>The configured <see cref="IMongoDbContextOptionsBuilder"/></returns>
        IMongoDbContextOptionsBuilder UseDatabase(string database);

        /// <summary>
        /// Configures the default <see cref="MongoCollectionSettings"/> to use
        /// </summary>
        /// <param name="configurationAction">An <see cref="Action{T}"/> use to configure the default <see cref="MongoCollectionSettings"/> to use</param>
        /// <returns>The configured <see cref="IMongoDbContextOptionsBuilder"/></returns>
        IMongoDbContextOptionsBuilder ConfigureDefaultCollectionSettings(Action<MongoCollectionSettings> configurationAction);

        /// <summary>
        /// Builds the <see cref="MongoDbContextOptions"/>
        /// </summary>
        /// <returns>A new <see cref="MongoDbContextOptions"/></returns>
        MongoDbContextOptions Build();

    }

    /// <summary>
    /// Defines the fundamentals of a service used to build <see cref="MongoDbContextOptions"/>
    /// </summary>
    /// <typeparam name="TDbContext">The type of <see cref="IMongoDbContext"/> to build the <see cref="MongoDbContextOptions"/> for</typeparam>
    public interface IMongoDbContextOptionsBuilder<TDbContext>
        : IMongoDbContextOptionsBuilder
        where TDbContext : class, IMongoDbContext
    {

        /// <summary>
        /// Builds the <see cref="MongoDbContextOptions"/>
        /// </summary>
        /// <returns>A new <see cref="MongoDbContextOptions"/></returns>
        new MongoDbContextOptions<TDbContext> Build();

    }


}
