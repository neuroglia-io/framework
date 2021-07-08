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
    /// Represents the default implementation of the <see cref="ICollectionBuilder"/> interface
    /// </summary>
    /// <typeparam name="TDocument">The type of document to build the collection for</typeparam>
    public class CollectionBuilder<TDocument>
        : ICollectionBuilder<TDocument>
    {

        /// <summary>
        /// Initializes a new <see cref="CollectionBuilder{TDocument}"/>
        /// </summary>
        /// <param name="dbContext">The <see cref="MongoDbContext"/> to build the <see cref="IMongoCollection{TDocument}"/> for</param>
        public CollectionBuilder(MongoDbContext dbContext)
        {
            this.DbContext = dbContext;
            this.CollectionName = this.DbContext.Pluralizer.Pluralize(typeof(TDocument).Name);
            this.OptionsConfiguration = settings => { };
        }

        /// <summary>
        /// Gets the <see cref="MongoDbContext"/> to build the <see cref="IMongoCollection{TDocument}"/> for
        /// </summary>
        protected MongoDbContext DbContext { get; }

        /// <summary>
        /// Gets the name of the collection
        /// </summary>
        protected string CollectionName { get; set; }

        /// <summary>
        /// Gets the <see cref="Action"/> used to configure the collection's <see cref="CreateCollectionOptions"/>
        /// </summary>
        protected Action<CreateCollectionOptions> OptionsConfiguration { get; set; }

        /// <inheritdoc/>
        public ICollectionBuilder<TDocument> WithName(string collectionName)
        {
            if (string.IsNullOrWhiteSpace(collectionName))
                throw new ArgumentNullException(nameof(collectionName));
            this.CollectionName = collectionName;
            return this;
        }

        /// <inheritdoc/>
        public ICollectionBuilder<TDocument> WithOptions(Action<CreateCollectionOptions> optionsConfiguration)
        {
            this.OptionsConfiguration = optionsConfiguration ?? throw new ArgumentNullException(nameof(optionsConfiguration));
            return this;
        }

        /// <inheritdoc/>
        public void Build()
        {
            CreateCollectionOptions options = new();
            this.OptionsConfiguration(options);
            this.DbContext.Database.CreateCollection(this.CollectionName, options);
        }

    }


}
