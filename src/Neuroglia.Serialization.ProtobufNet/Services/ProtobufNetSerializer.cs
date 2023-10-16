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

using ProtoBuf;

namespace Neuroglia.Serialization.Protobuf.Services;

/// <summary>
/// Represents the <see href="ProtobufNet">https://github.com/protobuf-net/protobuf-net</see> implementation of the <see cref="ISerializer"/>
/// </summary>
public class ProtobufNetSerializer
    : ISerializer
{

    /// <inheritdoc/>
    public virtual bool Supports(string mediaTypeName) => mediaTypeName switch
    {
        "application/protobuf" or "application/x-protobuf" or "application/vnd.google.protobuf" => true,
        _ => false
    };

    /// <inheritdoc/>
    public virtual void Serialize(object? value, Stream stream, Type? type = null) => Serializer.Serialize(stream, value);

    /// <inheritdoc/>
    public virtual object? Deserialize(Stream stream, Type type) => Serializer.Deserialize(stream, type);

}