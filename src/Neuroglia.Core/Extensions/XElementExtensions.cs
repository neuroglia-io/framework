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

using System.Xml.Linq;

namespace Neuroglia;

/// <summary>
/// Defines extensions for <see cref="XElement"/>s
/// </summary>
public static class XElementExtensions
{

    /// <summary>
    /// Determines whether or not the <see cref="XElement"/> contains the specified <see cref="XAttribute"/>
    /// </summary>
    /// <param name="element">The extended <see cref="XElement"/></param>
    /// <param name="name">The name of the <see cref="XAttribute"/> to get</param>
    /// <param name="attribute">The <see cref="XAttribute"/> with the specified name, if any</param>
    /// <returns>A boolean indicating whether or not the <see cref="XElement"/> contains the specified <see cref="XAttribute"/></returns>
    public static bool TryGetAttribute(this XElement element, XName name, out XAttribute? attribute)
    {
        attribute = element.Attribute(name);
        return attribute != null;
    }

}