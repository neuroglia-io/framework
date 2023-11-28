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

namespace Neuroglia.JsonSchema.Generation;

/// <summary>
/// Exposes methods to handle <see cref="SchemaGeneratorConfiguration"/>s
/// </summary>
public static class JsonSchemaGeneratorConfiguration
{

    /// <summary>
    /// Gets/sets an <see cref="Action{T}"/> used to configure the <see cref="SchemaGeneratorConfiguration"/> used by default
    /// </summary>
    public static Action<SchemaGeneratorConfiguration> DefaultOptionsConfiguration { get; set; } = (options) =>
    {
        options.PropertyNameResolver = PropertyNameResolvers.CamelCase;
        options.Optimize = false;
        options.Generators.Add(new DateTimeOffsetSchemaGenerator());
    };

    static SchemaGeneratorConfiguration? _default;
    /// <summary>
    /// Gets the default <see cref="SchemaGeneratorConfiguration"/>. For most use cases, please use dependency injection instead.
    /// </summary>
    public static SchemaGeneratorConfiguration Default
    {
        get
        {
            if(_default == null)
            {
                var value = new SchemaGeneratorConfiguration();
                DefaultOptionsConfiguration(value);
                return value;
            }
            return _default;
        }
        set
        {
            _default = value;
        }
    }

}