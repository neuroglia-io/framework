using System.Xml;

namespace Neuroglia;

/// <summary>
/// Represents an helper class for handling ISO 8601 durations
/// </summary>
public static partial class Iso8601Duration
{

    /// <summary>
    /// Parses the specified input into a new <see cref="TimeSpan"/>
    /// </summary>
    /// <param name="input">The input string to parse</param>
    /// <returns>The parsed <see cref="TimeSpan"/></returns>
    public static TimeSpan Parse(string input) => XmlConvert.ToTimeSpan(input);

    /// <summary>
    /// Formats the specified <see cref="TimeSpan"/>
    /// </summary>
    /// <param name="timeSpan">The <see cref="TimeSpan"/> to format</param>
    /// <returns>The parsed <see cref="TimeSpan"/></returns>
    public static string Format(TimeSpan timeSpan) => XmlConvert.ToString(timeSpan);

}