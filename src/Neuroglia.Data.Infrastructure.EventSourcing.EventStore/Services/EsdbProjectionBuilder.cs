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

using EventStore.Client;
using Lambda2Js;
using System.Linq.Expressions;
using System.Text;

namespace Neuroglia.Data.Infrastructure.EventSourcing.Services;

/// <summary>
/// Represents the EventStore implementation of the <see cref="IProjectionBuilder{TState}"/> interface
/// </summary>
/// <param name="name">The name of the projection to build</param>
/// <param name="projections">The underlying EventStore projection management API client</param>
public class EsdbProjectionBuilder<TState>(string name, EventStoreProjectionManagementClient projections)
    : IProjectionSourceBuilder<TState>, IProjectionBuilder<TState>
{

    /// <summary>
    /// Gets the name of the projection to build
    /// </summary>
    protected string Name { get; } = name;

    /// <summary>
    /// Gets the name of the stream, if any, from which to process events
    /// </summary>
    protected string? StreamName { get; set; }

    /// <summary>
    /// Gets the underlying EventStore projection management API client
    /// </summary>
    protected EventStoreProjectionManagementClient Projections { get; } = projections;

    /// <summary>
    /// Gets the <see cref="Expression"/> of the <see cref="Func{TResult}"/>, if any, used to create the projection's initial state
    /// </summary>
    protected Expression<Func<object>>? GivenFactory { get; set; }

    /// <summary>
    /// Gets the <see cref="Expression"/> of the predicate <see cref="Func{T1, T2, TResult}"/>, if any, used to filter incoming events
    /// </summary>
    protected Expression<Func<TState, IEventRecord, bool>>? WhenPredicate { get; set; }

    /// <summary>
    /// Gets the <see cref="Expression"/> of an <see cref="Action{T1, T2}"/> to perform on the projection when a matching event is processed
    /// </summary>
    protected List<Expression<Action<TState, IEventRecord>>>? ThenActions { get; set; }

    /// <inheritdoc/>
    public virtual IProjectionBuilder<TState> FromStream(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        this.StreamName = name;
        return this;
    }

    /// <inheritdoc/>
    public virtual IProjectionBuilder<TState> Given(Expression<Func<object>> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        this.GivenFactory = factory;
        return this;
    }

    /// <inheritdoc/>
    public virtual IProjectionBuilder<TState> When(Expression<Func<TState, IEventRecord, bool>> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        this.WhenPredicate = this.WhenPredicate == null
           ? predicate
           : Expression.Lambda<Func<TState, IEventRecord, bool>>(
               Expression.AndAlso(
                   this.WhenPredicate.Body,
                   predicate.Body),
               this.WhenPredicate.Parameters);
        return this;
    }

    /// <inheritdoc/>
    public virtual IProjectionBuilder<TState> Then(Expression<Action<TState, IEventRecord>> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        this.ThenActions ??= [];
        this.ThenActions.Add(action);
        return this;
    }

    /// <summary>
    /// Builds the projection on EventStore
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    public virtual async Task BuildAsync(CancellationToken cancellationToken = default)
    {
        var query = this.BuildQuery();
        await this.Projections.CreateContinuousAsync(this.Name, query, true, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Builds the EventStore JavaScript query expression of the projection to build
    /// </summary>
    /// <returns>The compiled EventStore JavaScript query expression of the projection to build</returns>
    protected virtual string BuildQuery()
    {
        var compilationOptions = new JavascriptCompilationOptions(JsCompilationFlags.BodyOnly, EsdbJavaScriptConversion.Extensions);
        var builder = new StringBuilder();
        builder.AppendLine($"fromStream('{this.StreamName}')");
        builder.AppendLine(@"    .when({");
        if (this.GivenFactory != null) builder.AppendLine(@$"       $init: () => {this.GivenFactory.CompileToJavascript(compilationOptions)},");
        builder.AppendLine(@"       $any: (state, e) => {");
        if (this.WhenPredicate != null) builder.AppendLine(@$"            if(!({this.WhenPredicate.CompileToJavascript(compilationOptions)})) return;");
        if (this.ThenActions?.Count > 0) this.ThenActions.ForEach(a => builder.AppendLine(@$"            {a.CompileToJavascript(compilationOptions)};"));
        builder.AppendLine(@"        }");
        builder.AppendLine(@"    });");
        return builder.ToString();
    }

}