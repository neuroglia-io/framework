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

using System.Runtime.Serialization;

namespace Neuroglia.Data.Infrastructure;

/// <summary>
/// Represents the default implementation of the <see cref="ITextDocumentWatchEvent"/> type
/// </summary>
[DataContract]
public record TextDocumentWatchEvent
    : ITextDocumentWatchEvent
{

    /// <summary>
    /// Initializes a new <see cref="TextDocumentWatchEvent"/>
    /// </summary>
    public TextDocumentWatchEvent() { }

    /// <summary>
    /// Initializes a new <see cref="TextDocumentWatchEvent"/>
    /// </summary>
    /// <param name="type">The watch event type</param>
    /// <param name="content">The document's content</param>
    public TextDocumentWatchEvent(TextDocumentWatchEventType type, string? content = null)
    {
        this.Type = type;
        this.Content = content;
    }

    /// <inheritdoc/>
    public virtual TextDocumentWatchEventType Type { get; set; }

    /// <inheritdoc/>
    public virtual string? Content { get; set; }
}