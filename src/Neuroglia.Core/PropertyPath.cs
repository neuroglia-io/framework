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

using System.Linq.Expressions;
using System.Reflection;

namespace Neuroglia;

/// <summary>
/// Represents a parsed property path, which typically is a chain of property names separated with a dot
/// </summary>
public class PropertyPath
{

    /// <summary>
    /// Initializes a new <see cref="PropertyPath"/>
    /// </summary>
    /// <param name="path">The string that contains the raw property path</param>
    public PropertyPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));
        if (!IsValidPropertyPath(path)) throw new ArgumentException("The specified value '" + path + "' is not a valid " + nameof(PropertyPath) + " string", nameof(path));
        this.OriginalPath = path;
        this.Values = [.. this.OriginalPath.Split('.', StringSplitOptions.RemoveEmptyEntries)];
    }

    /// <summary>
    /// Gets a string that represents the <see cref="PropertyPath"/>'s original path
    /// </summary>
    public string OriginalPath { get; private set; }

    /// <summary>
    /// Gets a list of the values the <see cref="PropertyPath"/> is made of
    /// </summary>
    public List<string> Values { get; private set; }

    /// <summary>
    /// Gets a <see cref="MemberExpression"/> that expresses the member-access to the specified property
    /// </summary>
    /// <param name="target">The <see cref="Expression"/> that represents the instance on which to attempt the member-access</param>
    /// <returns>A <see cref="MemberExpression"/> that expresses the member-access to the specified property</returns>
    public MemberExpression ToExpression(Expression target)
    {
        if (this.Values.Count < 1) throw new InvalidOperationException("The " + nameof(PropertyPath) + " is empty");
        var propertyName = this.Values[0];
        var property = target.Type.GetProperty(propertyName, BindingFlags.Default | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase) ?? throw new MissingMemberException("Failed to retrieve a property with name '" + propertyName + "' in type '" + target.Type.Name + "'");
        var memberExpression = Expression.MakeMemberAccess(target, property);
        target = memberExpression;
        for (var index = 1; index < this.Values.Count; index++)
        {
            propertyName = this.Values[index];
            property = target.Type.GetProperty(propertyName, BindingFlags.Default | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (property == null) throw new MissingMemberException("Failed to retrieve a property with name '" + propertyName + "' in type '" + target.Type.Name + "'");
            memberExpression = Expression.MakeMemberAccess(target, property);
            target = memberExpression;
        }
        return memberExpression;
    }

    /// <summary>
    /// Parse the specified input into a new <see cref="PropertyPath"/>
    /// </summary>
    /// <param name="input">The input to parse</param>
    /// <returns>A new <see cref="PropertyPath"/> based on the specified input</returns>
    public static PropertyPath Parse(string input) => new(input);

    /// <summary>
    /// Attempts to parse the specified input into a new <see cref="PropertyPath"/>
    /// </summary>
    /// <param name="input">The input to parse</param>
    /// <param name="propertyPath">The resulting <see cref="PropertyPath"/>, if any</param>
    /// <returns>A boolean indicating whether or not a <see cref="PropertyPath"/> could be parse from the specified input</returns>
    public static bool TryParse(string input, out PropertyPath? propertyPath)
    {
        try
        {
            propertyPath = Parse(input);
            return true;
        }
        catch
        {
            propertyPath = null;
            return false;
        }
    }

    /// <summary>
    /// Gets a boolean indicating whether or not the specified input is a valid <see cref="PropertyPath"/>
    /// </summary>
    /// <param name="propertyPath">The property path to evaluate</param>
    /// <returns>A boolean indicating whether or not the specified input is a valid <see cref="PropertyPath"/></returns>
    public static bool IsValidPropertyPath(string propertyPath)
    {
        foreach (var c in propertyPath)
        {
            if (!char.IsLetter(c)
                && !char.IsNumber(c)
                && c != '.')
                return false;
        }
        return true;
    }

}
