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
using System;

namespace Neuroglia.Data.MongoDB
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IMongoDbContextOptionsBuilder{TDbContext}"/>
    /// </summary>
    /// <typeparam name="TDbContext">The type of <see cref="IMongoDbContext"/> to build the <see cref="MongoDbContextOptions"/> for</typeparam>
    public class MongoDbContextOptionsBuilder<TDbContext>
        : IMongoDbContextOptionsBuilder<TDbContext>
        where TDbContext : class, IMongoDbContext
    {

        /// <summary>
        /// Initializes a new <see cref="MongoDbContextOptionsBuilder{TDbContext}"/>
        /// </summary>
        /// <param name="options">The <see cref="MongoDbContextOptions{TDbContext}"/> to configure</param>
        public MongoDbContextOptionsBuilder(MongoDbContextOptions<TDbContext> options)
        {
            this.Options = options;
        }

        /// <summary>
        /// Initializes a new <see cref="MongoDbContextOptionsBuilder{TDbContext}"/>
        /// </summary>
        public MongoDbContextOptionsBuilder()
            : this(new MongoDbContextOptions<TDbContext>())
        {

        }

        /// <summary>
        /// Gets the <see cref="MongoDbContextOptions{TDbContext}"/> to configure
        /// </summary>
        protected MongoDbContextOptions<TDbContext> Options { get; }

        /// <inheritdoc/>
        public virtual IMongoDbContextOptionsBuilder UseConnectionString(string connectionString)
        {
            this.Options.ConnectionString = connectionString;
            return this;
        }

        /// <inheritdoc/>
        public virtual IMongoDbContextOptionsBuilder UseDatabase(string database)
        {
            this.Options.DatabaseName = database;
            return this;
        }

        /// <inheritdoc/>
        public virtual IMongoDbContextOptionsBuilder ConfigureDefaultCollectionSettings(Action<MongoCollectionSettings> configurationAction)
        {
            MongoCollectionSettings settings = new();
            configurationAction(settings);
            this.Options.DefaultCollectionSettings = settings;
            return this;
        }

        /// <inheritdoc/>
        public virtual MongoDbContextOptions<TDbContext> Build()
        {
            return this.Options;
        }

        MongoDbContextOptions IMongoDbContextOptionsBuilder.Build()
        {
            return this.Build();
        }

    }


}
