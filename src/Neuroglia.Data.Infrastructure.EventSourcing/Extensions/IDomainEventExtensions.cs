namespace Neuroglia.Data.Infrastructure.EventSourcing;

/// <summary>
/// Defines extensions for <see cref="IDomainEvent"/>s
/// </summary>
public static class IDomainEventExtensions
{

    /// <summary>
    /// Gets the <see cref="IDomainEvent"/>'s <see cref="IEventDescriptor"/>
    /// </summary>
    /// <param name="e">The <see cref="IDomainEvent"/> to get the <see cref="IEventDescriptor"/> of</param>
    /// <returns>The <see cref="IEventDescriptor"/> of the specified <see cref="IDomainEvent"/></returns>
    public static IEventDescriptor GetDescriptor(this IDomainEvent e)
    {
        if (e == null) throw new ArgumentNullException(nameof(e));
        return new EventDescriptor(e.GetTypeName(), e);
    }

    /// <summary>
    /// Gets the <see cref="IDomainEvent"/>'s type name
    /// </summary>
    /// <param name="e">The <see cref="IDomainEvent"/> to get the type name of</param>
    /// <returns>The <see cref="IDomainEvent"/>'s type name</returns>
    public static string GetTypeName(this IDomainEvent e)
    {
        if (e == null) throw new ArgumentNullException(nameof(e));
        return e.GetType().Name.Replace("DomainEvent", "").SplitCamelCase().ToKebabCase();
    }

}
