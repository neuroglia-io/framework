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
/// Defines the fundamentals of a service used to serialize and deserialize data asynchronously
/// </summary>
public interface IAsyncSerializer
{

    /// <summary>
    /// Serializes an object to the specified <see cref="Stream"/>
    /// </summary>
    /// <param name="stream">The <see cref="Stream"/> to serialize the object to</param>
    /// <param name="graph">The object to serialize</param>
    /// <param name="type">The type of the object to serialize</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    Task SerializeAsync(Stream stream, object graph, Type type, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deserializes the value written on the specified <see cref="Stream"/>
    /// </summary>
    /// <param name="stream">The <see cref="Stream"/> the value to deserialize has been written to</param>
    /// <param name="type">The value's expected type</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The deserialized value, if any</returns>
    Task<object?> DeserializeAsync(Stream stream, Type type, CancellationToken cancellationToken = default);

}
