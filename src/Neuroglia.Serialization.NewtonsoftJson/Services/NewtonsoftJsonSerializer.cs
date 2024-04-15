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

using Json.More;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Neuroglia.Serialization.DataContract;

/// <summary>
/// Represents the DataContract implementation of the <see cref="IXmlSerializer"/>
/// </summary>
/// <remarks>
/// Initializes a new <see cref="NewtonsoftJsonSerializer"/>
/// </remarks>
/// <param name="settings">The service used to monitor the current <see cref="JsonSerializerSettings"/></param>
public class NewtonsoftJsonSerializer(IOptionsMonitor<JsonSerializerSettings> settings)
    : IJsonSerializer
{

    /// <summary>
    /// Gets the service used to monitor the current <see cref="JsonSerializerSettings"/>
    /// </summary>
    protected IOptionsMonitor<JsonSerializerSettings> Settings { get; } = settings;

    /// <inheritdoc/>
    public virtual bool Supports(string mediaTypeName) => mediaTypeName == MediaTypeNames.Application.Json || mediaTypeName.EndsWith("+json");

    /// <inheritdoc/>
    public virtual void Serialize(object? value, Stream stream, Type? type = null)
    {
        var serializer = Newtonsoft.Json.JsonSerializer.Create(this.Settings.CurrentValue);
        using var streamWriter = new StreamWriter(stream, leaveOpen: true);
        using var jsonTextWriter = new JsonTextWriter(streamWriter);
        serializer.Serialize(jsonTextWriter, value, type);
    }

    /// <inheritdoc/>
    public virtual string SerializeToText(object? value, Type? type = null) => JsonConvert.SerializeObject(value, type, this.Settings.CurrentValue);

    /// <inheritdoc/>
    public virtual JsonNode? SerializeToNode<T>(T graph) => graph == null ? null : JsonNode.Parse(this.SerializeToText(graph));

    /// <inheritdoc/>
    public virtual JsonElement? SerializeToElement<T>(T graph) 
    {
        if (graph == null) return null;
        var reader = new Utf8JsonReader(this.SerializeToByteArray(graph));
        return JsonElement.ParseValue(ref reader);
    }

    /// <inheritdoc/>
    public virtual JsonDocument? SerializeToDocument<T>(T graph) => graph == null ? null : JsonDocument.Parse(this.SerializeToText(graph));

    /// <inheritdoc/>
    public virtual object? Deserialize(string input, Type type) => JsonConvert.DeserializeObject(input, type, this.Settings.CurrentValue);

    /// <inheritdoc/>
    public virtual T? Deserialize<T>(string input) => JsonConvert.DeserializeObject<T>(input, this.Settings.CurrentValue);

    /// <inheritdoc/>
    public virtual object? Deserialize(Stream stream, Type type)
    {
        var serializer = Newtonsoft.Json.JsonSerializer.Create(this.Settings.CurrentValue);
        using var streamReader = new StreamReader(stream, leaveOpen: true);
        using var jsonTextReader = new JsonTextReader(streamReader);
        return serializer.Deserialize(jsonTextReader, type);
    }

    /// <inheritdoc/>
    public virtual object? Deserialize(JsonElement element, Type type)
    {
        var json = element.ToJsonString();
        return this.Deserialize(json, type); 
    }

    /// <inheritdoc/>
    public virtual T? Deserialize<T>(JsonElement element) => (T?)this.Deserialize(element, typeof(T));

    /// <inheritdoc/>
    public virtual object? Deserialize(JsonNode node, Type type)
    {
        var json = node.ToJsonString(Json.JsonSerializer.DefaultOptions);
        return this.Deserialize(json, type);
    }

    /// <inheritdoc/>
    public virtual T? Deserialize<T>(JsonNode node) => (T?)this.Deserialize(node, typeof(T));

    /// <inheritdoc/>
    public virtual IAsyncEnumerable<T?> DeserializeAsyncEnumerable<T>(Stream stream, CancellationToken cancellationToken = default) => Newtonsoft.Json.JsonSerializer.Create(this.Settings.CurrentValue).DeserializeAsyncEnumerable<T>(stream, cancellationToken);

}
