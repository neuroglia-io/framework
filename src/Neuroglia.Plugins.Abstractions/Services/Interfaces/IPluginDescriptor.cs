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

using System.Reflection;
using System.Runtime.Loader;

namespace Neuroglia.Plugins.Services;

/// <summary>
/// Defines the fundamentals of an object that describes a plugin
/// </summary>
public interface IPluginDescriptor
{

    /// <summary>
    /// Gets the name of the plugin
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the version of the plugin
    /// </summary>
    Version Version { get; }

    /// <summary>
    /// Gets the plugin's type
    /// </summary>
    Type Type { get; }

    /// <summary>
    /// Gets the plugin's assembly
    /// </summary>
    Assembly Assembly { get; }

    /// <summary>
    /// Gets the <see cref="IPluginDescriptor"/>'s <see cref="System.Runtime.Loader.AssemblyLoadContext"/>
    /// </summary>
    AssemblyLoadContext AssemblyLoadContext { get; }

    /// <summary>
    /// Gets the <see cref="IPluginSource"/> the <see cref="IPluginDescriptor"/> is sourced by
    /// </summary>
    IPluginSource Source { get; }

    /// <summary>
    /// Gets a list containing the plugin's tags, if any
    /// </summary>
    IEnumerable<string>? Tags { get; }

}