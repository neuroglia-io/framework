/*
 * Copyright © 2021 Neuroglia SPRL. All rights reserved.
 * <p>
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * <p>
 * http://www.apache.org/licenses/LICENSE-2.0
 * <p>
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */
using System;
using System.Linq;
using System.Reflection;

namespace Neuroglia
{
    /// <summary>
    /// Defines extension methods for <see cref="MemberInfo"/>s
    /// </summary>
    public static class MemberInfoExtensions
    {

        /// <summary>
        /// Attempts to get a custom attribute of the specified <see cref="MemberInfo"/>
        /// </summary>
        /// <typeparam name="TAttribute">The type of the custom attribute to get</typeparam>
        /// <param name="extended">The extended <see cref="MemberInfo"/></param>
        /// <param name="attribute">The resulting custom attribute</param>
        /// <returns>A boolean indicating whether or not the custom attribute of the specified <see cref="MemberInfo"/> could be found</returns>
        public static bool TryGetCustomAttribute<TAttribute>(this MemberInfo extended, out TAttribute attribute)
            where TAttribute : Attribute
        {
            attribute = extended.GetCustomAttribute<TAttribute>();
            return attribute != null;
        }

        /// <summary>
        /// Gets the <see cref="MemberInfo"/>'s XML documentation
        /// </summary>
        /// <param name="extended">The extended <see cref="MemberInfo"/></param>
        /// <returns>The <see cref="MemberInfo"/>'s XML documentation</returns>
        public static string GetDocumentation(this MemberInfo extended)
        {
            if (extended == null)
                throw new ArgumentNullException(nameof(extended));
            return XmlDocumentationHelper.DocumentationOf(extended);
        }

        /// <summary>
        /// Gets the <see cref="MemberInfo"/>'s XML documentation summary
        /// </summary>
        /// <param name="extended">The extended <see cref="MemberInfo"/></param>
        /// <returns>The <see cref="MemberInfo"/>'s XML documentation summary</returns>
        public static string GetDocumentationSummary(this MemberInfo extended)
        {
            if (extended == null)
                throw new ArgumentNullException(nameof(extended));
            return XmlDocumentationHelper.SummaryOf(extended);
        }

        /// <summary>
        /// Gets the <see cref="MemberInfo"/> overriden by the <see cref="MemberInfo"/>
        /// </summary>
        /// <param name="extended">The <see cref="MemberInfo"/> to get the overriden <see cref="MemberInfo"/> for</param>
        /// <returns>The <see cref="MemberInfo"/> overriden by the <see cref="MemberInfo"/>, if any</returns>
        public static MemberInfo GetOverridenMember(this MemberInfo extended)
        {
            if (extended == null)
                throw new ArgumentNullException(nameof(extended));
            Type declaringType = extended.DeclaringType;
            if (declaringType == extended.ReflectedType)
                declaringType = declaringType.GetDeclaringTypeOf(extended);
            switch (extended)
            {
                case ConstructorInfo constructor:
                    return extended; //nothing to do here: constructors cannot be overriden
                case FieldInfo field:
                    return declaringType.GetField(field.Name);
                case PropertyInfo property:
                    return declaringType.GetProperty(property.Name);
                case MethodInfo method:
                    return declaringType.GetMethod(method.Name, method.GetParameters().Select(p => p.ParameterType).ToArray());
                default:
                    throw new NotSupportedException($"The specified member type '{extended.MemberType}' is not supported");
            }
        }

    }

}