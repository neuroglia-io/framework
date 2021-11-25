using CloudNative.CloudEvents;
using Neuroglia.Eventing.Services;

namespace Neuroglia.Eventing.Configuration
{

    /// <summary>
    /// Represents the options used to configure a <see cref="CloudEventBus"/>
    /// </summary>
    public class CloudEventBusOptions
    {

        /// <summary>
        /// Gets/sets the <see cref="CloudEvent"/> broker <see cref="Uri"/>
        /// </summary>
        public virtual Uri BrokerUri { get; set; } = null!;

        /// <summary>
        /// Gets/sets the capacity of the <see cref="CloudEventBus"/>'s outbound <see cref="CloudEvent"/>s queue. Null defines an outbounded queue.
        /// </summary>
        public virtual int? QueueCapacity { get; set; }

    }

}
