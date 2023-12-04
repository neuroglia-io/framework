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

using System.Collections.Concurrent;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace Neuroglia;

/// <summary>
/// Defines helper members to handle Xml code documentation
/// </summary>
/// <remarks>Code adapted from https://docs.microsoft.com/en-us/archive/msdn-magazine/2019/october/csharp-accessing-xml-documentation-via-reflection</remarks>
public static partial class XmlDocumentationHelper
{

    private static ConcurrentDictionary<string, AssemblyXmlDocumentationContainer> LoadedAssemblyXmlDocumentation = [];

    /// <summary>
    /// Gets the specified type's XML code documentation
    /// </summary>
    /// <param name="type">The type to get the XML code documentation for</param>
    /// <returns>The XML code documentation of the specified type</returns>
    public static string? DocumentationOf(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        if (!LoadedAssemblyXmlDocumentation.TryGetValue(type.Assembly.FullName!, out var container))
        {
            container = AssemblyXmlDocumentationContainer.BuildFor(type.Assembly);
            LoadedAssemblyXmlDocumentation[type.Assembly.FullName!] = container;
        }

        return container.GetDocumentationFor(type);
    }

    /// <summary>
    /// Gets the specified type's XML code documentation
    /// </summary>
    /// <param name="type">The type to get the XML code documentation for</param>
    /// <returns>The XML code documentation of the specified type</returns>
    public static XElement? DocumentationXElementOf(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        var xml = DocumentationOf(type);

        if (string.IsNullOrWhiteSpace(xml)) return null;
        else return XElement.Parse(xml);
    }

    /// <summary>
    /// Gets the specified type's XML code documentation summary
    /// </summary>
    /// <param name="type">The type to get the XML code documentation summary for</param>
    /// <returns>The XML code documentation summary of the specified type</returns>
    public static string? SummaryOf(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        var docElement = DocumentationXElementOf(type);
        if (docElement == null) return null;

        return ReadDocumentationFrom(docElement);
    }

    /// <summary>
    /// Gets the specified member's XML code documentation
    /// </summary>
    /// <param name="member">The member to get the XML code documentation for</param>
    /// <returns>The XML code documentation of the specified member</returns>
    public static string DocumentationOf(MemberInfo member)
    {
        ArgumentNullException.ThrowIfNull(member);
        if (!LoadedAssemblyXmlDocumentation.TryGetValue(member.DeclaringType!.Assembly.FullName!, out var container))
        {
            container = AssemblyXmlDocumentationContainer.BuildFor(member.DeclaringType.Assembly);
            LoadedAssemblyXmlDocumentation[member.DeclaringType.Assembly.FullName!] = container;
        }
        return container.GetDocumentationFor((dynamic)member);
    }

    /// <summary>
    /// Gets the specified member's XML code documentation
    /// </summary>
    /// <param name="member">The member to get the XML code documentation for</param>
    /// <returns>The XML code documentation of the specified member</returns>
    public static XElement? DocumentationXElementOf(MemberInfo member)
    {
        ArgumentNullException.ThrowIfNull(member);
        var xml = DocumentationOf(member);
        if (string.IsNullOrWhiteSpace(xml)) return null;
        return XElement.Parse(xml);
    }

    /// <summary>
    /// Gets the specified member's XML code documentation summary
    /// </summary>
    /// <param name="member">The member to get the XML code documentation summary for</param>
    /// <returns>The XML code documentation summary of the specified member</returns>
    public static string? SummaryOf(MemberInfo member)
    {
        ArgumentNullException.ThrowIfNull(member);

        var docElement = DocumentationXElementOf(member);
        if (docElement == null) return null;

        if (member.DeclaringType != member.ReflectedType || docElement.Element("inheritdoc") != null) return SummaryOf(member.GetOverridenMember());

        return ReadDocumentationFrom(docElement);
    }

    /// <summary>
    /// Gets the specified parameter's XML code documentation
    /// </summary>
    /// <param name="parameter">The parameter to get the XML code documentation for</param>
    /// <returns>The XML code documentation of the specified parameter</returns>
    public static string DocumentationOf(ParameterInfo parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter);

