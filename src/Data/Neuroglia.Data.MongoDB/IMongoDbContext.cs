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
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.Data
{

    /// <summary>
    /// Defines the fundamentals of a <see href="https://www.mongodb.com/">MongoDB</see> context
    /// </summary>
    public interface IMongoDbContext
        : IDisposable
    {

        /// <summary>
        /// Ensures that the related Mongo database has been created
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A boolean indicating whether or not the Mongo database existed before the call</returns>
        Task<bool> EnsureCreatedAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the <see cref="IMongoCollection{TDocument}"/> that manages the specified type of document
        /// </summary>
        /// <typeparam name="TDocument">The type of document for which to get the <see cref="IMongoCollection{TDocument}"/></typeparam>
        /// <returns>The <see cref="IMongoCollection{TDocument}"/> that manages the specified document</returns>
        IMongoCollection<TDocument> Collection<TDocument>();

        /// <summary>
        /// Gets the <see cref="IMongoCollection{TDocument}"/> with the specified name that manages the specified type of document
        /// </summary>
        /// <typeparam name="TDocument">The type of document for which to get the <see cref="IMongoCollection{TDocument}"/></typeparam>
        /// <param name="collectionName">The name of the <see cref="IMongoCollection{TDocument}"/> to get</param>
        /// <returns>The <see cref="IMongoCollection{TDocument}"/> that manages the specified document</returns>
        IMongoCollection<TDocument> Collection<TDocument>(string collectionName);

    }

}
