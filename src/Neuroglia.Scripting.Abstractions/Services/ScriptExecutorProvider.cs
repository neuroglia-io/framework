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

using Microsoft.Extensions.DependencyInjection;

namespace Neuroglia.Scripting.Services;

/// <summary>
/// Represents the default implementation of the <see cref="IScriptExecutorProvider"/> interface
/// </summary>
/// <remarks>
/// Initializes a new <see cref="ScriptExecutorProvider"/>
/// </remarks>
/// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
public class ScriptExecutorProvider(IServiceProvider serviceProvider)
    : IScriptExecutorProvider
{

    /// <summary>
    /// Gets the current <see cref="IServiceProvider"/>
    /// </summary>
    protected IServiceProvider ServiceProvider { get; } = serviceProvider;
    /// <inheritdoc/>
    public virtual IScriptExecutor? GetExecutor(string language)
    {
        if (string.IsNullOrEmpty(language)) throw new ArgumentNullException(nameof(language));
        return this.GetExecutors(language).FirstOrDefault();
    }

    /// <inheritdoc/>
    public virtual IEnumerable<IScriptExecutor> GetExecutors(string language) => string.IsNullOrWhiteSpace(language) ? throw new ArgumentNullException(nameof(language)) : this.ServiceProvider.GetServices<IScriptExecutor>().Where(s => s.Supports(language));

}
