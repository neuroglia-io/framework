/*
 * Copyright © 2021 Neuroglia SPRL. All rights reserved.
 * <p>
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * <p>
 * http://www.apache.org/licenses/LICENSE-2.0
 * <p>
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */
using System;
using System.Reflection;

namespace Neuroglia.Mediation.Configuration
{

    /// <summary>
    /// Defines the fundamentals of a service used to build and configure a <see cref="Mediator"/> instance
    /// </summary>
    public interface IMediatorOptionsBuilder
    {

        /// <summary>
        /// Scans the specified <see cref="Assembly"/> for <see cref="IRequestHandler{TRequest, TResponse}"/> and <see cref="INotificationHandler{TNotification}"/> implementations
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/> to scan</param>
        /// <returns>The configured <see cref="IMediatorOptionsBuilder"/></returns>
        IMediatorOptionsBuilder ScanAssembly(Assembly assembly);

        /// <summary>
        /// Applies the specified <see cref="IMiddleware{TRequest, TResponse}"/> to all <see cref="IRequestHandler{TRequest, TResponse}"/> implementations<para></para>
        /// The order in which this method is called defines the order in which the behaviors will be called in the pipeline
        /// </summary>
        /// <param name="pipelineType">The type of the default <see cref="IMiddleware{TRequest, TResult}"/> to use</param>
        /// <returns>The configured <see cref="IMediatorOptionsBuilder"/></returns>
        IMediatorOptionsBuilder UseDefaultPipelineBehavior(Type pipelineType);

        /// <summary>
        /// Builds the <see cref="MediatorOptions"/>
        /// </summary>
        /// <returns>The resulting <see cref="MediatorOptions"/></returns>
        MediatorOptions Build();

    }

}
