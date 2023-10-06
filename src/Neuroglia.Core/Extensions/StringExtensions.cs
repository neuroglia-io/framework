using System.Globalization;
using System.Text;

namespace Neuroglia;

/// <summary>
/// Defines extensions for strings
/// </summary>
public static class StringExtensions
{

    /// <summary>
    /// Converts the string to its camel case representation
    /// </summary>
    /// <param name="input">The input to format</param>
    /// <returns>The formatted string</returns>
    public static string ToCamelCase(this string input)
    {
        if(string.IsNullOrWhiteSpace(input)) return input;
        var firstChar = input[0];
        return char.ToLower(firstChar) + input[1..];
    }

    /// <summary>
    /// Converts the string to its kebab/hyphen case representation
    /// </summary>
    /// <param name="input">The input to format</param>
    /// <returns>The formatted string</returns>
    public static string ToKebabCase(this string input) => string.IsNullOrWhiteSpace(input) ? input : string.Concat(input.Select((c, i) => char.IsUpper(c) && i != 0 ? $"-{c}" : c.ToString())).ToLower();

    /// <summary>
    /// Converts the string to its snake/undersocre case representation
    /// </summary>
    /// <param name="input">The input to format</param>
    /// <returns>The formatted string</returns>
    public static string ToSnakeCase(this string input) => string.IsNullOrWhiteSpace(input) ? input : string.Concat(input.Select((c, i) => char.IsUpper(c) && i != 0 ? $"_{c}" : c.ToString())).ToLower();

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
                if (i != 0 && (!char.IsUpper(text[i - 1]) || (i != text.Length - 1 && !char.IsUpper(text[i + 1])))) result += " ";
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

}
