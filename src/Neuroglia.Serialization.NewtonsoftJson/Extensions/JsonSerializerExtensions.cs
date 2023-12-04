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

using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace Neuroglia.Serialization;

/// <summary>
/// Defines extensions for <see cref="JsonSerializer"/>s
/// </summary>
public static partial class JsonSerializerExtensions
{

    /// <summary>
    ///  Wraps the JSON into an <see cref="IAsyncEnumerable{TValue}" /> that can be used to deserialize root-level JSON arrays in a streaming manner.
    /// </summary>
    /// <typeparam name="T">The type of items to deserialize</typeparam>
    /// <param name="serializer">The extended <see cref="JsonSerializer"/></param>
    /// <param name="stream">The <see cref="Stream"/> to deserialize</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IAsyncEnumerable{T}"/></returns>
    public static async IAsyncEnumerable<T> DeserializeAsyncEnumerable<T>(this JsonSerializer serializer, Stream stream, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var loadSettings = new JsonLoadSettings { LineInfoHandling = LineInfoHandling.Ignore };
        using var textReader = new StreamReader(stream, leaveOpen: true);
        using var reader = new JsonTextReader(textReader) { CloseInput = false };
        await foreach (var token in LoadAsyncEnumerable(reader, loadSettings, cancellationToken).ConfigureAwait(false)) yield return token.ToObject<T>(serializer)!;
    }

    static async IAsyncEnumerable<JToken> LoadAsyncEnumerable(JsonTextReader reader, JsonLoadSettings? settings = default, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        (await reader.MoveToContentAndAssertAsync().ConfigureAwait(false)).AssertTokenType(JsonToken.StartArray);
        cancellationToken.ThrowIfCancellationRequested();
        while ((await reader.ReadToContentAndAssert(cancellationToken).ConfigureAwait(false)).TokenType != JsonToken.EndArray)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return await JToken.LoadAsync(reader, settings, cancellationToken).ConfigureAwait(false);
        }
        cancellationToken.ThrowIfCancellationRequested();
    }

    static JsonReader AssertTokenType(this JsonReader reader, JsonToken tokenType) => reader.TokenType == tokenType ? reader : throw new JsonSerializationException(string.Format("Unexpected token {0}, expected {1}", reader.TokenType, tokenType));

    static async Task<JsonReader> ReadToContentAndAssert(this JsonReader reader, CancellationToken cancellationToken = default) => await (await reader.ReadAndAssertAsync(cancellationToken).ConfigureAwait(false)).MoveToContentAndAssertAsync(cancellationToken).ConfigureAwait(false);

    static async Task<JsonReader> MoveToContentAndAssertAsync(this JsonReader reader, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(reader);

        if (reader.TokenType == JsonToken.None) await reader.ReadAndAssertAsync(cancellationToken).ConfigureAwait(false);
        while (reader.TokenType == JsonToken.Comment) await reader.ReadAndAssertAsync(cancellationToken).ConfigureAwait(false);

        return reader;
    }

    static async Task<JsonReader> ReadAndAssertAsync(this JsonReader reader, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(reader);

        if (!await reader.ReadAsync(cancellationToken).ConfigureAwait(false)) throw new JsonReaderException("Unexpected end of JSON stream.");

        return reader;
    }

}