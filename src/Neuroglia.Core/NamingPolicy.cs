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

namespace Neuroglia;

/// <summary>
/// Represents a naming policy
/// </summary>
public class NamingPolicy
{

    /// <summary>
    /// Gets the none naming policy
    /// </summary>
    public static readonly NamingPolicy None = new(input => input);
    /// <summary>
    /// Gets the camel case naming policy
    /// </summary>
    public static readonly NamingPolicy CamelCase = new(input => input.ToCamelCase());
    /// <summary>
    /// Gets the lower case naming policy
    /// </summary>
    public static readonly NamingPolicy LowerCase = new(input => input.ToLowerInvariant());
    /// <summary>
    /// Gets the upper case naming policy
    /// </summary>
    public static readonly NamingPolicy UpperCase = new(input => input.ToUpperInvariant());
    /// <summary>
    /// Gets the kebab case naming policy
    /// </summary>
    public static readonly NamingPolicy KebabCase = new(input => input.ToKebabCase());
    /// <summary>
    /// Gets the snake case naming policy
    /// </summary>
    public static readonly NamingPolicy SnakeCase = new(input => input.ToSnakeCase());

    /// <summary>
    /// Gets the current naming policy. Defauls to none
    /// </summary>
    public static NamingPolicy Current { get; set; } = None;

    /// <summary>
    /// Initializes a new <see cref="NamingPolicy"/>
    /// </summary>
    /// <param name="converter">The <see cref="Func{T, TResult}"/> used to convert names</param>
    public NamingPolicy(Func<string, string> converter)
    {
        this.Converter = converter;
    }

    /// <summary>
    /// Gets the <see cref="Func{T, TResult}"/> used to convert names
    /// </summary>
    protected Func<string, string> Converter { get; }

    /// <summary>
    /// Converts the specified name
    /// </summary>
    /// <param name="name">The name to convert</param>
    /// <returns>The converted name</returns>
    public string ConvertName(string name) => this.Converter.Invoke(name);

}
