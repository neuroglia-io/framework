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

using Json.Schema;
using Neuroglia.Data.Infrastructure.ResourceOriented;
using System.Runtime.Serialization;

namespace Neuroglia.UnitTests.Data;

[DataContract]
internal record FakeNamespacedResourceDefinition
    : ResourceDefinition
{

    internal new const string ResourceGroup = "unit.tests.hylo.io";
    internal new const string ResourceVersion = "v1";
    internal const string ResourceSingular = "fake";
    internal new const string ResourcePlural = "fakes";
    internal new const string ResourceKind = "Fake";

    public FakeNamespacedResourceDefinition()
        : base(new ResourceDefinitionSpec(ResourceScope.Namespaced, ResourceGroup, new(ResourceSingular, ResourcePlural, ResourceKind), new ResourceDefinitionVersion(ResourceVersion, new(GetSchema())) { Served = true, Storage = true, SubResources = new Dictionary<string, object>() { { "status", new() } } }))
    {

    }

    static JsonSchema GetSchema()
    {
        return JsonSchema.FromText($$"""
{
  "type": "object",
  "properties": {
    "spec":{
      "type": "object",
      "properties":{
        "fakeProperty1":{
          "type": "string"
        },
        "fakeProperty2":{
          "type": "number"
        }
      }
    },
    "status":{
      "type": "object",
      "properties":{
        "fakeProperty1":{
          "type": "string"
        },
        "fakeProperty2":{
          "type": "number"
        }
      }
    }
  }
}
""");
    }

}
