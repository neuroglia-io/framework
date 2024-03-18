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

namespace Neuroglia.Data.Infrastructure.ResourceOriented;

/// <summary>
/// Defines extensions for <see cref="IResourceWatchEvent"/>s
/// </summary>
public static class IResourceWatchEventExtensions
{

    /// <summary>
    /// Converts the <see cref="ResourceWatchEvent"/> into a new <see cref="ResourceWatchEvent{TResource}"/>
    /// </summary>
    /// <typeparam name="TResource">The type of watched <see cref="Resource"/>s</typeparam>
    /// <param name="e">The <see cref="ResourceWatchEvent"/> to convert</param>
    /// <returns>A new <see cref="ResourceWatchEvent{TResource}"/></returns>
    public static IResourceWatchEvent<TResource> OfType<TResource>(this IResourceWatchEvent e)
        where TResource : class, IResource, new()
    {
        if (e is IResourceWatchEvent<TResource> generic) return generic;
        return new ResourceWatchEvent<TResource>(e.Type, e.Resource.ConvertTo<TResource>()!);
    }

}
