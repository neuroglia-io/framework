// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License"),
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Neuroglia.Data.Guards;
using Neuroglia.Data.Infrastructure.Mongo.Configuration;
using Neuroglia.Data.Infrastructure.Services;
using Pluralize.NET.Core;

namespace Neuroglia.Data.Infrastructure.Mongo.Services;

/// <summary>
/// Represents the default <see href="https://www.mongodb.com/">MongoDB</see> implementation of the <see cref="IRepository{T, TKey}"/> interface
/// </summary>
/// <typeparam name="TEntity">The type of entities managed by the repository</typeparam>
/// <typeparam name="TKey">The type of key used to uniquely identify entities managed by the repository</typeparam>
public class MongoRepository<TEntity, TKey>
    : QueryableRepositoryBase<TEntity, TKey>
    where TEntity : class, IIdentifiable<TKey>
    where TKey : IEquatable<TKey>
{

    /// <summary>
    /// Initializes a new <see cref="MongoRepository{TDocument, TKey}"/>
    /// </summary>
    /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
    /// <param name="database">The <see cref="IMongoDatabase"/> the <see cref="MongoRepository{TDocument, TKey}"/> belongs to</param>
    /// <param name="options">The service used to access the <see cref="MongoRepository{TEntity, TKey}"/>'s options</param>
    public MongoRepository(ILoggerFactory loggerFactory, IOptions<MongoRepositoryOptions<TEntity, TKey>> options, IMongoDatabase database)
    {
        this.Logger = loggerFactory.CreateLogger(this.GetType());
        this.Options = options.Value;
        this.Pluralizer = new Pluralizer();
        this.Database = database;
        this.Collection = this.Database.GetCollection<TEntity>(this.Pluralizer.Pluralize(typeof(TEntity).Name), this.Options.CollectionSettings);
    }

    /// <summary>
    /// Gets the service used to perform logging
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Gets the options used to configure the <see cref="MongoRepository{TEntity, TKey}"/>
    /// </summary>
    protected MongoRepositoryOptions Options { get; }

    /// <summary>
    /// Gets the service used to pluralize words
    /// </summary>
    public Pluralizer Pluralizer { get; }

    /// <summary>
    /// Gets the service used to interact with the MongoDB database the <see cref="MongoRepository{TEntity, TKey}"/> belongs to
    /// </summary>
    protected IMongoDatabase Database { get; }

    /// <summary>
    /// Gets the underlying <see cref="IMongoCollection{TDocument}"/>
    /// </summary>
    protected IMongoCollection<TEntity> Collection { get; }

    /// <inheritdoc/>
    public override Task<bool> ContainsAsync(TKey key, CancellationToken cancellationToken = default) => this.Collection.AsQueryable().AnyAsync(e => e.Id.Equals(key), cancellationToken);

    /// <inheritdoc/>
    public override async Task<TEntity?> GetAsync(TKey key, CancellationToken cancellationToken = default) => (await this.Collection.FindAsync(d => d.Id.Equals(key), new FindOptions<TEntity, TEntity>(), cancellationToken).ConfigureAwait(false)).FirstOrDefault(cancellationToken);

    /// <inheritdoc/>
    public override async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var options = new InsertOneOptions();
        await this.Collection.InsertOneAsync(entity, options, cancellationToken).ConfigureAwait(false);
        return (await this.GetAsync(entity.Id, cancellationToken).ConfigureAwait(false))!;
    }

    /// <inheritdoc/>
    public override async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var options = new ReplaceOptions();
        var result = await this.Collection.ReplaceOneAsync(d => d.Id.Equals(entity.Id), entity, options, cancellationToken).ConfigureAwait(false);

        Guard.AgainstArgument(result.ModifiedCount == 1 ? entity : null, nameof(entity)).WhenNullReference(entity.Id);

        return (await this.GetAsync(entity.Id, cancellationToken).ConfigureAwait(false))!;
    }

    /// <inheritdoc/>
    public override async Task<bool> RemoveAsync(TKey key, CancellationToken cancellationToken = default)
    {
        var result = await this.Collection.DeleteOneAsync(d => d.Id.Equals(key), cancellationToken).ConfigureAwait(false);
        return result.DeletedCount > 0;
    }

    /// <inheritdoc/>
    public override Task<bool> RemoveAsync(TEntity entity, CancellationToken cancellationToken = default) => this.RemoveAsync(entity.Id, cancellationToken);

    /// <inheritdoc/>
    public override Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    /// <inheritdoc/>
    public override IQueryable<TEntity> AsQueryable() => this.Collection.AsQueryable();

}