        if (!LoadedAssemblyXmlDocumentation.TryGetValue(parameter.Member.DeclaringType!.Assembly.FullName!, out var container))
        {
            container = AssemblyXmlDocumentationContainer.BuildFor(parameter.Member.DeclaringType.Assembly);
            LoadedAssemblyXmlDocumentation[parameter.Member.DeclaringType.Assembly.FullName!] = container;
        }
        return container.GetDocumentationFor((dynamic)parameter);
    }

    /// <summary>
    /// Gets the specified parameter's XML code documentation summary
    /// </summary>
    /// <param name="parameter">The parameter to get the XML code documentation summary for</param>
    /// <returns>The XML code documentation summary of the specified parameter</returns>
    public static string? SummaryOf(ParameterInfo parameter) => parameter == null ? throw new ArgumentNullException(nameof(parameter)) : ReadDocumentationFrom(DocumentationOf(parameter));

    static string? ReadDocumentationFrom(XElement? docElement, string? rootTag = "summary")
    {
        if(docElement == null) return null;

        var summary = string.IsNullOrWhiteSpace(rootTag) ? docElement : docElement.Element(rootTag);
        if (summary == null) return null;

        using var reader = summary?.CreateReader();
        reader?.MoveToContent();
        var documentation = reader?.ReadInnerXml().Trim();

        foreach(var tag in summary!.Elements())
        {
            var toReplace = tag.ToString();
            var replaceWith = string.Empty;
            if (!tag.IsEmpty)
            {
                using var tagReader = tag.CreateReader();
                tagReader.MoveToContent();
                replaceWith = tagReader?.ReadInnerXml().Trim();
            }
            if (tag.TryGetAttribute("cref", out var attribute) && attribute != null && string.IsNullOrWhiteSpace(replaceWith)) replaceWith = attribute.Value.Split('.', StringSplitOptions.RemoveEmptyEntries).Last().ToCamelCase();
            else if (tag.TryGetAttribute("href", out attribute) && attribute != null) replaceWith = string.IsNullOrWhiteSpace(replaceWith) ? attribute.Value : $"{replaceWith} ({attribute.Value})";
            documentation = documentation?.Replace(toReplace, replaceWith);
        }

        return documentation;
    }

    static string? ReadDocumentationFrom(string? documentationXml) => string.IsNullOrWhiteSpace(documentationXml) ? null : ReadDocumentationFrom(XElement.Parse(documentationXml), null);

    partial class AssemblyXmlDocumentationContainer
    {

        AssemblyXmlDocumentationContainer() { }

        public Dictionary<string, string> XmlDocumentation { get; } = [];

        public string? GetDocumentationFor(Type type)
        {
            ArgumentNullException.ThrowIfNull(type);

            var key = "T:" + this.BuildXmlKey(type.FullName!, null);
            this.XmlDocumentation.TryGetValue(key, out var documentation);

            return documentation?.Trim();
        }

        public string? GetDocumentationFor(MethodBase methodInfo)
        {
            ArgumentNullException.ThrowIfNull(methodInfo);

            var key = "M:" + this.BuildXmlKeyFor(methodInfo);
            this.XmlDocumentation.TryGetValue(key, out var documentation);

            return documentation?.Trim();
        }

        public string? GetDocumentationFor(FieldInfo fieldInfo)
        {
            ArgumentNullException.ThrowIfNull(fieldInfo);

            var key = "F:" + this.BuildXmlKey(fieldInfo.DeclaringType!.FullName!, fieldInfo.Name);
            this.XmlDocumentation.TryGetValue(key, out var documentation);
            return documentation?.Trim();
        }

        public string? GetDocumentationFor(EventInfo eventInfo)
        {
            ArgumentNullException.ThrowIfNull(eventInfo);

            var key = "M:" + this.BuildXmlKey(eventInfo.DeclaringType!.FullName!, eventInfo.Name);
            this.XmlDocumentation.TryGetValue(key, out var documentation);

            return documentation?.Trim();
        }

        public string? GetDocumentationFor(PropertyInfo propertyInfo)
        {
            ArgumentNullException.ThrowIfNull(propertyInfo);

            var key = "P:" + this.BuildXmlKey(propertyInfo.DeclaringType!.FullName!, propertyInfo.Name);
            this.XmlDocumentation.TryGetValue(key, out var documentation);

            return documentation?.Trim();
        }

        public string? GetDocumentationFor(ParameterInfo parameterInfo)
        {
            ArgumentNullException.ThrowIfNull(parameterInfo);

            var documentationXml = this.GetDocumentationFor((dynamic)parameterInfo.Member);
            if (string.IsNullOrWhiteSpace(documentationXml)) return null;

            XElement documentationNode = XElement.Parse(documentationXml);
            if (documentationNode.IsEmpty) return null;

            foreach (var childNode in documentationNode.Elements())
            {
                if(childNode.Name == "param" && childNode.TryGetAttribute("name", out var name) && name?.Value == parameterInfo.Name) return childNode.ToString();
            }
            return null;
        }

        private string BuildXmlKey(string fullTypeName, string? memberName = null)
        {
            if (string.IsNullOrWhiteSpace(fullTypeName)) throw new ArgumentNullException(nameof(fullTypeName));

            var key = ReplaceKeyRegex().Replace(fullTypeName, string.Empty).Replace('+', '.');
            if (memberName != null) key += "." + memberName;

            return key;
        }

        private string BuildXmlKeyFor(MethodBase methodBase)
        {
            var key = this.BuildXmlKey(methodBase.DeclaringType!.FullName!, methodBase.Name);

            if (methodBase.IsGenericMethod) key += $"`{methodBase.GetGenericArguments().Length}";
            if (methodBase is ConstructorInfo) key = key.Replace("..ctor", ".#ctor");

            key += $"({string.Join(",", methodBase.GetParameters().Select(p => this.BuildXmlKey(p.ParameterType.FullName!)))})";
            return key;
        }

        public static AssemblyXmlDocumentationContainer BuildFor(Assembly assembly)
        {
            ArgumentNullException.ThrowIfNull(assembly);

            var container = new AssemblyXmlDocumentationContainer();
            var file = $"{new Uri(assembly.Location).LocalPath[..^3]}xml";
            if (!File.Exists(file)) return container;

            var xmlDocumentation = File.ReadAllText(file);
            using var xmlReader = XmlReader.Create(new StringReader(xmlDocumentation));
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "member")
                {
                    var xmlName = xmlReader["name"]!;
                    container.XmlDocumentation[xmlName] = xmlReader.ReadOuterXml();
                }
            }
            return container;
        }

        [GeneratedRegex(@"\[.*\]")]
        private static partial Regex ReplaceKeyRegex();
    }

}
