// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License"),
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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
