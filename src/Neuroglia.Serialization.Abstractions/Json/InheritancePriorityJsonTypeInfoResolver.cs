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

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Neuroglia.Serialization.Json;

/// <summary>
/// Represents a JsonTypeInfo resolver used to order properties according to their inheritance hierarchy
/// </summary>
public class InheritancePriorityJsonTypeInfoResolver 
    : DefaultJsonTypeInfoResolver
{

    /// <inheritdoc/>
    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        var typeInfo = base.GetTypeInfo(type, options);
        var properties = type.GetProperties().Where(p => !p.TryGetCustomAttribute<JsonIgnoreAttribute>(out _)).ToDictionary(p => p.TryGetCustomAttribute<JsonPropertyNameAttribute>(out var propertyNameAttribute) && propertyNameAttribute != null ? propertyNameAttribute.Name : p.Name, p => p.DeclaringType!.GetAscendencyLevel(type));
        if (typeInfo.Kind == JsonTypeInfoKind.Object)
        {
            foreach (var property in typeInfo.Properties.OrderBy(a => a.Name))
            {
                var match = properties.FirstOrDefault(kvp => kvp.Key.Equals(property.Name, StringComparison.OrdinalIgnoreCase));
                if (match.Key == default) continue;
                var offset = match.Value;
                property.Order -= offset;
            }
        }
        return typeInfo;
    }
}