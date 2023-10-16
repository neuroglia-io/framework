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

using FluentValidation;
using FluentValidation.Results;
using System.Net;

namespace Neuroglia.Mediation;

/// <summary>
/// Represents an <see cref="IMiddleware{TRequest, TResult}"/> used to validate <see cref="IRequest"/>s
/// </summary>
/// <typeparam name="TRequest">The type of <see cref="IRequest"/> to handle</typeparam>
/// <typeparam name="TResult">The type of expected <see cref="IOperationResult"/></typeparam>
public class FluentValidationMiddleware<TRequest, TResult>
    : IMiddleware<TRequest, TResult>
    where TRequest : IRequest<TResult>
    where TResult : IOperationResult
{

    /// <summary>
    /// Initializes a new <see cref="FluentValidationMiddleware{TRequest, TResult}"/>
    /// </summary>
    /// <param name="validators">An <see cref="IEnumerable{T}"/> containing the services used to validate <see cref="IRequest"/>s handled by the <see cref="FluentValidationMiddleware{TRequest, TResult}"/></param>
    public FluentValidationMiddleware(IEnumerable<IValidator<TRequest>> validators)
    {
        this.Validators = validators;
    }

    /// <summary>
    /// Gets an <see cref="IEnumerable{T}"/> containing the services used to validate <see cref="IRequest"/>s handled by the <see cref="FluentValidationMiddleware{TRequest, TResult}"/>
    /// </summary>
    protected virtual IEnumerable<IValidator<TRequest>> Validators { get; }

    /// <inheritdoc/>
    public virtual async Task<TResult> HandleAsync(TRequest request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken = default)
    {
        var validationFailures = new List<ValidationFailure>();
        foreach (var validator in this.Validators)
        {
            var validationResult = validator.Validate(request);
            if (!validationResult.IsValid) validationFailures.AddRange(validationResult.Errors);
        }
        if (!validationFailures.Any()) return await next();
        TResult result;
        Type resultType;
        if (typeof(IOperationResult).IsAssignableFrom(typeof(TResult)))
        {
            if (typeof(TResult).IsGenericType) resultType = typeof(OperationResult<>).MakeGenericType(typeof(TResult).GetGenericArguments().First());
            else resultType = typeof(OperationResult);
            result = (TResult)Activator.CreateInstance
            (
                resultType, 
                (int)HttpStatusCode.BadRequest, 
                new Error[] 
                { 
                    new Error(ErrorTypes.Invalid, "Validation Failure", (int)HttpStatusCode.BadRequest, "Failed to validate the request", errors: validationFailures.GroupBy(f => f.PropertyName).Select(g => new KeyValuePair<string, string[]>(g.Key, g.Select(e => e.ErrorMessage).ToArray()))) 
                }
            )!;
        }
        else
        {
            result = default!;
        }
        return result;
    }

}
