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

namespace Neuroglia.Serialization;

/// <summary>
/// Defines the fundamentals of a service used to serialize and deserialize data
/// </summary>
public interface ISerializer
{

    /// <summary>
    /// Serializes the specified value
    /// </summary>
    /// <param name="value">The value to serialize</param>
    /// <param name="stream">The <see cref="Stream"/> to serialize the value to</param>
    /// <param name="type">The type to serialize the specified value as. If not specified, type is inferred</param>
    void Serialize(object? value, Stream stream, Type? type = null);

    /// <summary>
    /// Deserializes the value written on the specified <see cref="Stream"/>
    /// </summary>
    /// <param name="stream">The <see cref="Stream"/> the value to deserialize has been written to</param>
    /// <param name="type">The value's expected type</param>
    /// <returns>The deserialized value, if any</returns>
    object? Deserialize(Stream stream, Type type);

    /// <summary>
    /// Determines whether or not the <see cref="ISerializer"/> supports the specified media type name
    /// </summary>
    /// <param name="mediaTypeName">The media type name to check</param>
    /// <returns>A boolean indicating whether or not the <see cref="ISerializer"/> supports the specified media type name</returns>
    bool Supports(string mediaTypeName);

}
