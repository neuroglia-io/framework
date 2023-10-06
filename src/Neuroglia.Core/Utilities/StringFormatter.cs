using System.Text.RegularExpressions;

namespace Neuroglia;

/// <summary>
/// Defines methods to help format strings
/// </summary>
public static partial class StringFormatter
{

    /// <summary>
    /// Formats the specified template
    /// </summary>
    /// <param name="text">The template to format</param>
    /// <param name="args">The arguments to format the string with</param>
    /// <remarks>Accepts named arguments, which will be replaced in sequence by the specified values</remarks>
    /// <returns>The formatted template</returns>
    public static string Format(this string text, params object[] args)
    {
        if (string.IsNullOrWhiteSpace(text)) return text;
        var formattedText = text;
        var matches = MatchCurlyBracedWords.Matches(text)
            .Select(m => m.Value)
            .Distinct()
            .ToList();
        for (int i = 0; i < matches.Count && i < args.Length; i++)
        {
            formattedText = formattedText.Replace(matches[i], args[i].ToString());
        }
        return formattedText;
    }

    /// <summary>
    /// Formats the specified template using named parameters
    /// </summary>
    /// <param name="template">The template to format</param>
    /// <param name="parameters">A name/value mapping of the parameters to format</param>
    /// <returns>The formatted template</returns>
    public static string NamedFormat(string template, IDictionary<string, object>? parameters)
    {
        if (string.IsNullOrWhiteSpace(template) || parameters?.Any() != true) return template;
        var output = template;
        foreach(var parameter in parameters)
        {
            output = output.Replace($"{{{parameter.Key}}}", parameter.Value.ToString(), StringComparison.OrdinalIgnoreCase);
        }
        return output;
    }

    /// <summary>
    /// Formats the specified template using named parameters
    /// </summary>
    /// <param name="template">The template to format</param>
    /// <param name="parameterObject">An object that defines the top-level parameters to use</param>
    /// <returns>The formatted template</returns>
    public static string NamedFormat(string template, object? parameterObject)
    {
        if (string.IsNullOrWhiteSpace(template) || parameterObject == null) return template;
        var parameters = parameterObject.GetType()
            .GetProperties()
            .Where(p => p.CanRead)
            .ToDictionary(p => p.Name, p => p.GetValue(parameterObject)!);
        return NamedFormat(template, parameters);
    }

    static readonly Regex MatchCurlyBracedWords = MatchCurlyBracedWordsRegex();

    [GeneratedRegex("\\{([^}]+)\\}", RegexOptions.Compiled)]
    private static partial Regex MatchCurlyBracedWordsRegex();

}
