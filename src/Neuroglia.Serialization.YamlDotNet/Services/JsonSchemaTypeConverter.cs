﻿using System.Text.Json.Nodes;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.Schemas;

namespace Neuroglia.Serialization.Yaml;

/// <summary>
/// Represents the <see cref="IYamlTypeConverter"/> used to convert <see cref="JsonSchema"/>s
/// </summary>
public class JsonSchemaTypeConverter
    : IYamlTypeConverter
{

    /// <inheritdoc/>
    public virtual bool Accepts(Type type) => typeof(JsonSchema).IsAssignableFrom(type);

    /// <inheritdoc/>
    public virtual object? ReadYaml(IParser parser, Type type) => throw new NotSupportedException();

    /// <inheritdoc/>
    public virtual void WriteYaml(IEmitter emitter, object? value, Type type)
    {
        var schema = value as JsonSchema;
        if (schema == null) return;
        var node = Json.JsonSerializer.Default.Deserialize<JsonObject>(Json.JsonSerializer.Default.SerializeToText(schema));
        new JsonNodeTypeConverter().WriteYaml(emitter, node, type);
    }

}
