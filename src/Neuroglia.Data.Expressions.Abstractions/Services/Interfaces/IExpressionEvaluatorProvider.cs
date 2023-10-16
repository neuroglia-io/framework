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

namespace Neuroglia.Data.Expressions.Services;

/// <summary>
/// Defines the fundamentals of a service used to manage <see cref="IExpressionEvaluator"/>s
/// </summary>
public interface IExpressionEvaluatorProvider
{

    /// <summary>
    /// Gets the first <see cref="IExpressionEvaluator"/> that supports the specified expression language, if any
    /// </summary>
    /// <param name="language">The expression language for which to get an <see cref="IExpressionEvaluator"/></param>
    /// <returns>The <see cref="IExpressionEvaluator"/> that supports the specified expression language, if any</returns>
    IExpressionEvaluator? GetEvaluator(string language);

    /// <summary>
    /// Gets the <see cref="IExpressionEvaluator"/>s that support the specified expression language
    /// </summary>
    /// <param name="language">The expression language to get the <see cref="IExpressionEvaluator"/>s for</param>
    /// <returns>A new <see cref="IEnumerable{T}"/> containing the <see cref="IExpressionEvaluator"/> that support the specified expression language</returns>
    IEnumerable<IExpressionEvaluator> GetEvaluators(string language);

}
