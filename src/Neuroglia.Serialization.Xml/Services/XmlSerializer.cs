﻿// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
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

namespace Neuroglia.Serialization.Xml;

/// <summary>
/// Represents the default implementation of the <see cref="IXmlSerializer"/>
/// </summary>
public class XmlSerializer
    : IXmlSerializer
{

    /// <inheritdoc/>
    public virtual bool Supports(string mediaTypeName) => mediaTypeName switch
    {
        MediaTypeNames.Application.Xml or MediaTypeNames.Text.Xml => true,
        _ => mediaTypeName.EndsWith("+xml")
    };

    /// <inheritdoc/>
    public virtual void Serialize(object? value, Stream stream, Type? type = null)
    {
        if (value == null)
        {
            using var writer = new StreamWriter(stream, leaveOpen: true);
            writer.Write(string.Empty);
            writer.Flush();
        }
        else
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(type ?? value.GetType());
            serializer.Serialize(stream, value);
        }
    }

    /// <inheritdoc/>
    public virtual string SerializeToText(object? value, Type? type = null)
    {
        using var stream = new MemoryStream();
        this.Serialize(value, stream, type);
        stream.Flush();
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    /// <inheritdoc/>
    public virtual object? Deserialize(Stream stream, Type type)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(type);
        var serializer = new System.Xml.Serialization.XmlSerializer(type);
        return serializer.Deserialize(stream);
    }

    /// <inheritdoc/>
    public virtual object? Deserialize(string input, Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        if (string.IsNullOrWhiteSpace(input)) return null;
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));
        return this.Deserialize(stream, type);
    }

}
