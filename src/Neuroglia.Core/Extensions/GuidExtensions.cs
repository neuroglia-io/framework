namespace Neuroglia;

/// <summary>
/// Defines extensions for <see cref="Guid"/>s
/// </summary>
public static class GuidExtensions
{

    /// <summary>
    /// Formats the <see cref="Guid"/> into its short string representation
    /// </summary>
    /// <param name="id">The <see cref="Guid"/> to format</param>
    /// <returns>The specified <see cref="Guid"/>'s short string representation</returns>
    public static string ToShortString(this Guid id) => id.ToString("N")[..15];

}