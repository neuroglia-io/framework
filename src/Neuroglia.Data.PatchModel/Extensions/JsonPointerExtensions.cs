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

using Json.Pointer;

namespace Neuroglia;

/// <summary>
/// Defines extensions for <see cref="JsonPointer"/>s
/// </summary>
public static class JsonPointerExtensions
{

    /// <summary>
    /// Converts the <see cref="JsonPointer"/> into camel case
    /// </summary>
    /// <param name="pointer">The <see cref="JsonPointer"/> to convert</param>
    /// <returns>A new <see cref="JsonPointer"/></returns>
    public static JsonPointer ToCamelCase(this JsonPointer pointer) => JsonPointer.Create(pointer.Segments.Select(s => PointerSegment.Parse(s.Value.ToCamelCase())).ToArray());

    /// <summary>
    /// Determines whether or not the <see cref="JsonPointer"/> is an array indexer
    /// </summary>
    /// <param name="pointer">The <see cref="JsonPointer"/> to check</param>
    /// <returns>A boolean indicating whether or not the <see cref="JsonPointer"/> is an array indexer</returns>
    public static bool IsArrayIndexer(this JsonPointer pointer) => int.TryParse(pointer.Segments.Last().Value, out _);

    /// <summary>
    /// Creates a new <see cref="JsonPointer"/> without indexer
    /// </summary>
    /// <param name="pointer">The <see cref="JsonPointer"/> to remove the indexer of</param>
    /// <returns>A new <see cref="JsonPointer"/></returns>
    public static JsonPointer WithoutArrayIndexer(this JsonPointer pointer) => pointer.IsArrayIndexer() ? JsonPointer.Create(pointer.Segments[..^1]) : pointer;

}