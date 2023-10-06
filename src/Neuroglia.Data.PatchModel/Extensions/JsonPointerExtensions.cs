using Json.Pointer;

namespace Neuroglia;

/// <summary>
/// Defines extensions for <see cref="JsonPointer"/>s
/// </summary>
public static class JsonPointerExtensions
{

    /// <summary>
    /// Converts the <see cref="JsonPointer"/> into camel case
    /// </summary>
    /// <param name="pointer">The <see cref="JsonPointer"/> to convert</param>
    /// <returns>A new <see cref="JsonPointer"/></returns>
    public static JsonPointer ToCamelCase(this JsonPointer pointer) => JsonPointer.Create(pointer.Segments.Select(s => PointerSegment.Parse(s.Value.ToCamelCase())).ToArray());

}