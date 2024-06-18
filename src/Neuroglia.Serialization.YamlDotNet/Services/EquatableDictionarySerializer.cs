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

using System.Collections;
using System.Text;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Neuroglia.Serialization.Yaml;

/// <summary>
/// Represents the <see cref="IYamlTypeConverter"/> used to serialize <see cref="EquatableDictionary{TKey, TValue}"/> instances
/// </summary>
/// <param name="serializerFactory">A function used to create the underlying <see cref="YamlDotNet.Serialization.ISerializer"/></param>
public class EquatableDictionarySerializer(Func<YamlDotNet.Serialization.ISerializer> serializerFactory)
    : IYamlTypeConverter
{

    /// <inheritdoc/>
    public virtual bool Accepts(Type type) => type.GetGenericType(typeof(EquatableDictionary<,>))?.GetGenericTypeDefinition() == typeof(EquatableDictionary<,>);

    /// <inheritdoc/>
    public virtual object? ReadYaml(IParser parser, Type type) => throw new NotImplementedException();

    /// <inheritdoc/>
    public virtual void WriteYaml(IEmitter emitter, object? value, Type type)
    {
        if (value == null) return;
        var serializer = serializerFactory();
        emitter.Emit(new MappingStart(AnchorName.Empty, TagName.Empty, isImplicit: true, MappingStyle.Block));
        var dictionaryType = type.GetGenericType(typeof(IDictionary<,>))!;
        var keyType = dictionaryType.GetGenericArguments()[0];
        var valueType = dictionaryType.GetGenericArguments()[1];
        var keyProperty = typeof(KeyValuePair<,>).MakeGenericType(keyType, valueType).GetProperty(nameof(KeyValuePair<object, object>.Key))!;
        var valueProperty = typeof(KeyValuePair<,>).MakeGenericType(keyType, valueType).GetProperty(nameof(KeyValuePair<object, object>.Value))!;
        foreach (var kvp in ((IEnumerable)value))
        {
            var key = keyProperty.GetValue(kvp);
            var keyYaml = serializer.Serialize(key);
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(keyYaml));
            var streamReader = new StreamReader(stream);
            var parser = new Parser(streamReader);
            while (parser.MoveNext())
            {
                if (parser.Current == null || parser.Current is DocumentEnd) break;
                if (parser.Current is StreamStart || parser.Current is DocumentStart) continue;
                emitter.Emit(parser.Current);
            }
            streamReader.Dispose();
            stream.Dispose();

            var kvpValue = valueProperty.GetValue(kvp);
            var valueYaml = serializer.Serialize(kvpValue);
            stream = new MemoryStream(Encoding.UTF8.GetBytes(valueYaml));
            streamReader = new StreamReader(stream);
            parser = new Parser(streamReader);
            while (parser.MoveNext())
            {
                if (parser.Current == null || parser.Current is DocumentEnd) break;
                if (parser.Current is StreamStart || parser.Current is DocumentStart) continue;
                emitter.Emit(parser.Current);
            }
            streamReader.Dispose();
            stream.Dispose();
        }
        emitter.Emit(new MappingEnd());
    }

}
