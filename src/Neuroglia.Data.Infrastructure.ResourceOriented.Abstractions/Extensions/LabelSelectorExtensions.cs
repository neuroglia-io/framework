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

namespace Neuroglia.Data.Infrastructure.ResourceOriented;

/// <summary>
/// Defines extensions for <see cref="LabelSelector"/>s
/// </summary>
public static class LabelSelectorExtensions
{

    /// <summary>
    /// Determines whether or not the <see cref="LabelSelector"/> selects the specified <see cref="IResource"/>
    /// </summary>
    /// <param name="selector">The extended <see cref="LabelSelector"/></param>
    /// <param name="resource">The <see cref="IResource"/> to check if selected by the <see cref="LabelSelector"/></param>
    /// <returns>A boolean indicating whether or not the <see cref="LabelSelector"/> selects the specified <see cref="IResource"/></returns>
    public static bool Selects(this LabelSelector selector, IResource resource)
    {
        if (selector == null) throw new ArgumentNullException(nameof(selector));
        if (resource == null) throw new ArgumentNullException(nameof(resource));
        if (resource.Metadata.Labels == null) return false;
        if (!resource.Metadata.Labels.TryGetValue(selector.Key, out var labelValue) || string.IsNullOrWhiteSpace(labelValue)) return false;
        return selector.Operator switch
        {
            LabelSelectionOperator.Contains => selector.Values?.Contains(labelValue) == true,
            LabelSelectionOperator.Equals => selector.Value == labelValue,
            LabelSelectionOperator.NotContains => selector.Values?.Contains(labelValue) == false,
            LabelSelectionOperator.NotEquals => selector.Value != labelValue,
            _ => throw new NotSupportedException($"The specified {nameof(LabelSelectionOperator)} '{selector.Operator}' is not supported"),
        };
    }

    /// <summary>
    /// Converts the <see cref="LabelSelector"/>s into a new expression
    /// </summary>
    /// <param name="labelSelectors">The <see cref="LabelSelector"/>s to convert</param>
    /// <returns>A new selection expression based on the <see cref="LabelSelector"/>s</returns>
    public static string? ToExpression(this IEnumerable<LabelSelector>? labelSelectors) => labelSelectors?.Select(l => l.ToString()).Join(',');

}