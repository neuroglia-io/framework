namespace Neuroglia.Data.Expressions;

/// <summary>
/// Defines extensions for strings
/// </summary>
public static class StringExtensions
{

    /// <summary>
    /// Determines whether or not the string is a runtime expression
    /// </summary>
    /// <param name="text">The string to check</param>
    /// <returns>A boolean indicating whether or not the string is a runtime expression</returns>
    public static bool IsRuntimeExpression(this string text) => !string.IsNullOrEmpty(text) && text.TrimStart().StartsWith("${") && text.TrimEnd().EndsWith("}");

}
