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

using Neuroglia.Serialization.Json;

namespace Neuroglia.Serialization;

/// <summary>
/// Defines extensions for <see cref="IYamlSerializer"/>s
/// </summary>
public static class IYamlSerializerExtensions
{

    /// <summary>
    /// Converts the specified JSON input into YAML
    /// </summary>
    /// <param name="yamlSerializer">The extended <see cref="IYamlSerializer"/></param>
    /// <param name="json">The JSON input to convert</param>
    /// <returns>The JSON input converted into YAML</returns>
    public static string ConvertFromJson(this IYamlSerializer yamlSerializer, string json)
    {
        if (string.IsNullOrWhiteSpace(json)) return null!;
        var graph = JsonSerializer.Default.Deserialize<object>(json);
        return yamlSerializer.SerializeToText(graph);
    }

    /// <summary>
    /// Converts the specified YAML input into JSON
    /// </summary>
    /// <param name="yamlSerializer">The extended <see cref="IYamlSerializer"/></param>
    /// <param name="yaml">The YAML input to convert</param>
    /// <returns>The YAML input converted into JSON</returns>
    public static string ConvertToJson(this IYamlSerializer yamlSerializer, string yaml)
    {
        if (string.IsNullOrWhiteSpace(yaml)) return null!;
        var graph = yamlSerializer.Deserialize<object>(yaml);
        return JsonSerializer.Default.SerializeToText(graph);
    }

}