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
    /// Defines the fundamentals of a service used to build a MongoDB view
    /// </summary>
    public interface IViewBuilder
    {

        /// <summary>
        /// Builds the MongoDB view
        /// </summary>
        void Build();

    }

    /// <summary>
    /// Defines the fundamentals of a service used to build a MongoDB view
    /// </summary>
    /// <typeparam name="TDocument">The type of document to build a new view for</typeparam>
    /// <typeparam name="TView">The type of the view to build</typeparam>
    public interface IViewBuilder<TDocument, TView>
        : IViewBuilder
    {

        /// <summary>
        /// Sets the name of the view to build
        /// </summary>
        /// <param name="viewName">The name of the view to build</param>
        /// <returns>The configured <see cref="IViewBuilder{TDocument, TView}"/></returns>
        IViewBuilder<TDocument, TView> WithViewName(string viewName);

        /// <summary>
        /// Sets the name of the collection to view
        /// </summary>
        /// <param name="collectionName">The name of the collection to view</param>
        /// <returns>The configured <see cref="IViewBuilder{TDocument, TView}"/></returns>
        IViewBuilder<TDocument, TView> WithCollectionName(string collectionName);

        /// <summary>
        /// Uses the specified <see cref="PipelineDefinition{TInput, TOutput}"/> to build the view
        /// </summary>
        /// <param name="pipeline">The <see cref="PipelineDefinition{TInput, TOutput}"/> to use to build the view</param>
        /// <returns>The configured <see cref="IViewBuilder{TDocument, TView}"/></returns>
        IViewBuilder<TDocument, TView> UsePipeline(PipelineDefinition<TDocument, TView> pipeline);

        /// <summary>
        /// Configures the <see cref="CreateViewOptions{TDocument}"/> to use
        /// </summary>
        /// <param name="optionsConfiguration">An <see cref="Action{T}"/> used to configure <see cref="CreateViewOptions{TDocument}"/> to use</param>
        /// <returns>The configured <see cref="IViewBuilder{TDocument, TView}"/></returns>
        IViewBuilder<TDocument, TView> ConfigureCreateViewOptions(Action<CreateViewOptions<TDocument>> optionsConfiguration);

    }

}
