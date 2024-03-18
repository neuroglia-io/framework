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
/// Defines extensions for <see cref="ICollection"/>s
/// </summary>
public static class ICollectionExtensions
{

    /// <summary>
    /// Converts the <see cref="ICollection"/> into a new <see cref="ICollection{TObject}"/> of the specified type
    /// </summary>
    /// <typeparam name="TObject">The type to convert the collection items to</typeparam>
    /// <param name="collection">The <see cref="ICollection"/> to convert</param>
    /// <returns>A new <see cref="ICollection{TObject}"/></returns>
    public static ICollection<TObject> OfType<TObject>(this ICollection collection)
        where TObject : class, IObject, new()
    {
        if(collection is ICollection<TObject> generic) return generic;
        return new Collection<TObject>(collection.Metadata, collection.Items?.Select(i => i.ConvertTo<TObject>()!));
    }

}
