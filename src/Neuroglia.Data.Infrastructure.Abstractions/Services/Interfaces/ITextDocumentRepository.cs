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

namespace Neuroglia.Data.Infrastructure.Services;

/// <summary>
/// Defines the fundamentals of a service used to manage text documents
/// </summary>
public interface ITextDocumentRepository
{

    /// <summary>
    /// Gets the specified <see cref="ITextDocument"/>
    /// </summary>
    /// <param name="key">The key of the <see cref="ITextDocument"/> to get</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The <see cref="ITextDocument"/> with the specified key, if any</returns>
    Task<ITextDocument?> GetAsync(object key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all <see cref="ITextDocument"/>s
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IAsyncEnumerable{T}"/> used to enumerate all existing <see cref="ITextDocument"/>s</returns>
    IAsyncEnumerable<ITextDocument> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all <see cref="ITextDocument"/>s
    /// </summary>
    /// <param name="max">The maximum amount of documents to list, if any</param>
    /// <param name="skip">The amount of documents to skip, if any</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="ICollection{T}"/> containing all <see cref="ITextDocument"/>s</returns>
    Task<ICollection<ITextDocument>> ListAsync(int? max = null, int? skip = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reads the specified document's content
    /// </summary>
    /// <param name="key">The key of the document to read</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The specified document's content</returns>
    Task<string> ReadToEndAsync(object key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reads the specified document's content line by line
    /// </summary>
    /// <param name="key">The key of the document to read</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IAsyncEnumerable{T}"/> used to enumerate the specified document's lines</returns>
    IAsyncEnumerable<string> ReadAsync(object key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Watches changes made to the specified document
    /// </summary>
    /// <param name="key">The key of the document to watch</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="ITextDocumentWatch"/>, used to watch the changes made to a specified text document</returns>
    Task<ITextDocumentWatch> WatchAsync(object key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Appends text to the specified document
    /// </summary>
    /// <param name="key">The key of the document to append text to</param>
    /// <param name="text">The text to append to the specified document</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    Task AppendAsync(object key, string text, CancellationToken cancellationToken = default);

    /// <summary>
    /// Replaces the content of the specified document
    /// </summary>
    /// <param name="key">The key of the document to replace</param>
    /// <param name="text">The text to replace the specified document's content with</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    Task ReplaceAsync(object key, string text, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the specified document
    /// </summary>
    /// <param name="key">The key of the document to delete</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    Task DeleteAsync(object key, CancellationToken cancellationToken = default);

}

/// <summary>
/// Defines the fundamentals of a service used to manage text documents
/// </summary>
/// <typeparam name="TKey">The type of key used to uniquely identify a document</typeparam>
public interface ITextDocumentRepository<TKey>
    : ITextDocumentRepository
    where TKey : IEquatable<TKey>
{

    /// <summary>
    /// Gets the specified <see cref="ITextDocument"/>
    /// </summary>
    /// <param name="key">The key of the <see cref="ITextDocument"/> to get</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The <see cref="ITextDocument"/> with the specified key, if any</returns>
    Task<ITextDocument<TKey>?> GetAsync(TKey key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all <see cref="ITextDocument"/>s
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IAsyncEnumerable{T}"/> used to enumerate all existing <see cref="ITextDocument"/>s</returns>
    new IAsyncEnumerable<ITextDocument<TKey>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all <see cref="ITextDocument"/>s
    /// </summary>
    /// <param name="max">The maximum amount of documents to list, if any</param>
    /// <param name="skip">The amount of documents to skip, if any</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="ICollection{T}"/> containing all <see cref="ITextDocument"/>s</returns>
    new Task<ICollection<ITextDocument<TKey>>> ListAsync(int? max = null, int? skip = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reads the specified document's content
    /// </summary>
    /// <param name="key">The key of the document to read</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The specified document's content</returns>
    Task<string> ReadToEndAsync(TKey key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reads the specified document's content line by line
    /// </summary>
    /// <param name="key">The key of the document to read</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IAsyncEnumerable{T}"/> used to enumerate the specified document's lines</returns>
    IAsyncEnumerable<string> ReadAsync(TKey key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Watches changes made to the specified document
    /// </summary>
    /// <param name="key">The key of the document to watch</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="ITextDocumentWatch"/>, used to watch the changes made to a specified text document</returns>
    Task<ITextDocumentWatch> WatchAsync(TKey key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Appends text to the specified document
    /// </summary>
    /// <param name="key">The key of the document to append text to</param>
    /// <param name="text">The text to append to the specified document</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    Task AppendAsync(TKey key, string text, CancellationToken cancellationToken = default);

    /// <summary>
    /// Replaces the content of the specified document
    /// </summary>
    /// <param name="key">The key of the document to replace</param>
    /// <param name="text">The text to replace the specified document's content with</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    Task ReplaceAsync(TKey key, string text, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the specified document
    /// </summary>
    /// <param name="key">The key of the document to delete</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    Task DeleteAsync(TKey key, CancellationToken cancellationToken = default);

}
