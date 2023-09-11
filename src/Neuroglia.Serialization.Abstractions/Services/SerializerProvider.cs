using Microsoft.Extensions.DependencyInjection;

namespace Neuroglia.Serialization;

/// <summary>
/// Represents the default implementation of the <see cref="ISerializerProvider"/> interface
/// </summary>
public class SerializerProvider
    : ISerializerProvider
{

    /// <summary>
    /// Initializes a new <see cref="SerializerProvider"/>
    /// </summary>
    /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
    public SerializerProvider(IServiceProvider serviceProvider)
    {
        this.ServiceProvider = serviceProvider;
    }

    /// <summary>
    /// Gets the current <see cref="IServiceProvider"/>
    /// </summary>
    protected IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Gets an <see cref="IEnumerable{T}"/> containing all configured <see cref="ISerializer"/>s
    /// </summary>
    protected IEnumerable<ISerializer> Serializers => this.ServiceProvider.GetServices<ISerializer>();

    /// <inheritdoc/>
    public IEnumerable<ISerializer> GetSerializers() => this.Serializers;

    /// <inheritdoc/>
    public IEnumerable<ISerializer> GetSerializersFor(string mediaTypeName) => this.Serializers.Where(s => s.Supports(mediaTypeName));

}