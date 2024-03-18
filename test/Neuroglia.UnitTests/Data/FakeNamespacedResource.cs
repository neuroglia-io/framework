using Neuroglia.Data.Infrastructure.ResourceOriented;
using System.Runtime.Serialization;

namespace Neuroglia.UnitTests.Data;

[DataContract]
internal record FakeNamespacedResource
    : Resource<FakeResourceSpec, FakeResourceStatus>
{

    public static uint AutoIncrementIndex { get; set; }

    internal static readonly ResourceDefinitionInfo ResourceDefinition = new FakeNamespacedResourceDefinition()!;

    public FakeNamespacedResource() : base(ResourceDefinition) { }

    public FakeNamespacedResource(ResourceMetadata metadata, FakeResourceSpec spec, FakeResourceStatus? status = null) 
        : base(ResourceDefinition, metadata, spec, status)
    {

    }

    public static FakeNamespacedResource Create(string @namespace, IDictionary<string, string>? labels = null)
    {
        AutoIncrementIndex++;
        return new FakeNamespacedResource(new($"fake-resource-{AutoIncrementIndex}", @namespace, labels), new(), new());
    }

}
