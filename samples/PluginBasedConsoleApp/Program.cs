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

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Neuroglia.Plugins;
using PluginBasedConsoleApp;

var host = new HostBuilder();

host.ConfigureAppConfiguration(configuration => configuration.AddJsonFile("appsettings.json", true));
host.ConfigureServices((context, services) =>
{
    services.AddLogging();
    services.AddPluginProvider(context.Configuration);
    services.AddHostedService<App>();
});

using var app = host.Build();
await app.RunAsync();