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

using Neuroglia.Serialization.Json.Converters;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Neuroglia.Data.Infrastructure;

/// <summary>
/// Enumerates all default types of text document-related watch event
/// </summary>
[TypeConverter(typeof(EnumMemberTypeConverter))]
[JsonConverter(typeof(StringEnumConverter))]
public enum TextDocumentWatchEventType
{
    /// <summary>
    /// Indicates an event that describes the creation of a new text document
    /// </summary>
    [EnumMember(Value = "created")]
    Created = 0,
    /// <summary>
    /// Indicates an event that describes appending new content to a text document
    /// </summary>
    [EnumMember(Value = "appended")]
    Appended = 1,
    /// <summary>
    /// Indicates an event that describes the replacement of a document's content
    /// </summary>
    [EnumMember(Value = "updated")]
    Replaced = 2,
    /// <summary>
    /// Indicates an event that describes the deletion of a text document
    /// </summary>
    [EnumMember(Value = "deleted")]
    Deleted = 4
}