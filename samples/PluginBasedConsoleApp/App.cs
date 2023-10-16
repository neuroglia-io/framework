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

using Microsoft.Extensions.Hosting;
using Neuroglia.Plugins.Services;
using Shared;

namespace PluginBasedConsoleApp;

public class App
    : BackgroundService
{

    public App(IHostApplicationLifetime applicationLifetime, IPluginProvider pluginProvider) 
    {
        this.ApplicationLifetime = applicationLifetime;
        this.PluginProvider = pluginProvider; 
    }

    protected IHostApplicationLifetime ApplicationLifetime { get; }

    protected IPluginProvider PluginProvider { get; }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if(!this.PluginProvider.GetPlugins().Any())
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Failed to find any plugins. Please register plugin sources and services using the 'appsettings.json' file or using the dependency injection extensions.");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Press any key to exit the application.");
            Console.ReadKey();
            this.ApplicationLifetime.StopApplication();
            return Task.CompletedTask;
        }
        while (!stoppingToken.IsCancellationRequested)
        {
            Console.Write("Input text: ");
            var input = Console.ReadLine();
           
            var plugins = this.PluginProvider.GetPlugins().Where(p => typeof(ITextTransform).IsAssignableFrom(p.Type)).ToList();
            var pluginIndex = 0;
            foreach(var plugin in plugins)
            {
                Console.WriteLine($"- {pluginIndex}: {plugin.Name} {plugin.Version}");
                pluginIndex++;
            }
            while (true)
            {
                Console.Write("Plugin index: ");
                var pluginIndexInput = Console.ReadKey();
                if (!int.TryParse(pluginIndexInput.KeyChar.ToString(), out pluginIndex) || pluginIndex >= plugins.Count)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(" > Invalid");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    continue;
                }
                Console.WriteLine();
                break;
            }
            var selectedPlugin = plugins.ElementAt(pluginIndex);
            var pluginInstance = this.PluginProvider.GetPlugin<ITextTransform>(selectedPlugin.Name, selectedPlugin.Version, selectedPlugin.Source.Name);
            Console.WriteLine($"Transformed input: {pluginInstance.Transform(input)}");
        }
        return Task.CompletedTask;
    }

}
