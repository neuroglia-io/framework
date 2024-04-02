﻿// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Neuroglia.Plugins;
using Neuroglia.Plugins.Services;

namespace Neuroglia.UnitTests.Cases.Plugins.Sources;

public class NugetPackagePluginSourceTests
    : PluginSourceTestsBase
{

    public NugetPackagePluginSourceTests() : base(BuildServices()) { }

    [Fact(Skip = "Does not work on GitHub CI/CD which seems to duplicate assembly load context for some reason")]
    public override Task Load_Should_Work() => base.Load_Should_Work();

    static IServiceCollection BuildServices()
    {
        var services = new ServiceCollection();
        services.AddPluginSource(new NugetPackagePluginSource(new NullLoggerFactory(), "source1", new() { Filter = (PluginTypeFilter)new PluginTypeFilterBuilder().Implements<ILogger>().Build() }, "Microsoft.Extensions.Logging", "6.0.0"));
        return services;
    }

}
