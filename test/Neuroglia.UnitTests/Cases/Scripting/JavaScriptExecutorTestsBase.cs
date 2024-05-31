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

using Neuroglia.Serialization.Json;

namespace Neuroglia.UnitTests.Cases.Scripting;
public abstract class JavaScriptExecutorTestsBase
    : ScriptExecutorTestsBase
{

    [Fact]
    public async Task Execute_Script_Should_Work()
    {
        //arrange
        var script = File.ReadAllText(Path.Combine("Assets", "scripts", "import-axios.js"));
        var arguments = new List<string>() { "foo", "bar" };
        var environment = new Dictionary<string, string>();

        //act
        var process = await this.Executor.ExecuteAsync(script, arguments, environment);
        await process.WaitForExitAsync();
        var stdOut = await process.StandardOutput.ReadToEndAsync();
        var stdErr = await process.StandardError.ReadToEndAsync();

        //assert
        stdErr.Should().BeNullOrWhiteSpace();
        stdOut.Should().NotBeNullOrWhiteSpace();
        process.ExitCode.Should().Be(0);
        var outputArgs = JsonSerializer.Default.Deserialize<string[]>(stdOut.Split('\n').First());
        outputArgs.Should().BeEquivalentTo(arguments);
    }

}