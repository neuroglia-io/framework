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
