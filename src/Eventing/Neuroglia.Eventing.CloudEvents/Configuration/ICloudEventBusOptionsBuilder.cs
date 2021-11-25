using CloudNative.CloudEvents;

namespace Neuroglia.Eventing.Configuration
{

    /// <summary>
    /// Defines the fundamentals of a service used to build <see cref="CloudEventBusOptions"/>
    /// </summary>
    public interface ICloudEventBusOptionsBuilder
    {

        /// <summary>
        /// Configures the <see cref="ICloudEventBus"/>'s broker <see cref="Uri"/>
        /// </summary>
        /// <param name="uri">The <see cref="ICloudEventBus"/>'s broker <see cref="Uri"/></param>
        /// <returns>The configured <see cref="ICloudEventBusOptionsBuilder"/></returns>
        ICloudEventBusOptionsBuilder WithBrokerUri(Uri uri);

        /// <summary>
        /// Configures the <see cref="ICloudEventBus"/> outbound <see cref="CloudEvent"/> queue
        /// </summary>
        /// <param name="capacity">The capacity of the outbound <see cref="CloudEvent"/> queue. Null specifies an unbounded queue</param>
        /// <returns>The configured <see cref="ICloudEventBusOptionsBuilder"/></returns>
        ICloudEventBusOptionsBuilder WithQueueCapacity(int? capacity);

        /// <summary>
        /// Builds a <see cref="CloudEventBusOptions"/>
        /// </summary>
        /// <returns>New <see cref="CloudEventBusOptions"/></returns>
        CloudEventBusOptions Build();

    }

}
