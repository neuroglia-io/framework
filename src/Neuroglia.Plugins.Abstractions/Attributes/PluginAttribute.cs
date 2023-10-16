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

namespace Neuroglia.Plugins;

/// <summary>
/// Represents the attribute used to configure plugin-specific behaviors
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class PluginAttribute
    : Attribute
{

    /// <summary>
    /// Gets/sets the plugin's name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets/sets the plugin's version
    /// </summary>
    public Version? Version { get; set; }

    /// <summary>
    /// Gets/sets an array containing the tags, if any, associated to the marked plugin type
    /// </summary>
    public string[]? Tags { get; set; }

}