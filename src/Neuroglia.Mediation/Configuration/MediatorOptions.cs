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
/// Represents the object used to configure a <see cref="Mediator"/> instance
/// </summary>
public class MediatorOptions
{

    /// <summary>
    /// Gets a <see cref="List{T}"/> containing the assemblies to scan for <see cref="IRequestHandler{TRequest, TResponse}"/> and <see cref="INotificationHandler{TNotification}"/> implementations
    /// </summary>
    public List<Assembly> AssembliesToScan { get; } = new();

    /// <summary>
    /// Gets a <see cref="List{T}"/> containing the <see cref="IMiddleware{TRequest, TResult}"/> types to apply to to all <see cref="IRequestHandler{TRequest, TResponse}"/> implementations
    /// </summary>
    public List<Type> DefaultPipelineBehaviors { get; } = new();

}
