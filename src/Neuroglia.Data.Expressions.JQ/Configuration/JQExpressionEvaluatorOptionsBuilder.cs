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

using Neuroglia.Serialization;

namespace Neuroglia.Data.Expressions.JQ.Configuration;

/// <summary>
/// Represents the default implementation of the <see cref="IJQExpressionEvaluatorOptionsBuilder"/> interface
/// </summary>
public class JQExpressionEvaluatorOptionsBuilder
    : IJQExpressionEvaluatorOptionsBuilder
{

    /// <summary>
    /// Gets the <see cref="JQExpressionEvaluatorOptions"/> to configure
    /// </summary>
    protected JQExpressionEvaluatorOptions Options { get; } = new();

    /// <inheritdoc/>
    public virtual IJQExpressionEvaluatorOptionsBuilder UseSerializer<TSerializer>()
        where TSerializer : class, IJsonSerializer
    {
        this.Options.SerializerType = typeof(TSerializer);
        return this;
    }

    /// <inheritdoc/>
    public virtual JQExpressionEvaluatorOptions Build() => this.Options;

}
