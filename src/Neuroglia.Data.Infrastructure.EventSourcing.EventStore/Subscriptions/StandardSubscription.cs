using EventStore.Client;

namespace Neuroglia.Data.Infrastructure.EventSourcing.EventStore.Subscriptions;

/// <summary>
/// Represents a standard <see cref="EventStoreSubscription"/>
/// </summary>
public class StandardSubscription
    : EventStoreSubscription
{

    /// <summary>
    /// Initializes a new <see cref="StandardSubscription"/>
    /// </summary>
    /// <param name="id">The <see cref="StandardSubscription"/>'s id</param>
    /// <param name="source">The underlying <see cref="StreamSubscription"/></param>
    public StandardSubscription(string id, object source) : base(id, source) { }

    /// <summary>
    /// Gets the underlying <see cref="StreamSubscription"/>
    /// </summary>
    protected new StreamSubscription Source => (StreamSubscription)base.Source;

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            this.Source?.Dispose();
            base.Source = null!;
        }
        base.Dispose(disposing);
    }

}
