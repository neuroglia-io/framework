// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License"),
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization.NodeDeserializers;

namespace Neuroglia.Serialization.Yaml;

/// <summary>
/// Represents the <see href="YamlDotNet">https://github.com/aaubry/YamlDotNet</see> implementation of the <see cref="IYamlSerializer"/>
/// </summary>
/// <remarks>
/// Initializes a new <see cref="YamlSerializer"/>
/// </remarks>
/// <param name="serializer">The underlying <see cref="YamlDotNet.Serialization.ISerializer"/></param>
/// <param name="deserializer">The underlying <see cref="IDeserializer"/></param>
public class YamlSerializer(YamlDotNet.Serialization.ISerializer serializer, IDeserializer deserializer)
        : IYamlSerializer
{

    static readonly YamlDotNet.Serialization.ISerializer DefaultSerializer;
    static readonly IDeserializer DefaultDeserializer;

    /// <summary>
    /// Gets the action used to configure a <see cref="SerializerBuilder"/> by default
    /// </summary>
    public static readonly Action<SerializerBuilder> DefaultSerializerConfiguration = serializer =>
    {
        serializer
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitDefaults | DefaultValuesHandling.OmitEmptyCollections)
            .WithQuotingNecessaryStrings(true)
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .WithTypeConverter(new JsonNodeTypeConverter())
            .WithTypeConverter(new JsonSchemaTypeConverter())
            .WithTypeConverter(new StringEnumSerializer())
            .WithTypeConverter(new UriTypeSerializer())
            .WithTypeConverter(new DateTimeOffsetSerializer())
            .WithTypeConverter(new EquatableListSerializer(() => serializer.Build()))
            .WithTypeConverter(new EquatableDictionarySerializer(() => serializer.Build()));
    };

    /// <summary>
    /// Gets the action used to configure a <see cref="DeserializerBuilder"/> by default
    /// </summary>
    public static readonly Action<DeserializerBuilder> DefaultDeserializerConfiguration = deserializer =>
    {
        deserializer
            .IgnoreUnmatchedProperties()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .WithNodeTypeResolver(new InferTypeResolver())
            .WithNodeDeserializer(
                inner => new StringEnumDeserializer(inner),
                syntax => syntax.InsteadOf<ScalarNodeDeserializer>())
            .WithNodeDeserializer(
                inner => new JsonObjectDeserializer(inner),
                syntax => syntax.InsteadOf<DictionaryNodeDeserializer>())
            .WithNodeDeserializer(
                inner => new JsonSchemaDeserializer(inner),
                syntax => syntax.InsteadOf<JsonObjectDeserializer>());
    };

    static YamlSerializer? _default;
    /// <summary>
    /// Gets the default, globally accessible <see cref="YamlSerializer"/>
    /// </summary>
    public static YamlSerializer Default
    {
        get
        {
            _default ??= new(DefaultSerializer, DefaultDeserializer);
            return _default;
        }
    }

    static YamlSerializer()
    {
        var serializerBuilder = new SerializerBuilder();
        DefaultSerializerConfiguration(serializerBuilder);
        DefaultSerializer = serializerBuilder.Build();

        var deserializerBuilder = new DeserializerBuilder();
        DefaultDeserializerConfiguration(deserializerBuilder);
        DefaultDeserializer = deserializerBuilder.Build();
    }

    /// <summary>
    /// Gets the underlying <see cref="YamlDotNet.Serialization.ISerializer"/>
    /// </summary>
    protected YamlDotNet.Serialization.ISerializer Serializer { get; } = serializer;

    /// <summary>
    /// Gets the underlying <see cref="IDeserializer"/>
    /// </summary>
    protected IDeserializer Deserializer { get; } = deserializer;

    /// <inheritdoc/>
    public virtual bool Supports(string mediaTypeName) => mediaTypeName switch
    {
        "text/yaml" or "text/x-yaml" or "application/yaml" or "application/x-yaml" => true,
        _ => mediaTypeName.EndsWith("+yaml")
    };

    /// <inheritdoc/>
    public virtual void Serialize(object? value, Stream stream, Type? type = null)
    {
        using var textWriter = new StreamWriter(stream, leaveOpen: true);
        this.Serializer.Serialize(textWriter, value, type ?? value?.GetType()!);
    }

    /// <inheritdoc/>
    public virtual string SerializeToText(object? value, Type? type = null) => this.Serializer.Serialize(value);

    /// <summary>
    /// Serializes the specified object to YAML
    /// </summary>
    /// <typeparam name="T">The type of object to serialize</typeparam>
    /// <param name="graph">The object to serialized</param>
    /// <returns>The YAML of the serialized object</returns>
    public virtual string Serialize<T>(T graph) => this.Serializer.Serialize(graph!);

    /// <summary>
    /// Serializes the specified object to YAML
    /// </summary>
    /// <typeparam name="T">The type of object to serialize</typeparam>
    /// <param name="writer">The <see cref="TextWriter"/> to the YAML to</param>
    /// <param name="graph">The object to serialized</param>
    /// <returns>The YAML of the serialized object</returns>
    public virtual void Serialize<T>(TextWriter writer, T graph) => this.Serializer.Serialize(writer, graph!);

    /// <inheritdoc/>
    public virtual object? Deserialize(string input, Type type) => this.Deserializer.Deserialize(input, type);

    /// <inheritdoc/>
    public virtual object? Deserialize(Stream stream, Type type)
    {
        using var streamReader = new StreamReader(stream, leaveOpen: true);
        return this.Deserializer.Deserialize(streamReader, type);
    }

    /// <summary>
    /// Deserializes the specified YAML input
    /// </summary>
    /// <typeparam name="T">The type to deserialize the specified YAML into</typeparam>
    /// <param name="yaml">The YAML input to deserialize</param>
    /// <returns>The deserialized value</returns>
    public virtual T? Deserialize<T>(string yaml) => this.Deserializer.Deserialize<T>(yaml);

    /// <summary>
    /// Deserializes the specified YAML input
    /// </summary>
    /// <typeparam name="T">The type to deserialize the specified YAML into</typeparam>
    /// <param name="reader">The <see cref="TextReader"/> to read the YAML to deserialize</param>
    /// <returns>The deserialized value</returns>
    public virtual T? Deserialize<T>(TextReader reader) => this.Deserializer.Deserialize<T>(reader);

    /// <summary>
    /// Deserializes the specified YAML input
    /// </summary>
    /// <param name="reader">The <see cref="TextReader"/> to read the YAML to deserialize</param>
    /// <param name="type">The type to deserialize the specified YAML into</param>
    /// <returns>The deserialized value</returns>
    public virtual object? Deserialize(TextReader reader, Type type) => this.Deserializer.Deserialize(reader, type);

    /// <summary>
    /// Deserializes the specified YAML input
    /// </summary>
    /// <typeparam name="T">The type to deserialize the specified YAML into</typeparam>
    /// <param name="buffer">The UTF8 encoded YAML input</param>
    /// <returns>The deserialized value</returns>
    public virtual T? Deserialize<T>(ReadOnlySpan<byte> buffer) => this.Deserializer.Deserialize<T>(Encoding.UTF8.GetString(buffer));

}
