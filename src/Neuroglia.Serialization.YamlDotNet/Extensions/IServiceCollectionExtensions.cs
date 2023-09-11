using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Neuroglia.Serialization.Yaml;
using YamlDotNet.Serialization;

namespace Neuroglia.Serialization;

/// <summary>
/// Defines extensions for <see cref="IServiceCollection"/>s
/// </summary>
public static class IServiceCollectionExtensions
{

    /// <summary>
    /// Adds and configure a <see cref="YamlDotNetSerializer"/>
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="setup">An <see cref="Action{T}"/> used to configure the <see cref="YamlDotNetSerializer"/> to use</param>
    /// <param name="lifetime">The <see cref="YamlDotNetSerializer"/>'s lifetime</param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddYamlDotNetSerializer(this IServiceCollection services, Action<IYamlDotNetSerializerBuilder>? setup = null, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        var builder = new YamlDotNetSerializerBuilder();
        setup?.Invoke(builder);
        services.TryAdd(new ServiceDescriptor(typeof(YamlDotNet.Serialization.ISerializer), builder.Serializer.Build()));
        services.TryAdd(new ServiceDescriptor(typeof(IDeserializer), builder.Deserializer.Build()));
        services.AddSerializer<YamlDotNetSerializer>(lifetime);
        return services;
    }

}