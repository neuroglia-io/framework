using Neuroglia;
using Shared;

namespace KebabCaseTextTransformPlugin;

public class KebabCaseTextTransform
    : ITextTransform
{

    public string? Transform(string? input) => input?.ToKebabCase();

}
