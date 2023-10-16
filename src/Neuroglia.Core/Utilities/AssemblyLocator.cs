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

using System.Reflection;

namespace Neuroglia;

/// <summary>
/// Acts as a helper class for locating <see cref="Assembly"/> instances
/// </summary>
public static class AssemblyLocator
{

    static readonly object Lock = new();

    static readonly List<Assembly> LoadedAssemblies = new();

    static AssemblyLocator()
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            LoadedAssemblies.Add(assembly);
        }
        AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoad;
    }

    /// <summary>
    /// Get all loaded assemlies
    /// </summary>
    /// <returns>An <see cref="IEnumerable{T}"/> of all loaded assemblies</returns>
    public static IEnumerable<Assembly> GetAssemblies() => LoadedAssemblies.AsEnumerable();

    static void OnAssemblyLoad(object? sender, AssemblyLoadEventArgs e)
    {
        lock (Lock)
        {
            LoadedAssemblies.Add(e.LoadedAssembly);
        }
    }

}