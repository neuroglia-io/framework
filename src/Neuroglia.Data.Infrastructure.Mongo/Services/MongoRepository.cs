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
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.WireProtocol.Messages;
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
    : QueryableRepositoryBase<TEntity, TKey>, IDisposable
    where TEntity : class, IIdentifiable<TKey>
    where TKey : IEquatable<TKey>
{

    bool _disposed;
    bool? _supportsTransactions;

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
    
    /// <summary>
    /// Gets a boolean indicating whether or not the <see cref="MongoRepository{TEntity, TKey}"/> supports transactions
    /// </summary>
    protected bool SupportsTransactions
    {
        get
        {
            if (!this._supportsTransactions.HasValue)
            {
                this._supportsTransactions = this.Database.Client
                    .GetDatabase("admin")
                    .RunCommand<BsonDocument>(BsonDocument.Parse("{ getCmdLineOpts: 1 }"))
                    .Contains("replSet");
            }
            return this._supportsTransactions.Value;
        }
    }

    /// <summary>
    /// Gets the current <see cref="IClientSessionHandle"/>, if any
    /// </summary>
    protected IClientSessionHandle? CurrentSession { get; private set; }

    /// <inheritdoc/>
    public override Task<bool> ContainsAsync(TKey key, CancellationToken cancellationToken = default) => this.Collection.AsQueryable().AnyAsync(e => e.Id.Equals(key), cancellationToken);

    /// <inheritdoc/>
    public override async Task<TEntity?> GetAsync(TKey key, CancellationToken cancellationToken = default) => (await this.Collection.FindAsync(d => d.Id.Equals(key), new FindOptions<TEntity, TEntity>(), cancellationToken).ConfigureAwait(false)).FirstOrDefault(cancellationToken);

    /// <inheritdoc/>
    public override async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        if (this.SupportsTransactions)
        {
            this.CurrentSession ??= await this.Database.Client.StartSessionAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            if (!this.CurrentSession.IsInTransaction) this.CurrentSession.StartTransaction();
        }

        var options = new InsertOneOptions();
        if (this.SupportsTransactions) await this.Collection.InsertOneAsync(this.CurrentSession, entity, options, cancellationToken).ConfigureAwait(false);
        else await this.Collection.InsertOneAsync(entity, options, cancellationToken).ConfigureAwait(false);

        return (await this.GetAsync(entity.Id, cancellationToken).ConfigureAwait(false))!;
    }

    /// <inheritdoc/>
    public override async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        if (this.SupportsTransactions)
        {
            this.CurrentSession ??= await this.Database.Client.StartSessionAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            if (!this.CurrentSession.IsInTransaction) this.CurrentSession.StartTransaction();
        }

        var options = new ReplaceOptions();
        var result = this.SupportsTransactions
            ? await this.Collection.ReplaceOneAsync(this.CurrentSession, d => d.Id.Equals(entity.Id), entity, options, cancellationToken).ConfigureAwait(false)
            : await this.Collection.ReplaceOneAsync(d => d.Id.Equals(entity.Id), entity, options, cancellationToken).ConfigureAwait(false);

        Guard.AgainstArgument(result.ModifiedCount == 1 ? entity : null, nameof(entity)).WhenNullReference(entity.Id);

        return (await this.GetAsync(entity.Id, cancellationToken).ConfigureAwait(false))!;
    }

    /// <inheritdoc/>
    public override async Task<bool> RemoveAsync(TKey key, CancellationToken cancellationToken = default)
    {
        if (this.SupportsTransactions)
        {
            this.CurrentSession ??= await this.Database.Client.StartSessionAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            if (!this.CurrentSession.IsInTransaction) this.CurrentSession.StartTransaction();
        }

        var result = this.SupportsTransactions
            ? await this.Collection.DeleteOneAsync(this.CurrentSession, d => d.Id.Equals(key), cancellationToken: cancellationToken).ConfigureAwait(false)
            : await this.Collection.DeleteOneAsync(d => d.Id.Equals(key), cancellationToken).ConfigureAwait(false);
        return result.DeletedCount > 0;
    }

    /// <inheritdoc/>
    public override Task<bool> RemoveAsync(TEntity entity, CancellationToken cancellationToken = default) => this.RemoveAsync(entity.Id, cancellationToken);

    /// <inheritdoc/>
    public override async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (!this.SupportsTransactions || this.CurrentSession == null || !this.CurrentSession.IsInTransaction) return;
        try
        {
            await this.CurrentSession.CommitTransactionAsync(cancellationToken).ConfigureAwait(false);
        }
        catch(Exception)
        {
            try { await this.CurrentSession.AbortTransactionAsync(cancellationToken).ConfigureAwait(false); } catch { }
            throw;
        }
    }

    /// <inheritdoc/>
    public override IQueryable<TEntity> AsQueryable() => this.Collection.AsQueryable();

    /// <summary>
    /// Disposes of the <see cref="MongoRepository{TEntity, TKey}"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="MongoRepository{TEntity, TKey}"/> is being disposed of</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!this._disposed)
        {
            if (disposing)
            {
                this.CurrentSession?.AbortTransaction();
                this.CurrentSession?.Dispose();
            }
            this._disposed = true;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

}
