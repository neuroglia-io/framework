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

namespace Neuroglia.Data.Infrastructure;

/// <summary>
/// Defines the fundamentals of a text document watch event
/// </summary>
public interface ITextDocumentWatchEvent
{

    /// <summary>
    /// Gets the key of the <see cref="ITextDocument"/> that has produced the <see cref="ITextDocumentWatchEvent"/>
    /// </summary>
    object Key { get; }

    /// <summary>
    /// Gets the watch event type
    /// </summary>
    TextDocumentWatchEventType Type { get; }

    /// <summary>
    /// Gets the document's content. In case <see cref="Type"/> has been set to <see cref="TextDocumentWatchEventType.Appended"/>, contains the appended text only
    /// </summary>
    string? Content { get; }

}
