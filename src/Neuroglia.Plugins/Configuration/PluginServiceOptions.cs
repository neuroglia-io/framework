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

using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace Neuroglia.Plugins.Configuration;

/// <summary>
/// Represents an object used to configure a plugin service
/// </summary>
public class PluginServiceOptions
{

    /// <summary>
    /// Gets/sets the assembly qualified name of the type of plugin service to register
    /// </summary>
    [Required, MinLength(3)]
    public virtual string Type { get; set; } = null!;

    /// <summary>
    /// Gets/sets the name of the plugin source, if any, to get the plugin implementation from
    /// </summary>
    public virtual string? Source { get; set; } = null;

    /// <summary>
    /// Gets/sets the lifetime of the plugin service
    /// </summary>
    public virtual ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Singleton;

}