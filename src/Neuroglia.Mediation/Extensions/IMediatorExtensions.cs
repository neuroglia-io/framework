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

using Neuroglia.Data.Guards;

namespace Neuroglia.Mediation;

/// <summary>
/// Defines extensions for <see cref="IMediator"/>s
/// </summary>
public static class IMediatorExtensions
{

    /// <summary>
    /// Executes the specified <see cref="IRequest"/> and unwraps the returned <see cref="IOperationResult"/>
    /// </summary>
    /// <typeparam name="T">The type of data wrapped by the <see cref="IOperationResult"/> returned by the <see cref="IRequest"/> to execute</typeparam>
    /// <param name="mediator">The <see cref="IMediator"/> used to execute the specified <see cref="IRequest"/></param>
    /// <param name="request">The <see cref="IRequest"/> to execute</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The data returned by the <see cref="IRequest"/>'s execution, in case of success</returns>
    public static async Task<T?> ExecuteAndUnwrapAsync<T>(this IMediator mediator, IRequest<IOperationResult<T>> request, CancellationToken cancellationToken = default)
    {
        var result = await mediator.ExecuteAsync(request, cancellationToken);
        if (result.Status < 200 || result.Status > 299) throw new GuardException($"An error has occurred during the request's execution:{Environment.NewLine}{string.Join(Environment.NewLine, result.Errors == null ? Array.Empty<string>() : result.Errors.Select(e => $"{e.Status}: {e.Detail}"))}", null);
        return result.Data;
    }

    /// <summary>
    /// Executes the specified <see cref="IRequest"/> and unwraps the returned <see cref="IOperationResult"/>
    /// </summary>
    /// <param name="mediator">The <see cref="IMediator"/> used to execute the specified <see cref="IRequest"/></param>
    /// <param name="request">The <see cref="IRequest"/> to execute</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    public static async Task ExecuteAndUnwrapAsync(this IMediator mediator, IRequest<IOperationResult> request, CancellationToken cancellationToken = default)
    {
        var result = await mediator.ExecuteAsync(request, cancellationToken);
        if (result.Status < 200 || result.Status > 299) throw new GuardException($"An error has occurred during the request's execution:{Environment.NewLine}{string.Join(Environment.NewLine, result.Errors == null ? Array.Empty<string>() : result.Errors.Select(e => $"{e.Status}: {e.Detail}"))}", null);
    }

}