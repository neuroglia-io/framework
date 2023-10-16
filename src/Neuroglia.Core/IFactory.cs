namespace Neuroglia;

/// <summary>
/// Defines the fundamentals of a service used to create object instances
/// </summary>
public interface IFactory
{

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <returns>A new instance</returns>
    object Create();

}

/// <summary>
/// Defines the fundamentals of a service used to create object instances
/// </summary>
/// <typeparam name="T">The type of the object to create</typeparam>
public interface IFactory<T>
    : IFactory
{

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <returns>A new instance</returns>
    new T Create();

}
