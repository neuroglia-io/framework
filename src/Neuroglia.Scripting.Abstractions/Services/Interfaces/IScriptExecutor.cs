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

using System.Diagnostics;

namespace Neuroglia.Scripting.Services;

/// <summary>
/// Defines the fundamentals of a service used to execute scripts
/// </summary>
public interface IScriptExecutor
{

    /// <summary>
    /// Determines whether or not the <see cref="IScriptExecutor"/> supports the specified expression language
    /// </summary>
    /// <param name="language">The expression language to check</param>
    /// <returns>A boolean indicating whether or not the <see cref="IScriptExecutor"/> supports the specified expression language</returns>
    bool Supports(string language);

    /// <summary>
    /// Executes the specified script
    /// </summary>
    /// <param name="script">The script to run</param>
    /// <param name="arguments">A list of the arguments to run the script with, if any</param>
    /// <param name="environment">A key/value mapping of the environment variables to use</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="Process"/></returns>
    Task<Process> ExecuteAsync(string script, IEnumerable<string>? arguments = null, IDictionary<string, string>? environment = null, CancellationToken cancellationToken = default);

}
