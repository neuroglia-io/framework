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

using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Data.Expressions;
using Neuroglia.Data.Expressions.JavaScript;
using Neuroglia.Data.Expressions.Services;
using Neuroglia.Serialization;
using Neuroglia.Serialization.Json;
using System.Dynamic;

namespace Neuroglia.UnitTests.Cases.Data.Expressions.Evaluators;

public class JavaScriptExpressionEvaluatorTests
{

    public JavaScriptExpressionEvaluatorTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSerialization();
        services.AddJavaScriptExpressionEvaluator();
        var serviceProvider = services.BuildServiceProvider();
        ExpressionEvaluator = serviceProvider.GetRequiredService<IExpressionEvaluator>();
    }

    protected IExpressionEvaluator ExpressionEvaluator { get; }

    [Fact]
    public void Supports_Should_Work()
    {
        //arrange
        var supported = "javascript";
        var unsupported = "jq";

        //assert
        ExpressionEvaluator.Supports(supported).Should().BeTrue();
        ExpressionEvaluator.Supports(unsupported).Should().BeFalse();
    }

    [Fact]
    public async Task Evaluate_PrimitiveOutput_ShouldWork()
    {
        //arrange
        var value = 42;
        var expression = "input.value";
        var data = new { value };

        //act
        var result = (int)(await this.ExpressionEvaluator.EvaluateAsync<double>(expression, data));

        //assert
        result.Should().Be(value);
    }

    [Fact]
    public async Task Evaluate_ComplexTypeOutput_ShouldWork()
    {
        //arrange
        var value = 42;
        var expression = "${ input }";
        var data = new { value };

        //act
        var result = await this.ExpressionEvaluator.EvaluateAsync<ExpandoObject>(expression, data);

        //assert
        result.Should().BeEquivalentTo(data.ToExpandoObject());
    }

    [Fact]
    public async Task Evaluate_ComplexTypeInput_ShouldWork()
    {
        //arrange
        var bar = "bar";
        var baz = new { foo = "bar" };
        var obj = new { foo = "${ input.bar }", bar = "foo", baz };
        var data = new { bar };
        var expectedResult = new { foo = bar, bar = "foo", baz = baz.ToExpandoObject() };

        //act
        var result = await this.ExpressionEvaluator.EvaluateAsync(obj, data);

        //assert
        result.Should().BeEquivalentTo(expectedResult.ToExpandoObject());
    }

    [Fact]
    public async Task Evaluate_LargeData_ShouldWork()
    {
        //arrange
        var data = JsonSerializer.Default.Deserialize<List<object>>(File.ReadAllText(Path.Combine("Assets", "dogs.json")))!;
        var expression = "input.filter(i => i.category?.name === CONST.category)[0]";
        var args = new Dictionary<string, object>() { { "CONST", new { category = "Pugal" } } };

        //act
        var result = await this.ExpressionEvaluator.EvaluateAsync(expression, data, args);

        //assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Evaluate_Object_ShouldWork()
    {
        //arrange
        var expression = "({ foo: 'bar', fizz: 'buzz' })";
        var data = new { foo = "bar", fizz = "buzz" };

        //act
        var result = await this.ExpressionEvaluator.EvaluateAsync(expression, data);

        //assert
        result.Should().BeEquivalentTo(data.ToExpandoObject());
    }

    [Fact]
    public async Task Evaluate_LargeExpression_ShouldWork()
    {
        //arrange
        var data = new { };
        var expression = File.ReadAllText(Path.Combine("Assets", "pets.expression.js.txt"));
        var args = new Dictionary<string, object>() { { "CONST", new { category = "Pugal" } } };

        //act
        dynamic? result = await this.ExpressionEvaluator.EvaluateAsync(expression, data, args);

        //assert
        Assert.NotEmpty(result?.pets);
    }

    [Fact]
    public async Task Evaluate_EscapedJsonInput_ShouldWork()
    {
        //arrange
        var json = File.ReadAllText(Path.Combine("Assets", "inputWithEscapedJson.json"));
        var data = Newtonsoft.Json.JsonConvert.DeserializeObject<ExpandoObject>(json)!;
        var expression = "input._user";

        //act
        dynamic? result = await this.ExpressionEvaluator.EvaluateAsync(expression, data);

        //assert
        Assert.NotEmpty(result?.name);
    }

    [Fact]
    public async Task Evaluate_String_Concatenation_ShouldWork()
    {
        //arrange
        var data = JsonSerializer.Default.Deserialize<ExpandoObject>(File.ReadAllText(Path.Combine("Assets", "string-concat.input.json")))!;
        var expression = File.ReadAllText(Path.Combine("Assets", "string-concat.expression.js.txt"));

        //act
        var result = (string)(await this.ExpressionEvaluator.EvaluateAsync(expression, data, null, typeof(string)))!;

        //assert
        result.Should().Be("hello world");
    }

    [Fact]
    public async Task Evaluate_String_Interpolation_ShouldWork()
    {
        //arrange
        var data = NJsonSerializer.Default.Deserialize<ExpandoObject>(File.ReadAllText(Path.Combine("Assets", "string-interpolation.input.json")))!;
        var expression = File.ReadAllText(Path.Combine("Assets", "string-interpolation.expression.js.txt"));

        //act
        var result = (string)(await this.ExpressionEvaluator.EvaluateAsync(expression, data, null, typeof(string)))!;

        //assert
        result.Should().Be("hello world is a greeting");
    }

    [Fact]
    public async Task Evaluate_Complex_String_Substitution_ShouldWork()
    {
        //arrange
        var data = JsonSerializer.Default.Deserialize<ExpandoObject>(File.ReadAllText(Path.Combine("Assets", "string-substitution.input.json")))!;
        var expression = File.ReadAllText(Path.Combine("Assets", "string-substitution.expression.js.txt"));

        //act
        var result = (string)(await this.ExpressionEvaluator.EvaluateAsync(expression, data, null, typeof(string)))!;

        //assert
        result.Should().Be("Hello world");
    }

    [Fact]
    public async Task Evaluate_String_With_Escaped_Quotes_ShouldWork()
    {
        //arrange
        var data = JsonSerializer.Default.Deserialize<ExpandoObject>(File.ReadAllText(Path.Combine("Assets", "string-quoted.input.json")))!;
        var expression = File.ReadAllText(Path.Combine("Assets", "string-quoted.expression.js.txt"));

        //act
        var result = (string)(await this.ExpressionEvaluator.EvaluateAsync(expression, data, null, typeof(string)))!;

        //assert
        result.Should().Be(@"bar is ""bar""");
    }

}