using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Neuroglia.Data;

/// <summary>
/// Defines the fundamentals of an entity
/// </summary>
public interface IEntity
    : IIdentifiable, IVersionedState
{

    /// <summary>
    /// Gets the date and time at which the entity has been created
    /// </summary>
    DateTimeOffset CreatedAt { get; }

    /// <summary>
    /// Gets the date and time, if any, at which the entity has last been modified
    /// </summary>
    DateTimeOffset? LastModified { get; }

}

/// <summary>
/// Defines the fundamentals of an entity
/// </summary>
public interface IEntity<TKey>
    : IEntity, IIdentifiable<TKey>
    where TKey : IEquatable<TKey>
{



}