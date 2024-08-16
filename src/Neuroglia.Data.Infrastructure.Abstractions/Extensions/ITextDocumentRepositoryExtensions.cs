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

using Neuroglia.Data.Infrastructure.Services;

namespace Neuroglia.Data.Infrastructure;

/// <summary>
/// Defines extensions for <see cref="ITextDocumentRepository"/> instances
/// </summary>
public static class ITextDocumentRepositoryExtensions
{

    /// <summary>
    /// Determines whether or not the document with the specified key exists
    /// </summary>
    /// <param name="repository">The extended <see cref="ITextDocumentRepository"/></param>
    /// <param name="key">The key of the document to check</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A boolean indicating whether or not the document with the specified key exists</returns>
    public static async Task<bool> ExistsAsync(this ITextDocumentRepository repository, object key, CancellationToken cancellationToken = default) => await repository.GetAsync(key, cancellationToken).ConfigureAwait(false) == null;

    /// <summary>
    /// Determines whether or not the document with the specified key exists
    /// </summary>
    /// <param name="repository">The extended <see cref="ITextDocumentRepository"/></param>
    /// <param name="key">The key of the document to check</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A boolean indicating whether or not the document with the specified key exists</returns>
    public static async Task<bool> ExistsAsync<TKey>(this ITextDocumentRepository<TKey> repository, TKey key, CancellationToken cancellationToken = default) where TKey : IEquatable<TKey> => await repository.GetAsync(key, cancellationToken).ConfigureAwait(false) == null;

}