using System;
using System.Collections.Generic;

namespace Neuroglia.Serialization
{

    /// <summary>
    /// Defines the fundamentals of a service used to provide <see cref="ISerializer"/>s
    /// </summary>
    public interface ISerializerProvider
    {

        /// <summary>
        /// Gets all registered <see cref="IBinarySerializer"/>s
        /// </summary>
        /// <returns>A new <see cref="IEnumerable{T}"/> containing all registered <see cref="IBinarySerializer"/>s</returns>
        IEnumerable<IBinarySerializer> GetBinarySerializers();

        /// <summary>
        /// Gets all registered <see cref="ITextSerializer"/>s
        /// </summary>
        /// <returns>A new <see cref="IEnumerable{T}"/> containing all registered <see cref="ITextSerializer"/>s</returns>
        IEnumerable<ITextSerializer> GetTextSerializers();

        /// <summary>
        /// Gets all registered <see cref="IJsonSerializer"/>s
        /// </summary>
        /// <returns>A new <see cref="IEnumerable{T}"/> containing all registered <see cref="IJsonSerializer"/>s</returns>
        IEnumerable<IJsonSerializer> GetJsonSerializers();

        /// <summary>
        /// Gets all registered <see cref="IYamlSerializer"/>s
        /// </summary>
        /// <returns>A new <see cref="IEnumerable{T}"/> containing all registered <see cref="IYamlSerializer"/>s</returns>
        IEnumerable<IYamlSerializer> GetYamlSerializers();

        /// <summary>
        /// Gets all registered <see cref="IXmlSerializer"/>s
        /// </summary>
        /// <returns>A new <see cref="IEnumerable{T}"/> containing all registered <see cref="IXmlSerializer"/>s</returns>
        IEnumerable<IXmlSerializer> GetXmlSerializers();

        /// <summary>
        /// Gets the <see cref="ISerializer"/>s that support the specified content type
        /// </summary>
        /// <param name="contentType">The content type to serialize/deserialize</param>
        /// <returns>A new <see cref="IEnumerable{T}"/> containing the <see cref="ISerializer"/>s that support the specified content type, if any</returns>
        IEnumerable<ISerializer> GetSerializersFor(string contentType);

        /// <summary>
        /// Gets the first registered <see cref="ISerializer"/> that support the specified content type
        /// </summary>
        /// <param name="contentType">The content type to serialize/deserialize</param>
        /// <returns>The first registered <see cref="ISerializer"/> that support the specified content type, if any</returns>
        ISerializer GetSerializerFor(string contentType);

        /// <summary>
        /// Gets the <see cref="ISerializer"/> of the specified type
        /// </summary>
        /// <param name="serializerType">The type of <see cref="ISerializer"/> to get</param>
        /// <returns>The <see cref="ISerializer"/> of the specified type</returns>
        ISerializer GetSerializer(Type serializerType);

        /// <summary>
        /// Gets the <see cref="ISerializer"/> of the specified type
        /// </summary>
        /// <typeparam name="TSerializer">The type of <see cref="ISerializer"/> to get</typeparam>
        /// <returns>The <see cref="ISerializer"/> of the specified type</returns>
        TSerializer GetSerializer<TSerializer>()
            where TSerializer: class, ISerializer;

    }

}
