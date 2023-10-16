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

using System.Globalization;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Neuroglia.Serialization.Yaml;

/// <summary>
/// Represents the <see cref="IYamlTypeConverter"/> used to serialize <see cref="DateTimeOffset"/>s
/// </summary>
public class DateTimeOffsetSerializer
    : IYamlTypeConverter
{

    /// <inheritdoc/>
    public virtual bool Accepts(Type type) => typeof(DateTimeOffset).IsAssignableFrom(type);

    /// <inheritdoc/>
    public virtual object ReadYaml(IParser parser, Type type)
    {
        Scalar scalar = (Scalar)parser.Current!;
        parser.MoveNext();
        return DateTimeOffset.Parse(scalar.Value, CultureInfo.InvariantCulture);
    }

    /// <inheritdoc/>
    public virtual void WriteYaml(IEmitter emitter, object? value, Type type)
    {
        if (value == null || value is not DateTimeOffset dateTime) return;
        emitter.Emit(new Scalar(dateTime.ToString("o", CultureInfo.InvariantCulture)));
    }

}