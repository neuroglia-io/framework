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
/// Defines extension methods for <see cref="ParameterInfo"/>s
/// </summary>
public static class ParameterInfoExtensions
{

    /// <summary>
    /// Attempts to get a custom attribute of the specified <see cref="ParameterInfo"/>
    /// </summary>
    /// <typeparam name="TAttribute">The type of the custom attribute to get</typeparam>
    /// <param name="extended">The extended <see cref="ParameterInfo"/></param>
    /// <param name="attribute">The resulting custom attribute</param>
    /// <returns>A boolean indicating whether or not the custom attribute of the specified <see cref="ParameterInfo"/> could be found</returns>
    public static bool TryGetCustomAttribute<TAttribute>(this ParameterInfo extended, out TAttribute? attribute)
        where TAttribute : Attribute
    {
        attribute = extended.GetCustomAttribute<TAttribute>();
        return attribute != null;
    }

}
