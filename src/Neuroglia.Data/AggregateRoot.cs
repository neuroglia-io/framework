namespace Neuroglia.Data;

/// <summary>
/// Represents the default implementation of the <see cref="IAggregateRoot"/> interface
/// </summary>
/// <typeparam name="TKey">The type of key used to uniquely identify the <see cref="IAggregateRoot"/></typeparam>
public abstract class AggregateRoot<TKey>
    : Entity<TKey>, IAggregateRoot<TKey>
    where TKey : IEquatable<TKey>
{

    /// <summary>
    /// Initializes a new <see cref="AggregateRoot{TKey}"/>
    /// </summary>
    protected AggregateRoot() { }

    /// <summary>
    /// Initializes a new <see cref="AggregateRoot{TKey}"/>
    /// </summary>
    /// <param name="id">The <see cref="AggregateRoot{TKey}"/>'s unique identifier</param>
    protected AggregateRoot(TKey id)
        : base(id)
    {

    }

    private readonly List<IDomainEvent> _pendingEvents = new();
    /// <inheritdoc/>
    public virtual IReadOnlyList<IDomainEvent> PendingEvents => this._pendingEvents.AsReadOnly();

    /// <summary>
    /// Registers the specified <see cref="IDomainEvent"/>
    /// </summary>
    /// <typeparam name="TEvent">The type of <see cref="IDomainEvent"/> to register</typeparam>
    /// <param name="e">The <see cref="IDomainEvent"/> to register</param>
    /// <returns>The registered <see cref="IDomainEvent"/></returns>
    protected virtual TEvent RegisterEvent<TEvent>(TEvent e)
        where TEvent : IDomainEvent
    {
        if (e == null) throw new ArgumentNullException(nameof(e));
        this._pendingEvents.Add(e);
        return e;
    }

    /// <inheritdoc/>
    public virtual void ClearPendingEvents()
    {
        this._pendingEvents.Clear();
    }

}