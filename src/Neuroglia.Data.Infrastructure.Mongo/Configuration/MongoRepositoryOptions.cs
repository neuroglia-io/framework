using MongoDB.Driver;
using Neuroglia.Data.Infrastructure.Mongo.Services;

namespace Neuroglia.Data.Infrastructure.Mongo.Configuration;

/// <summary>
/// Represents the options used to configure an <see cref="MongoRepository{TEntity, TKey}"/>
/// </summary>
public class MongoRepositoryOptions
{

    /// <summary>
    /// Gets/sets the <see cref="MongoRepository{TEntity, TKey}"/>'s <see cref="MongoCollectionSettings"/>
    /// </summary>
    public virtual MongoCollectionSettings CollectionSettings { get; set; } = new();

}

/// <summary>
/// Represents the options used to configure an <see cref="MongoRepository{TEntity, TKey}"/>
/// </summary>
/// <typeparam name="TEntity">The type of entities managed by the repository</typeparam>
/// <typeparam name="TKey">The type of key used to uniquely identify entities managed by the repository</typeparam>
public class MongoRepositoryOptions<TEntity, TKey>
    : MongoRepositoryOptions
{



}
