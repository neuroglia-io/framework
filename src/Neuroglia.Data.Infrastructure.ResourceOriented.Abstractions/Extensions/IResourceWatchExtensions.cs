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
/// Defines extensions for <see cref="IResourceWatch"/>es
/// </summary>
public static class IResourceWatchExtensions
{

    /// <summary>
    /// Converts the <see cref="IResourceWatch"/> into a new <see cref="IResourceWatch{TResource}"/> of the specified type
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="IResource"/> to watch</typeparam>
    /// <param name="watch">The <see cref="IResourceWatch"/> to convert</param>
    /// <returns>A new <see cref="IResourceWatch{TResource}"/> of the specified type</returns>
    public static IResourceWatch<TResource> OfType<TResource>(this IResourceWatch watch)
        where TResource : class, IResource, new()
    {
        if (watch is IResourceWatch<TResource> generic) return generic;
        var dispose = watch is not ResourceWatch knownWatch || knownWatch.DisposeObservable;
        return new ResourceWatch<TResource>(watch, dispose);
    }

}
