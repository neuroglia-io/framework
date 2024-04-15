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
/// Defines extensions for <see cref="ISerializer"/>s
/// </summary>
public static class ISerializerExtensions
{

    /// <summary>
    /// Serializes the specified value to a byte array
    /// </summary>
    /// <param name="serializer">The extended <see cref="ISerializer"/></param>
    /// <param name="value">The value to serialize</param>
    /// <param name="type">The type to serialize the specified value as. If not specified, type is inferred</param>
    /// <returns>The binary representation of the serialized value</returns>
    public static byte[]? SerializeToByteArray(this ISerializer serializer, object? value, Type? type = null)
    {
        using var stream = new MemoryStream();
        serializer.Serialize(value, stream, type);
        return stream.ToArray();
    }

    /// <summary>
    /// Deserializes the value written on the specified <see cref="Stream"/>
    /// </summary>
    /// <typeparam name="T">The value's expected type</typeparam>
    /// <param name="serializer">The extended <see cref="ISerializer"/></param>
    /// <param name="stream">The <see cref="Stream"/> the value to deserialize has been written to</param>
    /// <returns>The deserialized value, if any</returns>
    public static T? Deserialize<T>(this ISerializer serializer, Stream stream) => (T?)serializer.Deserialize(stream, typeof(T));

    /// <summary>
    /// Deserializes the specified byte array
    /// </summary>
    /// <param name="serializer">The extended <see cref="ISerializer"/></param>
    /// <param name="byteArray">The byte array to deserialize</param>
    /// <param name="type">The type to deserialize the byte array to</param>
    /// <returns>The deserialized value, if any</returns>
    public static object? Deserialize(this ISerializer serializer, byte[]? byteArray, Type type)
    {
        if (byteArray == null || byteArray.Length == 0) return null;
        using var stream = new MemoryStream(byteArray);
        return serializer.Deserialize(stream, type);
    }

    /// <summary>
    /// Deserializes the specified byte array
    /// </summary>
    /// <typeparam name="T">The type to deserialize the byte array to</typeparam>
    /// <param name="serializer">The extended <see cref="ISerializer"/></param>
    /// <param name="byteArray">The byte array to deserialize</param>
    /// <returns>The deserialized value, if any</returns>
    public static T? Deserialize<T>(this ISerializer serializer, byte[]? byteArray)
    {
        if (byteArray == null || byteArray.Length == 0) return default;
        using var stream = new MemoryStream(byteArray);
        return serializer.Deserialize<T>(stream);
    }

    /// <summary>
    /// Deserializes the specified text
    /// </summary>
    /// <typeparam name="T">The type to deserialize the text to</typeparam>
    /// <param name="serializer">The extended <see cref="ISerializer"/></param>
    /// <param name="text">The text to deserialize</param>
    /// <returns>The deserialized value, if any</returns>
    public static T? Deserialize<T>(this ITextSerializer serializer, string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return default;
        return (T?)serializer.Deserialize(text, typeof(T));
    }

    /// <summary>
    /// Deserializes the value written on the specified <see cref="Stream"/>
    /// </summary>
    /// <typeparam name="T">The value's expected type</typeparam>
    /// <param name="serializer">The extended <see cref="ISerializer"/></param>
    /// <param name="stream">The <see cref="Stream"/> the value to deserialize has been written to</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The deserialized value, if any</returns>
    public static async Task<T?> DeserializeAsync<T>(this IAsyncSerializer serializer, Stream stream, CancellationToken cancellationToken = default) => (T?)await serializer.DeserializeAsync(stream, typeof(T), cancellationToken);

    /// <summary>
    /// Converts the specified value into a new instance of the target type
    /// </summary>
    /// <param name="serializer">The extended <see cref="ISerializer"/></param>
    /// <param name="source">The value to convert</param>
    /// <param name="targetType">The type to convert the value to</param>
    /// <returns>The converted value, if any</returns>
    public static object? Convert(this ISerializer serializer, object? source, Type targetType)
    {
        ArgumentNullException.ThrowIfNull(targetType);
        if (source == null) return null;
        if (targetType.IsAssignableFrom(source.GetType())) return source;
        var byteArray = serializer.SerializeToByteArray(source);
        return serializer.Deserialize(byteArray, targetType);
    }

}
