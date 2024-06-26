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

namespace Neuroglia.Blazor.Dagre.Models;

/// <summary>
/// Used to serialize/deserialize a <see cref="IGraphLib"/> to/from JSON
/// </summary>
public interface IGraphLibJsonConverter
{
    /// <summary>
    /// Serializes a <see cref="IGraphLib"/> to JSON (aka GraphLib json.write(g))
    /// </summary>
    /// <param name="graph"></param>
    /// <returns>The serialized <see cref="IGraphLib"/></returns>
    Task<string> SerializeAsync(IGraphLib graph);
    /// <summary>
    /// Deserializes a JSON to a 
    /// </summary>
    /// <param name="json"></param> <see cref="IGraphLib"/> (aka GraphLib json.read(json))
    /// <returns>The deserialized <see cref="IGraphLib"/></returns>
    Task<IGraphLib> DeserializeAsync(string json);
}
