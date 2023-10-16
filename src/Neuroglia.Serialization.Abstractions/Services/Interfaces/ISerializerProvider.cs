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

namespace Neuroglia.Serialization;

/// <summary>
/// Defines the fundamentals of a service used to manage serializers
/// </summary>
public interface ISerializerProvider
{

    /// <summary>
    /// Gets all registered <see cref="ISerializer"/>s
    /// </summary>
    /// <returns>A new <see cref="IEnumerable{T}"/> containing all registered <see cref="ISerializer"/>s</returns>
    IEnumerable<ISerializer> GetSerializers();

    /// <summary>
    /// Gets all registered <see cref="ISerializer"/> that support the specified media type name
    /// </summary>
    /// <param name="mediaTypeName">The media type name supported by <see cref="ISerializer"/> to get</param>
    /// <returns>A new <see cref="IEnumerable{T}"/> containing all registered <see cref="ISerializer"/> that support the specified media type name</returns>
    IEnumerable<ISerializer> GetSerializersFor(string mediaTypeName);

}
