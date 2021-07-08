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
    /// Defines the fundamentals of a service used to build a <see cref="MongoDbContext"/> model
    /// </summary>
    public interface IModelBuilder
    {

        /// <summary>
        /// Builds a collection for the specified document type
        /// </summary>
        /// <typeparam name="TDocument">The type of document to configure the collection for</typeparam>
        /// <returns>A new <see cref="ICollectionBuilder{TDocument}"/> used to build the collection for the specified document type</returns>
        ICollectionBuilder<TDocument> Document<TDocument>();

        /// <summary>
        /// Builds a new view for the specified document type
        /// </summary>
        /// <typeparam name="TDocument">The type of document to build the view for</typeparam>
        /// <typeparam name="TView">The type of view to build</typeparam>
        /// <returns>A new <see cref="IViewBuilder{TDocument, TView}"/> used to configure the view for the specified document type</returns>
        IViewBuilder<TDocument, TView> View<TDocument, TView>();

        /// <summary>
        /// Builds the model of the <see cref="MongoDbContext"/> 
        /// </summary>
        void Build();

    }

}
