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

using System.Text.RegularExpressions;

namespace Neuroglia.Data.Infrastructure.ResourceOriented;

/// <summary>
/// Defines extensions for <see cref="RuleWithOperation"/>s
/// </summary>
public static class RuleWithOperationExtensions
{

    /// <summary>
    /// Determines whether or not the <see cref="RuleWithOperation"/> matches the specified resource
    /// </summary>
    /// <param name="rule">The <see cref="RuleWithOperation"/> to evaluate</param>
    /// <param name="operation">The operation to perform</param>
    /// <param name="group">The API group the resource to operate on belongs to</param>
    /// <param name="version">The version of the API the resource to operate on belongs to</param>
    /// <param name="plural">The plural name of the type of the resource to operate on</param>
    /// <param name="namespace">The namespace the resource to operate on belongs to</param>
    /// <returns>A boolean indicating whether or not the <see cref="RuleWithOperation"/> matches the specified resource</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static bool Matches(this RuleWithOperation rule, Operation operation, string group, string version, string plural, string? @namespace = null)
    {
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (!string.IsNullOrWhiteSpace(rule.Scope) && !string.IsNullOrWhiteSpace(@namespace) && !Regex.IsMatch(@namespace, rule.Scope, RegexOptions.Compiled)) return false;
        if (rule.Operations != null && !rule.Operations.Any(o => o == EnumHelper.Stringify(operation))) return false;
        if (rule.ApiGroups != null && !rule.ApiGroups.Any(g => Regex.IsMatch(group, g, RegexOptions.Compiled))) return false;
        if (rule.ApiVersions != null && !rule.ApiVersions.Any(v => Regex.IsMatch(version, v, RegexOptions.Compiled))) return false;
        if (rule.Kinds != null && !rule.Kinds.Any(k => Regex.IsMatch(plural, k, RegexOptions.Compiled))) return false;
        return true;
    }

}