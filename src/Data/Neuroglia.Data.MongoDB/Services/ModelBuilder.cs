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
using System.Collections.Generic;
using System.Linq;

namespace Neuroglia.Data.MongoDB
{

    /// <summary>
    /// Represents the default implementation of the <see cref="IModelBuilder"/> interface
    /// </summary>
    public class ModelBuilder
        : IModelBuilder
    {

        /// <summary>
        /// Initializes a new <see cref="ModelBuilder"/>
        /// </summary>
        /// <param name="dbContext">The <see cref="MongoDbContext"/> to build the model for</param>
        internal ModelBuilder(MongoDbContext dbContext)
        {
            this.DbContext = dbContext;
            this.CollectionBuilders = new List<ICollectionBuilder>();
            this.ViewBuilders = new List<IViewBuilder>();
        }

        /// <summary>
        /// Gets the <see cref="MongoDbContext"/> to build the model for
        /// </summary>
        protected MongoDbContext DbContext { get; }

        /// <summary>
        /// Gets a <see cref="List{T}"/> containing all configured <see cref="ICollectionBuilder"/>s
        /// </summary>
        protected List<ICollectionBuilder> CollectionBuilders { get; }

        /// <summary>
        /// Gets a <see cref="List{T}"/> containing all configured <see cref="IViewBuilder"/>s
        /// </summary>
        protected List<IViewBuilder> ViewBuilders { get; }

        /// <inheritdoc/>
        public ICollectionBuilder<TDocument> Document<TDocument>()
        {
            ICollectionBuilder<TDocument> builder = this.CollectionBuilders
                .OfType<ICollectionBuilder<TDocument>>()
                .SingleOrDefault();
            if (builder == null)
            {
                builder = new CollectionBuilder<TDocument>(this.DbContext);
                this.CollectionBuilders.Add(builder);
            }
            return builder;
        }

        /// <inheritdoc/>
        public IViewBuilder<TDocument, TView> View<TDocument, TView>()
        {
            IViewBuilder<TDocument, TView> builder = this.ViewBuilders
                .OfType<IViewBuilder<TDocument, TView>>()
                .SingleOrDefault();
            if (builder == null)
            {
                builder = new ViewBuilder<TDocument, TView>(this.DbContext);
                this.ViewBuilders.Add(builder);
            }
            return builder;
        }

        /// <inheritdoc/>
        public void Build()
        {
            foreach (ICollectionBuilder builder in this.CollectionBuilders)
            {
                builder.Build();
            }
            foreach (IViewBuilder builder in this.ViewBuilders)
            {
                builder.Build();
            }
        }

    }

}
