using Neuroglia;
using Shared;

namespace CamelCaseTextTransformPlugin;

public class CamelCaseTextTransform
    : ITextTransform
{

    public string? Transform(string? input) => input?.ToCamelCase();

}
