namespace Neuroglia.Eventing.Configuration
{
    /// <summary>
    /// Represents the default implementation of the <see cref="ICloudEventBusOptionsBuilder"/> interface
    /// </summary>
    public class CloudEventBusOptionsBuilder
        : ICloudEventBusOptionsBuilder
    {

        /// <summary>
        /// Gets the <see cref="CloudEventBusOptions"/> to configure
        /// </summary>
        protected CloudEventBusOptions Options { get; } = new();

        /// <inheritdoc/>
        public virtual ICloudEventBusOptionsBuilder WithBrokerUri(Uri uri)
        {
            this.Options.BrokerUri = uri;
            return this;
        }

        /// <inheritdoc/>
        public virtual ICloudEventBusOptionsBuilder WithQueueCapacity(int? capacity)
        {
            this.Options.QueueCapacity = capacity;
            return this;
        }

        /// <inheritdoc/>
        public virtual CloudEventBusOptions Build()
        {
            return this.Options;
        }

    }

}
