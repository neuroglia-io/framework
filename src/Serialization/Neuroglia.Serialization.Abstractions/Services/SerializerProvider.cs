using System;
using System.Collections.Generic;
using System.Linq;

namespace Neuroglia.Serialization
{
    /// <summary>
    /// Represents the default implementation of the <see cref="ISerializerProvider"/> interface
    /// </summary>
    public class SerializerProvider
        : ISerializerProvider
    {

        /// <summary>
        /// Initializes a new <see cref="SerializerProvider"/>
        /// </summary>
        /// <param name="serializers">An <see cref="IEnumerable{T}"/> containing all registered <see cref="ISerializer"/>s</param>
        public SerializerProvider(IEnumerable<ISerializer> serializers)
        {
            this.Serializers = serializers;
        }

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> containing all registered <see cref="ISerializer"/>s
        /// </summary>
        protected IEnumerable<ISerializer> Serializers { get; }

        /// <inheritdoc/>
        public virtual IEnumerable<IBinarySerializer> GetBinarySerializers()
        {
            return this.Serializers.OfType<IBinarySerializer>();
        }

        /// <inheritdoc/>
        public virtual IEnumerable<IJsonSerializer> GetJsonSerializers()
        {
            return this.Serializers.OfType<IJsonSerializer>();
        }

        /// <inheritdoc/>
        public virtual ISerializer GetSerializer(Type serializerType)
        {
            return this.Serializers.Where(s => serializerType.IsAssignableFrom(s.GetType())).FirstOrDefault();
        }

        /// <inheritdoc/>
        public virtual TSerializer GetSerializer<TSerializer>()
            where TSerializer : class, ISerializer
        {
            return (TSerializer)this.GetSerializer(typeof(TSerializer));
        }

        /// <inheritdoc/>
        public virtual ISerializer GetSerializerFor(string contentType)
        {
            if (string.IsNullOrWhiteSpace(contentType))
                throw new ArgumentNullException(nameof(contentType));
            return this.GetSerializersFor(contentType).FirstOrDefault();
        }

        /// <inheritdoc/>
        public virtual IEnumerable<ISerializer> GetSerializersFor(string contentType)
        {
            if (string.IsNullOrWhiteSpace(contentType))
                throw new ArgumentNullException(nameof(contentType));
            return this.Serializers.Where(s => s.SupportedContentTypes.Contains(contentType));
        }

        /// <inheritdoc/>
        public virtual IEnumerable<ITextSerializer> GetTextSerializers()
        {
            return this.Serializers.OfType<ITextSerializer>();
        }

        /// <inheritdoc/>
        public virtual IEnumerable<IXmlSerializer> GetXmlSerializers()
        {
            return this.Serializers.OfType<IXmlSerializer>();
        }

        /// <inheritdoc/>
        public virtual IEnumerable<IYamlSerializer> GetYamlSerializers()
        {
            return this.Serializers.OfType<IYamlSerializer>();
        }

    }

}
