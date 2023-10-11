using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Mime;

namespace Neuroglia.Serialization.DataContract;

/// <summary>
/// Represents the DataContract implementation of the <see cref="IXmlSerializer"/>
/// </summary>
public class NewtonsoftJsonSerializer
    : IJsonSerializer
{

    /// <summary>
    /// Initializes a new <see cref="NewtonsoftJsonSerializer"/>
    /// </summary>
    /// <param name="settings">The service used to monitor the current <see cref="JsonSerializerSettings"/></param>
    public NewtonsoftJsonSerializer(IOptionsMonitor<JsonSerializerSettings> settings)
    {
        this.Settings = settings;
    }

    /// <summary>
    /// Gets the service used to monitor the current <see cref="JsonSerializerSettings"/>
    /// </summary>
    protected IOptionsMonitor<JsonSerializerSettings> Settings { get; }

    /// <inheritdoc/>
    public virtual bool Supports(string mediaTypeName) => mediaTypeName == MediaTypeNames.Application.Json || mediaTypeName.EndsWith("+json");

    /// <inheritdoc/>
    public virtual void Serialize(object? value, Stream stream, Type? type = null)
    {
        var serializer = JsonSerializer.Create(this.Settings.CurrentValue);
        using var streamWriter = new StreamWriter(stream, leaveOpen: true);
        using var jsonTextWriter = new JsonTextWriter(streamWriter);
        serializer.Serialize(jsonTextWriter, value, type);
    }

    /// <inheritdoc/>
    public virtual string SerializeToText(object? value, Type? type = null) => JsonConvert.SerializeObject(value, type, this.Settings.CurrentValue);

    /// <inheritdoc/>
    public virtual object? Deserialize(string input, Type type) => JsonConvert.DeserializeObject(input, type, this.Settings.CurrentValue);

    /// <inheritdoc/>
    public virtual object? Deserialize(Stream stream, Type type)
    {
        var serializer = JsonSerializer.Create(this.Settings.CurrentValue);
        using var streamReader = new StreamReader(stream, leaveOpen: true);
        using var jsonTextReader = new JsonTextReader(streamReader);
        return serializer.Deserialize(jsonTextReader, type);
    }

}
