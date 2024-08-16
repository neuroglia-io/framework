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

namespace Neuroglia.Data.Infrastructure;

/// <summary>
/// Defines the fundamentals of a text document
/// </summary>
public interface ITextDocument
{

    /// <summary>
    /// Gets the key used to uniquely identify the text document
    /// </summary>
    object Key { get; }

    /// <summary>
    /// Gets the date and time at which the document was created at
    /// </summary>
    DateTimeOffset CreatedAt { get; }

    /// <summary>
    /// Gets the date and time at which the document has last been modified
    /// </summary>
    DateTimeOffset LastModified { get; set; }

    /// <summary>
    /// Gets the document's content length
    /// </summary>
    long Length { get; set; }

}

/// <summary>
/// Defines the fundamentals of a text document
/// </summary>
/// <typeparam name="TKey">The type of key used to uniquely identify the text document</typeparam>
public interface ITextDocument<TKey>
    : ITextDocument
{

    /// <summary>
    /// Gets the key used to uniquely identify the text document
    /// </summary>
    new TKey Key { get; }

}
