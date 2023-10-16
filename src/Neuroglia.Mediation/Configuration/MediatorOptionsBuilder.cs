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

namespace Neuroglia.Mediation.Configuration;

/// <summary>
/// Represents the default implementation of the <see cref="IMediatorOptionsBuilder"/> interface
/// </summary>
public class MediatorOptionsBuilder
    : IMediatorOptionsBuilder
{

    /// <summary>
    /// Initializes a new <see cref="MediatorOptionsBuilder"/>
    /// </summary>
    public MediatorOptionsBuilder()
    {
        this.Options = new MediatorOptions();
    }

    /// <summary>
    /// Gets the <see cref="MediatorOptions"/> to configure
    /// </summary>
    protected MediatorOptions Options { get; }

    /// <inheritdoc/>
    public virtual IMediatorOptionsBuilder ScanAssembly(Assembly assembly)
    {
        if (assembly == null) throw new ArgumentNullException(nameof(assembly));
        if (!this.Options.AssembliesToScan.Contains(assembly)) this.Options.AssembliesToScan.Add(assembly);
        return this;
    }

    /// <inheritdoc/>
    public virtual IMediatorOptionsBuilder UseDefaultPipelineBehavior(Type pipelineType)
    {
        if (pipelineType == null) throw new ArgumentNullException(nameof(pipelineType));
        if (pipelineType.GetGenericType(typeof(IMiddleware<,>)) == null) throw new ArgumentException($"The specified type must be an implementation of the '{typeof(IMiddleware<,>).Name}' interface", nameof(pipelineType));
        this.Options.DefaultPipelineBehaviors.Add(pipelineType);
        return this;
    }

    /// <inheritdoc/>
    public virtual MediatorOptions Build() => this.Options;

}
