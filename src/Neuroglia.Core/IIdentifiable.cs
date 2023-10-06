namespace Neuroglia;

/// <summary>
/// Defines the fundamentals of an object that is uniquely identifiable
/// </summary>
public interface IIdentifiable
{

    /// <summary>
    /// Gets the object's unique identifier
    /// </summary>
    object Id { get; }

}

/// <summary>
/// Defines the fundamentals of an object that is uniquely identifiable
/// </summary>
/// <typeparam name="TKey">The type of key used to uniquely identify the object</typeparam>
public interface IIdentifiable<TKey>
    : IIdentifiable, IEquatable<IIdentifiable<TKey>>
    where TKey : IEquatable<TKey>
{

    /// <summary>
    /// Gets the object's unique identifier
    /// </summary>
    new TKey Id { get; }

}
