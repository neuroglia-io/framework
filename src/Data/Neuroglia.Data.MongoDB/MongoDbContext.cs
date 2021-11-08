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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Core.Extensions.DiagnosticSources;
using MongoDB.Driver.Linq;
using Neuroglia.Data.MongoDB;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.Data
{

    /// <summary>
    /// Represents the default implementation of the <see cref="IMongoDbContext"/> interface
    /// </summary>
    public abstract class MongoDbContext
        : IMongoDbContext
    {

        /// <summary>
        /// Initializes a new <see cref="MongoDbContext"/>
        /// </summary>
        /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
        /// <param name="options">The service used to access the current <see cref="MongoDbContextOptions"/></param>
        /// <param name="pluralizer">The service used to pluralize words</param>
        protected MongoDbContext(ILoggerFactory loggerFactory, IOptions<MongoDbContextOptions> options, IPluralizer pluralizer)
        {
            this.Logger = loggerFactory.CreateLogger(this.GetType());
            this.Options = options.Value;
            MongoClientSettings clientSettings = MongoClientSettings.FromConnectionString(this.Options.ConnectionString);
            clientSettings.ClusterConfigurator = builder => builder.Subscribe(new DiagnosticsActivityEventSubscriber());
            clientSettings.LinqProvider = LinqProvider.V3;
            this.Client = new MongoClient(clientSettings);
            this.Database = this.Client.GetDatabase(this.Options.DatabaseName);
            this.Pluralizer = pluralizer;
        }

        /// <summary>
        /// Gets the service used to perform logging
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the current <see cref="MongoDbContextOptions"/>
        /// </summary>
        public MongoDbContextOptions Options { get; }

        /// <summary>
        /// Gets the underlying <see cref="MongoClient"/>
        /// </summary>
        public MongoClient Client { get; }

        /// <summary>
        /// Gets the underlying <see cref="IMongoDatabase"/>
        /// </summary>
        public IMongoDatabase Database { get; }

        /// <summary>
        /// Gets the service used to pluralize words
        /// </summary>
        public IPluralizer Pluralizer { get; }

        /// <inheritdoc/>
        public virtual async Task<bool> EnsureCreatedAsync(CancellationToken cancellationToken = default)
        {
            if ((await this.Client.ListDatabaseNames(cancellationToken)
                .ToListAsync(cancellationToken))
                .Contains(this.Options.DatabaseName))
                return true;
            IModelBuilder modelBuilder = new ModelBuilder(this);
            this.OnModelCreating(modelBuilder);
            modelBuilder.Build();
            return false;
        }

        /// <inheritdoc/>
        public virtual IMongoCollection<TDocument> Collection<TDocument>()
        {
            return this.Collection<TDocument>(this.Pluralizer.Pluralize(typeof(TDocument).Name));
        }

        /// <inheritdoc/>
        public virtual IMongoCollection<TDocument> Collection<TDocument>(string collectionName)
        {
            return this.Database.GetCollection<TDocument>(collectionName, this.Options.DefaultCollectionSettings);
        }

        /// <summary>
        /// Configures the <see cref="MongoDbContext"/>'s model
        /// </summary>
        /// <param name="modelBuilder">The service used to build the <see cref="MongoDbContext"/>'s model</param>
        protected virtual void OnModelCreating(IModelBuilder modelBuilder)
        {

        }

        private bool _Disposed;
        /// <summary>
        /// Disposes of the <see cref="MongoDbContext"/>
        /// </summary>
        /// <param name="disposing">A boolean indicating whether or not the <see cref="MongoDbContext"/> is being disposed of</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._Disposed)
            {
                if (disposing)
                {

                }
                this._Disposed = true;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

    }

}
