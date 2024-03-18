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
/// Represents the default implementation of the <see cref="IObjectNamingConvention"/> interface
/// </summary>
public partial class ObjectNamingConvention
    : IObjectNamingConvention
{

    readonly int _groupMaxLength = 63;
    readonly int _nameMaxLength = 63;
    readonly int _annotationMaxLength = 63;
    readonly int _labelMaxLength = 63;
    readonly int _maxVersionLength = 22;

    readonly Regex LabelNameRegex = GetLabelNameRegex();

    /// <summary>
    /// Gets/sets the current <see cref="IObjectNamingConvention"/> for Hylo object names
    /// </summary>
    public static IObjectNamingConvention Current { get; set; } = new ObjectNamingConvention();

    /// <inheritdoc/>
    public virtual bool IsValidResourceGroup(string group)
    {
        if (string.IsNullOrWhiteSpace(group)) return true;
        return group.Length <= _groupMaxLength
            && group.IsLowercased()
            && group.IsAlphanumeric('.', '-')
            && char.IsLetterOrDigit(group.First())
            && char.IsLetterOrDigit(group.Last());
    }

    /// <inheritdoc/>
    public virtual bool IsValidResourceName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        return name.Length <= _nameMaxLength
            && name.IsLowercased()
            && name.IsAlphanumeric('-')
            && char.IsLetterOrDigit(name.First())
            && char.IsLetterOrDigit(name.Last());
    }

    /// <inheritdoc/>
    public virtual bool IsValidResourcePluralName(string plural)
    {
        if (string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        return plural.Length <= _nameMaxLength
            && plural.IsLowercased()
            && plural.IsAlphanumeric('-')
            && char.IsLetterOrDigit(plural.First())
            && char.IsLetterOrDigit(plural.Last());
    }

    /// <inheritdoc/>
    public virtual bool IsValidResourceKind(string kind)
    {
        if (string.IsNullOrWhiteSpace(kind)) throw new ArgumentNullException(nameof(kind));
        return kind.Length <= _nameMaxLength
            && kind.IsAlphabetic()
            && char.IsUpper(kind.First());
    }
    /// <inheritdoc/>
    public virtual bool IsValidAnnotationName(string annotation)
    {
        if (string.IsNullOrWhiteSpace(annotation)) throw new ArgumentNullException(nameof(annotation));
        return annotation.Length <= _annotationMaxLength
            && annotation.IsLowercased()
            && annotation.IsAlphanumeric('-')
            && char.IsAsciiLetterOrDigit(annotation.First())
            && char.IsLetterOrDigit(annotation.Last());
    }

    /// <inheritdoc/>
    public virtual bool IsValidLabelName(string label)
    {
        if (string.IsNullOrWhiteSpace(label)) throw new ArgumentNullException(nameof(label));
        return label.Length > 3 && label.Length <= _labelMaxLength && LabelNameRegex.IsMatch(label);
    }

    /// <inheritdoc/>
    public virtual bool IsValidVersion(string version)
    {
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        return version.Length <= _maxVersionLength
            && version.IsAlphanumeric()
            && version.StartsWith('v');
    }

    [GeneratedRegex("^[a-z0-9]([-a-z0-9]*[a-z0-9])([\\/a-z0-9]([-a-z0-9]*[a-z0-9]))$", RegexOptions.Compiled)]
    private static partial Regex GetLabelNameRegex();

}