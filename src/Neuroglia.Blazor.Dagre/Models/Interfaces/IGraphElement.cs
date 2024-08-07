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

namespace Neuroglia.Blazor.Dagre.Models;

/// <summary>
/// Defines the fundamentals of a graph element
/// </summary>
public interface IGraphElement
    : IIdentifiable, ILabeled, ICssStylable, IMetadata
{

    /// <summary>
    /// Gets the type of the component used to represents the graph element
    /// </summary>
    /// 
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    Type? ComponentType { get; set; }
    /// <summary>
    /// The event fired whenever the graph element changes
    /// </summary>
    event EventHandler? Changed;
}
