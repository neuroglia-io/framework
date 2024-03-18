using Neuroglia.Data.Infrastructure.ResourceOriented;
using System.Runtime.Serialization;

namespace Neuroglia.UnitTests.Data;

[DataContract]
internal record FakeClusterResource
    : Resource<FakeResourceSpec, FakeResourceStatus>
{

    public static uint AutoIncrementIndex { get; set; }

    internal static readonly ResourceDefinitionInfo ResourceDefinition = new FakeClusterResourceDefinition()!;

    public FakeClusterResource() : base(ResourceDefinition) { }

    public FakeClusterResource(ResourceMetadata metadata, FakeResourceSpec spec, FakeResourceStatus? status = null)
        : base(ResourceDefinition, metadata, spec, status)
    {

    }

    public static FakeClusterResource Create(IDictionary<string, string>? labels = null)
    {
        AutoIncrementIndex++;
        return new FakeClusterResource(new($"fake-resource-{AutoIncrementIndex}", null, labels), new(), new());
    }

}
