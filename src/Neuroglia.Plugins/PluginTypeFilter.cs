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

using Neuroglia.Plugins.Services;
using System.Reflection;

namespace Neuroglia.Plugins;

/// <summary>
/// Represents an object used to configure how to filter plugin types
/// </summary>
public class PluginTypeFilter
    : IPluginTypeFilter
{

    /// <summary>
    /// Gets/sets a list containing the filter's filtering criteria
    /// </summary>
    public List<PluginTypeFilterCriterion> Criteria { get; set; } = new();

    /// <summary>
    /// Determines whether or not to filter the specified type
    /// </summary>
    /// <param name="type">The type to check</param>
    /// <param name="metadataLoadContext">The <see cref="MetadataLoadContext"/> to use, if any</param>
    /// <returns>A boolean indicating whether or not to filter the specified type</returns>
    public virtual bool Filters(Type type, MetadataLoadContext? metadataLoadContext = null) => Criteria.All(c => c.IsMetBy(type, metadataLoadContext));

}

