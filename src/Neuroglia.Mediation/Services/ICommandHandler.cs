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
/// Defines the fundamentals of a service used to handle <see cref="ICommand"/>s of the specified type
/// </summary>
public interface ICommandHandler
{



}

/// <summary>
/// Defines the fundamentals of a service used to handle <see cref="ICommand"/>s of the specified type
/// </summary>
/// <typeparam name="TCommand">The type of <see cref="ICommand"/>s to handle</typeparam>
public interface ICommandHandler<TCommand>
    : ICommandHandler, IRequestHandler<TCommand, IOperationResult>
    where TCommand : class, ICommand<IOperationResult>
{



}

/// <summary>
/// Defines the fundamentals of a service used to handle <see cref="ICommand"/>s of the specified type
/// </summary>
/// <typeparam name="TCommand">The type of <see cref="ICommand"/>s to handle</typeparam>
/// <typeparam name="T">The type of data wrapped by the resulting <see cref="IOperationResult"/></typeparam>
public interface ICommandHandler<TCommand, T>
    : ICommandHandler, IRequestHandler<TCommand, IOperationResult<T>>
    where TCommand : class, ICommand<IOperationResult<T>>
{



}
