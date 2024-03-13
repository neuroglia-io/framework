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

namespace Neuroglia;

/// <summary>
/// Represents an <see cref="Exception"/> described by <see cref="ProblemDetails"/>
/// </summary>
/// <remarks>
/// Initializes a new <see cref="ProblemDetailsException"/>
/// </remarks>
/// <param name="problem">An object that describes the problem that has occurred</param>
public class ProblemDetailsException(ProblemDetails problem)
    : Exception($"[{problem.Status} - {problem.Title}] {problem.Detail}{(problem.Errors?.Count > 0 ? Environment.NewLine + string.Join(Environment.NewLine, problem.Errors.Select(e => $"{e.Key}: {string.Join(", ", e.Value)}")) : "")}")
{

    /// <summary>
    /// An object that describes the problem that has occurred
    /// </summary>
    public ProblemDetails Problem { get; } = problem;

}