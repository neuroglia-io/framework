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

using System.Text.RegularExpressions;

namespace Neuroglia.Data.Infrastructure.ResourceOriented;

/// <summary>
/// Represents a label-based resource selector
/// </summary>
[DataContract]
public partial record LabelSelector
{

    /// <summary>
    /// Initializes a new <see cref="LabelSelector"/>
    /// </summary>
    public LabelSelector() { }

    /// <summary>
    /// Initializes a new <see cref="LabelSelector"/>
    /// </summary>
    /// <param name="key">The key of the label to select resources by</param>
    /// <param name="operator">The selection operator</param>
    /// <param name="values">The expected values, if any</param>
    public LabelSelector(string key, LabelSelectionOperator @operator, params string[] values)
    {
        Key = key;
        Operator = @operator;
        if (values == null) return;
        if (values.Length == 1 && @operator == LabelSelectionOperator.Equals || @operator == LabelSelectionOperator.NotEquals) Value = values[0];
        else Values = values.WithValueSemantics();
    }

    /// <summary>
    /// Gets/sets the key of the label to select resources by
    /// </summary>
    [Required]
    [DataMember(Order = 1, Name = "key", IsRequired = true), JsonPropertyOrder(1), JsonPropertyName("key"), YamlMember(Order = 1, Alias = "key")]
    public string Key { get; set; } = null!;

    /// <summary>
    /// Gets/sets the selection operator
    /// </summary>
    [Required]
    [DataMember(Order = 2, Name = "operator", IsRequired = true), JsonPropertyOrder(2), JsonPropertyName("operator"), YamlMember(Order = 2, Alias = "operator")]
    public virtual LabelSelectionOperator Operator { get; set; }

    /// <summary>
    /// Gets/sets the expected value, if any
    /// </summary>
    [DataMember(Order = 3, Name = "value", IsRequired = true), JsonPropertyOrder(3), JsonPropertyName("value"), YamlMember(Order = 3, Alias = "value")]
    public virtual string? Value { get; set; }

    /// <summary>
    /// Gets/sets a list containing expected values, if any
    /// </summary>
    [DataMember(Order = 4, Name = "values", IsRequired = true), JsonPropertyOrder(4), JsonPropertyName("values"), YamlMember(Order = 4, Alias = "values")]
    public virtual EquatableList<string>? Values { get; set; }

    /// <inheritdoc/>
    public override string ToString()
    {
        return Operator switch
        {
            LabelSelectionOperator.Contains => string.IsNullOrWhiteSpace(Value) && Values?.Any() == false ? Key : $"{Key} in ({Values!.Join(',')})",
            LabelSelectionOperator.NotContains => string.IsNullOrWhiteSpace(Value) && Values?.Any() == false ? $"!{Key}" : $"{Key} notin ({Values!.Join(',')})",
            LabelSelectionOperator.Equals => $"{Key}={Value}",
            LabelSelectionOperator.NotEquals => $"{Key}!={Value}",
            _ => throw new NotSupportedException($"The specified {nameof(LabelSelectionOperator)} '{Operator}' is not supported"),
        };
    }

    /// <summary>
    /// Parses the specified input
    /// </summary>
    /// <param name="input">The input to parse</param>
    /// <returns>A new <see cref="LabelSelector"/></returns>
    public static LabelSelector Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) throw new ArgumentNullException(nameof(input));
        if (input.StartsWith('!')) return new(input[1..], LabelSelectionOperator.NotContains);
        string key;
        var components = input.Split('=', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        LabelSelectionOperator selectionOperator;
        if (components.Length == 2)
        {
            key = components[0];
            selectionOperator = LabelSelectionOperator.Equals;
            if (key.EndsWith('!'))
            {
                key = key[..^1];
                selectionOperator = LabelSelectionOperator.NotEquals;
            }
            return new(key, selectionOperator, components[1]);
        }
        components = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (components.Length == 1) return new(input, LabelSelectionOperator.Contains);
        selectionOperator = components[1] switch
        {
            "in" => LabelSelectionOperator.Contains,
            "notin" => LabelSelectionOperator.NotContains,
            _ => throw new NotSupportedException($"The specified selection operator '{components[2]}' is not supported")
        };
        key = components[0];
        var operatorIndex = input.IndexOf(components[1], key.Length + 1) + components[1].Length + 1;
        components = input[operatorIndex..].Trim().TrimStart('(').TrimEnd(')').Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return new LabelSelector(key, selectionOperator, components);
    }

    /// <summary>
    /// Parses the specified input
    /// </summary>
    /// <param name="input">The input that contains the selectors, separated by a comma</param>
    /// <returns>A new <see cref="IEnumerable{T}"/> containing the parsed <see cref="LabelSelector"/>s</returns>
    public static IEnumerable<LabelSelector> ParseList(string input)
    {
        if(string.IsNullOrWhiteSpace(input)) return Enumerable.Empty<LabelSelector>();
        var labelSelectors = new List<LabelSelector>();
        if (!string.IsNullOrWhiteSpace(input)) labelSelectors.AddRange(SplitNonEnclosedCsvRegex().Split(input).Select(Parse));
        return labelSelectors;
    }

    /// <summary>
    /// Implicitly converts the specified input into a new <see cref="LabelSelector"/>
    /// </summary>
    /// <param name="input">The input to convert</param>
    public static implicit operator LabelSelector?(string? input) => string.IsNullOrWhiteSpace(input) ? null : Parse(input);

    [GeneratedRegex(",\\s*(?![^()]*\\))")]
    private static partial Regex SplitNonEnclosedCsvRegex();
}
