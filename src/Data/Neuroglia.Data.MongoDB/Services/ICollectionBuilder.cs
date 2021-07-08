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
    /// Defines the fundamentals of a service used to build a <see cref="IMongoCollection{TDocument}"/>
    /// </summary>
    public interface ICollectionBuilder
    {

        /// <summary>
        /// Builds the <see cref="IMongoCollection{TDocument}"/>
        /// </summary>
        void Build();

    }

    /// <summary>
    /// Defines the fundamentals of a service used to build a <see cref="IMongoCollection{TDocument}"/>
    /// </summary>
    /// <typeparam name="TDocument">The type of document to build the collection for</typeparam>
    public interface ICollectionBuilder<TDocument>
        : ICollectionBuilder
    {

        /// <summary>
        /// Sets the name of the collection to build
        /// </summary>
        /// <param name="collectionName">The name of the collection to build</param>
        /// <returns>The configured <see cref="ICollectionBuilder{TDocument}"/></returns>
        ICollectionBuilder<TDocument> WithName(string collectionName);

        /// <summary>
        /// Configures the <see cref="MongoCollectionSettings"/> of the collection to build
        /// </summary>
        /// <param name="optionsConfiguration">An <see cref="Action{T}"/> used to configure the <see cref="CreateCollectionOptions"/> of the collection to build</param>
        /// <returns>The configured <see cref="ICollectionBuilder{TDocument}"/></returns>
        ICollectionBuilder<TDocument> WithOptions(Action<CreateCollectionOptions> optionsConfiguration);

    }

}
