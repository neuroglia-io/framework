namespace Neuroglia;

/// <summary>
/// Represents the <see cref="Attribute"/> used to indicate the marked object should be instantiated using an <see cref="IFactory"/>
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class FactoryAttribute
    : Attribute
{

    /// <summary>
    /// Initializes a new <see cref="FactoryAttribute"/>
    /// </summary>
    /// <param name="factoryType">The type of the <see cref="IFactory"/> to use. Must be a non-generic concrete implementation of the <see cref="IFactory"/> interface</param>
    public FactoryAttribute(Type factoryType)
    {
        if(!factoryType.IsClass || factoryType.IsAbstract || factoryType.IsInterface || factoryType.IsGenericType) throw new ArgumentException($"The specified type '{factoryType.FullName}' is not a non-generic concrete implementation of the {nameof(IFactory)} interface", nameof(factoryType));
        this.FactoryType = factoryType;
    }

    /// <summary>
    /// Gets the type of the <see cref="IFactory"/> to use. Must be a non-generic concrete implementation of the <see cref="IFactory"/> interface
    /// </summary>
    public virtual Type FactoryType { get; }

}
