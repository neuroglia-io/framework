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

using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Neuroglia;

/// <summary>
/// Defines extensions for strings
/// </summary>
public static partial class StringExtensions
{

    const string SubstitutionBlock = "§§";

    /// <summary>
    /// Determines whether or not the specified input only contains letters
    /// </summary>
    /// <param name="input">The input to check</param>
    /// <returns>A boolean indicating whether or not the specified input only contains letters</returns>
    public static bool IsAlphabetic(this string input) => input.All(char.IsLetter);

    /// <summary>
    /// Determines whether or not the specified input only contains digits
    /// </summary>
    /// <param name="input">The input to check</param>
    /// <returns>A boolean indicating whether or not the specified input only contains digits</returns>
    public static bool IsNumeric(this string input) => input.All(char.IsDigit);

    /// <summary>
    /// Determines whether or not the specified input only contains letters or digits
    /// </summary>
    /// <param name="input">The input to check</param>
    /// <param name="exceptions">An array containing all exceptions allowed</param>
    /// <returns>A boolean indicating whether or not the specified input only contains letters or digits</returns>
    public static bool IsAlphanumeric(this string input, params char[] exceptions) => input.All(c => char.IsLetterOrDigit(c) || (exceptions != null && exceptions.Contains(c)));

    /// <summary>
    /// Determines whether or not the specified input is lowercased
    /// </summary>
    /// <param name="input">The input to check</param>
    /// <returns>A boolean indicating whether or not the specified input is lowercased</returns>
    public static bool IsLowercased(this string input) => input.Where(char.IsLetter).All(char.IsLower);

    /// <summary>
    /// Determines whether or not the specified input is uppercased
    /// </summary>
    /// <param name="input">The input to check</param>
    /// <returns>A boolean indicating whether or not the specified input is uppercased</returns>
    public static bool IsUppercased(this string input) => input.All(char.IsLower);

    /// <summary>
    /// Converts the string to its camel case representation
    /// </summary>
    /// <param name="input">The input to format</param>
    /// <returns>The formatted string</returns>
    public static string ToCamelCase(this string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return input;
        var result = input.RemoveDiacritics();
        result = MatchNonAlphanumericCharactersExpression().Replace(result, SubstitutionBlock).Trim();
        result = ReduceSpacesExpression().Replace(result, " ").Replace(" ", string.Empty);
        result = ReplaceSubstitutionBlockExpression().Replace(result, string.Empty);
        var firstChar = result[0];
        return char.ToLower(firstChar) + result[1..];
    }

    /// <summary>
    /// Converts the string to its kebab/hyphen case representation
    /// </summary>
    /// <param name="input">The input to format</param>
    /// <returns>The formatted string</returns>
    public static string ToKebabCase(this string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return input;
        var delimiter = "-";
        var result = input.RemoveDiacritics();
        result = MatchNonAlphanumericCharactersExpression().Replace(result, SubstitutionBlock).Trim();
        result = ReduceSpacesExpression().Replace(result, " ").Replace(" ", delimiter);
        result = ReplaceSubstitutionBlockExpression().Replace(result, delimiter);
        result = string.Concat(result.Select((c, i) => char.IsUpper(c) && i != 0 ? !char.IsUpper(result[i - 1]) ? $"{delimiter}{c}" : c.ToString() : c.ToString())).ToLower();
        result = ReduceHyphenExpressions().Replace(result, delimiter);
        if (result.Last().ToString() == delimiter) result = result[..^1];
        return result;
    }

    /// <summary>
    /// Converts the string to its snake/undersocre case representation
    /// </summary>
    /// <param name="input">The input to format</param>
    /// <returns>The formatted string</returns>
    public static string ToSnakeCase(this string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return input;
        var delimiter = "_";
        var result = input.RemoveDiacritics();
        result = MatchNonAlphanumericCharactersExpression().Replace(result, SubstitutionBlock).Trim();
        result = ReduceSpacesExpression().Replace(result, " ").Replace(" ", delimiter);
        result = ReplaceSubstitutionBlockExpression().Replace(result, delimiter);
        result = string.Concat(result.Select((c, i) => char.IsUpper(c) && i != 0 ? $"{delimiter}{c}" : c.ToString())).ToLower();
        result = ReduceUnderscoreExpressions().Replace(result, delimiter);
        if (result.Last().ToString() == delimiter) result = result[..^1];
        return result;
    }

    /// <summary>
    /// Replaces the upper case characters by their lowercase counterpart and prepend them with a whitespace character
    /// </summary>
    /// <param name="text">The string to split</param>
    /// <param name="toLowerCase">A boolean indicating whether or not to lowercase the first character of each resulting word</param>
    /// <param name="keepFirstLetterInUpercase">A boolean indicating whether or not to keep the first letter in upper case</param>
    /// <returns>The resulting string</returns>
    public static string SplitCamelCase(this string text, bool toLowerCase = true, bool keepFirstLetterInUpercase = true)
    {
        var result = string.Empty;
        for (var i = 0; i < text.Length; i++)
        {
            var currentChar = text[i];
            if (i == 0 && keepFirstLetterInUpercase)
            {
                result += currentChar;
                continue;
            }
            if (char.IsUpper(currentChar))
            {
                if (i != 0 && (!char.IsUpper(text[i - 1]) || i != text.Length - 1 && !char.IsUpper(text[i + 1]))) result += " ";
                if (toLowerCase && i != text.Length - 1 && !char.IsUpper(text[i + 1])) result += char.ToLower(currentChar);
                else result += currentChar;
            }
            else
            {
                result += currentChar;
            }
        }
        return result;
    }

    /// <summary>
    /// Removes diacritics from the string
    /// </summary>
    /// <param name="text">The string to remove diacritics from</param>
    /// <returns>The resulting string</returns>
    public static string RemoveDiacritics(this string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();
        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark) stringBuilder.Append(c);
        }
        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    [GeneratedRegex("[^a-zA-Z0-9\\s]", RegexOptions.Compiled)]
    private static partial Regex MatchNonAlphanumericCharactersExpression();

    [GeneratedRegex("\\s+", RegexOptions.Compiled)]
    private static partial Regex ReduceSpacesExpression();

    [GeneratedRegex("§§", RegexOptions.Compiled)]
    private static partial Regex ReplaceSubstitutionBlockExpression();

    [GeneratedRegex("\\-+", RegexOptions.Compiled)]
    private static partial Regex ReduceHyphenExpressions();

    [GeneratedRegex("_+", RegexOptions.Compiled)]
    private static partial Regex ReduceUnderscoreExpressions();

}