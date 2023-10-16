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

namespace Neuroglia.Mediation;

/// <summary>
/// Represents the attribute used to add <see cref="IMiddleware{TRequest, TResult}"/> to an <see cref="IRequestHandler{TRequest, TResult}"/>
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class PipelineMiddlewareAttribute
    : Attribute
{

    /// <summary>
    /// Initializes a new <see cref="PipelineMiddlewareAttribute"/>
    /// </summary>
    /// <param name="pipelineBehaviorType">The type of <see cref="IMiddleware{TRequest, TResult}"/> to apply</param>
    /// <param name="priority">The priority of the the referenced <see cref="IMiddleware{TRequest, TResult}"/></param>
    public PipelineMiddlewareAttribute(Type pipelineBehaviorType, int priority = 99)
    {
        if (pipelineBehaviorType == null) throw new ArgumentNullException(nameof(pipelineBehaviorType));
        if (pipelineBehaviorType.GetGenericType(typeof(IMiddleware<,>)) == null) throw new ArgumentException($"The specified type must be implement the '{typeof(IMiddleware<,>).Name}' interface", nameof(pipelineBehaviorType));
        this.PipelineBehaviorType = pipelineBehaviorType;
        this.Priority = priority;
    }

    /// <summary>
    /// Gets the type of <see cref="IMiddleware{TRequest, TResult}"/> to apply
    /// </summary>
    public Type PipelineBehaviorType { get; }

    /// <summary>
    /// Gets the priority of the referenced <see cref="IMiddleware{TRequest, TResult}"/>
    /// </summary>
    public int Priority { get; }

}
