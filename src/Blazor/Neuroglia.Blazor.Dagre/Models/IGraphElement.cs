
namespace Neuroglia.Blazor.Dagre.Models
{
    public interface IGraphElement
        : IIdentifiable, ILabeled, ICssClass, IMetadata
    {
        Type? ComponentType { get; set; }
        event Action? Changed;
    }
}
