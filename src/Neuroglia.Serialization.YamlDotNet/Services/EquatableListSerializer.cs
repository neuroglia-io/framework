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
/// Represents the <see cref="IYamlTypeConverter"/> used to serialize <see cref="EquatableList{T}"/> instances
/// </summary>
public class EquatableListSerializer
    : IYamlTypeConverter
{

    /// <inheritdoc/>
    public virtual bool Accepts(Type type) => type.GetGenericType(typeof(EquatableList<>))?.GetGenericTypeDefinition() == typeof(EquatableList<>);

    /// <inheritdoc/>
    public virtual object? ReadYaml(IParser parser, Type type) => throw new NotImplementedException();

    /// <inheritdoc/>
    public virtual void WriteYaml(IEmitter emitter, object? value, Type type)
    {
        if (value == null || value is not IEnumerable collection) return;
        emitter.Emit(new SequenceStart(null, null, false, SequenceStyle.Block));
        foreach (var item in collection)
        {
            var keyYaml = YamlSerializer.Default.Serialize(item);
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(keyYaml));
            using var streamReader = new StreamReader(stream);
            var parser = new Parser(streamReader);
            while (parser.MoveNext())
            {
                if (parser.Current == null || parser.Current is DocumentEnd) break;
                if (parser.Current is StreamStart || parser.Current is DocumentStart) continue;
                emitter.Emit(parser.Current);
            }
        }
        emitter.Emit(new SequenceEnd());
    }

}