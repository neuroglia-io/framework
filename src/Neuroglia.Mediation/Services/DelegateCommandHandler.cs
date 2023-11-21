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

namespace Neuroglia.Mediation.Services;

/// <summary>
/// Represents a delegate-based implementation of the <see cref="ICommandHandler"/> interface
/// </summary>
/// <typeparam name="TCommand">The type of notification to handle</typeparam>
public class DelegateCommandHandler<TCommand>
    : ICommandHandler<TCommand>
    where TCommand : class, ICommand<IOperationResult>
{

    /// <summary>
    /// Initializes a new <see cref="DelegateCommandHandler{TCommand}"/>
    /// </summary>
    /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
    /// <param name="delegate">The delegate used to handle the <see cref="ICommand"/></param>
    public DelegateCommandHandler(IServiceProvider serviceProvider, Func<IServiceProvider, TCommand, CancellationToken, Task<IOperationResult>> @delegate)
    {
        this.ServiceProvider = serviceProvider;
        this.Delegate = @delegate;
    }

    /// <summary>
    /// Gets the current <see cref="IServiceProvider"/>
    /// </summary>
    protected IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Gets the delegate used to handle the notification
    /// </summary>
    protected Func<IServiceProvider, TCommand, CancellationToken, Task<IOperationResult>> Delegate { get; }

    /// <inheritdoc/>
    public virtual Task<IOperationResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default) => this.Delegate.Invoke(this.ServiceProvider, command, cancellationToken);

}

/// <summary>
/// Represents a delegate-based implementation of the <see cref="ICommandHandler"/> interface
/// </summary>
/// <typeparam name="TCommand">The type of notification to handle</typeparam>
/// <typeparam name="T">The type of data wrapped by the <see cref="IOperationResult"/></typeparam>
public class DelegateCommandHandler<TCommand, T>
    : ICommandHandler<TCommand, T>
    where TCommand : class, ICommand<IOperationResult<T>, T>
{

    /// <summary>
    /// Initializes a new <see cref="DelegateCommandHandler{TCommand}"/>
    /// </summary>
    /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
    /// <param name="delegate">The delegate used to handle the <see cref="ICommand"/></param>
    public DelegateCommandHandler(IServiceProvider serviceProvider, Func<IServiceProvider, TCommand, CancellationToken, Task<IOperationResult<T>>> @delegate)
    {
        this.ServiceProvider = serviceProvider;
        this.Delegate = @delegate;
    }

    /// <summary>
    /// Gets the current <see cref="IServiceProvider"/>
    /// </summary>
    protected IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Gets the delegate used to handle the notification
    /// </summary>
    protected Func<IServiceProvider, TCommand, CancellationToken, Task<IOperationResult<T>>> Delegate { get; }

    /// <inheritdoc/>
    public virtual Task<IOperationResult<T>> HandleAsync(TCommand command, CancellationToken cancellationToken = default) => this.Delegate.Invoke(this.ServiceProvider, command, cancellationToken);

}
