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
using Neuroglia.Serialization;

namespace Neuroglia.Data.Expressions.JQ.Configuration
{
    /// <summary>
    /// Defines the fundamentals of a service used to build <see cref="JQExpressionEvaluatorOptions"/>
    /// </summary>
    public interface IJQExpressionEvaluatorOptionsBuilder
    {

        /// <summary>
        /// Configures the <see cref="JQExpressionEvaluator"/> to use the specified <see cref="IJsonSerializer"/> type
        /// </summary>
        /// <typeparam name="TSerializer">The type of <see cref="IJsonSerializer"/> to use</typeparam>
        /// <returns>The configured <see cref="IJQExpressionEvaluatorOptionsBuilder"/></returns>
        IJQExpressionEvaluatorOptionsBuilder UseSerializer<TSerializer>()
            where TSerializer : class, IJsonSerializer;

        /// <summary>
        /// Builds the <see cref="JQExpressionEvaluatorOptions"/>
        /// </summary>
        /// <returns>New <see cref="JQExpressionEvaluatorOptions"/></returns>
        JQExpressionEvaluatorOptions Build();

    }
}
