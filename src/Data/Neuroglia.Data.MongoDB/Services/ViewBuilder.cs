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
    /// Represents the default implementation of the <see cref="IViewBuilder"/> interface
    /// </summary>
    /// <typeparam name="TDocument">The type of document to build a new view for</typeparam>
    /// <typeparam name="TView">The type of the view to build</typeparam>
    public class ViewBuilder<TDocument, TView>
        : IViewBuilder<TDocument, TView>
    {

        /// <summary>
        /// Initializes a new <see cref="ViewBuilder{TDocument, TView}"/>
        /// </summary>
        /// <param name="dbContext">The <see cref="MongoDbContext"/> to build the view for</param>
        public ViewBuilder(MongoDbContext dbContext)
        {
            this.DbContext = dbContext;
            this.CollectionName = this.DbContext.Pluralizer.Pluralize(typeof(TDocument).Name);
            this.ViewName = this.DbContext.Pluralizer.Pluralize(typeof(TView).Name);
            this.CreateViewOptionsConfiguration = options => { };
        }

        /// <summary>
        /// Gets the <see cref="MongoDbContext"/> to build the view for
        /// </summary>
        protected MongoDbContext DbContext { get; }

        /// <summary>
        /// Gets/sets the name of the collection to view
        /// </summary>
        protected string CollectionName { get; set; }

        /// <summary>
        /// Gets/sets the name of the view to build
        /// </summary>
        protected string ViewName { get; set; }

        /// <summary>
        /// Gets/sets the <see cref="PipelineDefinition{TInput, TOutput}"/> used to generate the view to build
        /// </summary>
        protected PipelineDefinition<TDocument, TView> Pipeline { get; set; }

        /// <summary>
        /// Gets/sets the <see cref="Action{T}"/> used to configure <see cref="CreateViewOptions{TDocument}"/> to use
        /// </summary>
        protected Action<CreateViewOptions<TDocument>> CreateViewOptionsConfiguration { get; set; }

        /// <inheritdoc/>
        public IViewBuilder<TDocument, TView> WithCollectionName(string collectionName)
        {
            if (string.IsNullOrWhiteSpace(collectionName))
                throw new ArgumentNullException(nameof(collectionName));
            this.CollectionName = collectionName;
            return this;
        }

        /// <inheritdoc/>
        public IViewBuilder<TDocument, TView> WithViewName(string viewName)
        {
            if (string.IsNullOrWhiteSpace(viewName))
                throw new ArgumentNullException(nameof(viewName));
            this.ViewName = viewName;
            return this;
        }

        /// <inheritdoc/>
        public IViewBuilder<TDocument, TView> UsePipeline(PipelineDefinition<TDocument, TView> pipeline)
        {
            this.Pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
            return this;
        }

        /// <inheritdoc/>
        public IViewBuilder<TDocument, TView> ConfigureCreateViewOptions(Action<CreateViewOptions<TDocument>> optionsConfiguration)
        {
            this.CreateViewOptionsConfiguration = optionsConfiguration ?? throw new ArgumentNullException(nameof(optionsConfiguration));
            return this;
        }

        /// <inheritdoc/>
        public void Build()
        {
            if (this.Pipeline == null)
                throw new Exception($"The {nameof(Pipeline)} must be set");
            CreateViewOptions<TDocument> options = new();
            this.CreateViewOptionsConfiguration(options);
            this.DbContext.Database.CreateView(this.ViewName, this.CollectionName, this.Pipeline, options);
        }

    }


}
