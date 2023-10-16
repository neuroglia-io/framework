using Neuroglia;
using Shared;

namespace SnakeCaseTextTransformPlugin;

public class SnakeCaseTextTransform
    : ITextTransform
{

    public string? Transform(string? input) => input?.ToCamelCase();

}
