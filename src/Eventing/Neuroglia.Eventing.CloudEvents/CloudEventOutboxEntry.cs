using CloudNative.CloudEvents;
using Neuroglia.Data;
using System.Net.Mime;

namespace Neuroglia.Eventing
{
    /// <summary>
    /// Represents a <see cref="CloudEvent"/> outbox entry
    /// </summary>
    public class CloudEventOutboxEntry
        : Entity<string>
    {

        /// <summary>
        /// Initializes a new <see cref="CloudEventOutboxEntry"/>
        /// </summary>
        protected CloudEventOutboxEntry()
        {
            this.Data = null!;
            this.ContentType = null!;
        }

        /// <summary>
        /// Initializes a new <see cref="CloudEventOutboxEntry"/>
        /// </summary>
        /// <param name="id">The id of the outbound <see cref="CloudEvent"/></param>
        /// <param name="data">The data of the outbound <see cref="CloudEvent"/></param>
        /// <param name="contentType">The outbound <see cref="CloudEvent"/>'s <see cref="System.Net.Mime.ContentType"/></param>
        public CloudEventOutboxEntry(string id, byte[] data, ContentType contentType)
            : base(id)
        {
            Data = data;
            ContentType = contentType.ToString();
        }

        /// <summary>
        /// Gets the data of the outbound <see cref="CloudEvent"/>
        /// </summary>
        public virtual byte[] Data { get; protected set; }

        /// <summary>
        /// Gets the <see cref="CloudEvent"/>'s <see cref="System.Net.Mime.ContentType"/>
        /// </summary>
        public virtual string ContentType { get; protected set; }

    }

}
