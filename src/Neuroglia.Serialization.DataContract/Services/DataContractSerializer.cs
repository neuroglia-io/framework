using System.Net.Mime;
using System.Text;

namespace Neuroglia.Serialization.DataContract;

/// <summary>
/// Represents the DataContract implementation of the <see cref="IXmlSerializer"/>
/// </summary>
public class DataContractSerializer
    : IXmlSerializer
{

    /// <inheritdoc/>
    public virtual bool Supports(string mediaTypeName) => mediaTypeName switch
    {
        MediaTypeNames.Application.Xml or MediaTypeNames.Text.Xml => true,
        _ => mediaTypeName.EndsWith("+xml")
    };

    /// <inheritdoc/>
    public virtual void Serialize(object? value, Stream stream, Type? type = null) => new System.Runtime.Serialization.DataContractSerializer(type ?? value?.GetType()!).WriteObject(stream, value);

    /// <inheritdoc/>
    public virtual string SerializeToText(object? value, Type? type = null)
    {
        using var stream = new MemoryStream();
        this.Serialize(value, stream, type);
        stream.Flush();
        stream.Position = 0;
        using var streamReader = new StreamReader(stream);
        return streamReader.ReadToEnd();
    }

    /// <inheritdoc/>
    public virtual object? Deserialize(Stream stream, Type type) => new System.Runtime.Serialization.DataContractSerializer(type).ReadObject(stream);

    /// <inheritdoc/>
    public virtual object? Deserialize(string input, Type type)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));
        return this.Deserialize(stream, type);
    }


}
