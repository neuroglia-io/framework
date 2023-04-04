using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Data.Expressions;
using Neuroglia.Data.Expressions.JavaScript;
using Neuroglia.Serialization;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Text.Json;
using Xunit;

namespace Neuroglia.UnitTests.Cases.Data.Expressions
{

    public class JavaScriptExpressionEvaluatorTests
    {

        [Fact]
        public void Evaluate_PrimitiveOutput_UsingNewtonsoft_ShouldWork()
        {
            //arrange
            var evaluator = BuildExpressionEvaluatorWithNewtonsoftJsonSerializer();
            var value = 42;
            var expression = "input.value";
            var data = new { value };

            //act
            var result = (int)evaluator.Evaluate<double>(expression, data);

            //assert
            result.Should().Be(value);
        }

        [Fact]
        public void Evaluate_PrimitiveOutput_UsingSystemTextJson_ShouldWork()
        {
            //arrange
            var evaluator = BuildExpressionEvaluatorWithSystemTextJsonSerializer();
            var value = 42;
            var expression = "input.value";
            var data = new { value };

            //act
            var result = evaluator.Evaluate<int>(expression, data);

            //assert
            result.Should().Be(value);
        }

        [Fact]
        public void Evaluate_ComplexTypeOutput_UsingNewtonsoft_ShouldWork()
        {
            //arrange
            var evaluator = BuildExpressionEvaluatorWithNewtonsoftJsonSerializer();
            var value = 42;
            var expression = "${ input }";
            var data = new { value };

            //act
            var result = evaluator.Evaluate<ExpandoObject>(expression, data);

            //assert
            result.Should().BeEquivalentTo(data.ToExpandoObject());
        }

        [Fact]
        public void Evaluate_ComplexTypeOutput_UsingSystemTextJson_ShouldWork()
        {
            //arrange
            var evaluator = BuildExpressionEvaluatorWithSystemTextJsonSerializer();
            var value = 42;
            var expression = "${ input }";
            var data = new { value };

            //act
            var result = evaluator.Evaluate<ExpandoObject>(expression, data);

            //assert
            Assert.Equal(((System.Text.Json.JsonElement)((dynamic)result).value).GetInt64(), value);
        }

        [Fact]
        public void Evaluate_ComplexTypeInput_ShouldWork()
        {
            //arrange
            var evaluator = BuildExpressionEvaluatorWithNewtonsoftJsonSerializer();
            var bar = "bar";
            var baz = new { foo = "bar" };
            var obj = new { foo = "${ input.bar }", bar = "foo", baz = baz };
            var data = new { bar = bar };
            var expectedResult = new { foo = bar, bar = "foo", baz = baz.ToExpandoObject() };

            //act
            var result = evaluator.Evaluate(obj, data);

            //assert
            result.Should().BeEquivalentTo(expectedResult.ToExpandoObject());
        }

        [Fact]
        public void Evaluate_LargeData_UsingNewtonsoft_ShouldWork()
        {
            //arrange
            var evaluator = BuildExpressionEvaluatorWithNewtonsoftJsonSerializer();
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ExpandoObject>>(File.ReadAllText(Path.Combine("Assets", "dogs.json")));
            var expression = "input.filter(i => i.category?.name === CONST.category)[0]";
            var args = new Dictionary<string, object>() { { "CONST", new { category = "Pugal" } } };

            //act
            var result = evaluator.Evaluate(expression, data, args);

            //assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void Evaluate_LargeData_UsingSystemTextJson_ShouldWork()
        {
            //arrange
            var evaluator = BuildExpressionEvaluatorWithSystemTextJsonSerializer();
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ExpandoObject>>(File.ReadAllText(Path.Combine("Assets", "dogs.json")));
            var expression = "input.filter(i => i.category?.name === CONST.category)[0]";
            var args = new Dictionary<string, object>() { { "CONST", new { category = "Pugal" } } };

            //act
            var result = evaluator.Evaluate(expression, data, args);

            //assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void Evaluate_Object_UsingNewtonsoft_ShouldWork()
        {
            //arrange
            var evaluator = BuildExpressionEvaluatorWithNewtonsoftJsonSerializer();
            var expression = "({ foo: 'bar', fizz: 'buzz' })";
            var data = new { foo = "bar", fizz = "buzz" };

            //act
            var result = evaluator.Evaluate(expression, data);

            //assert
            result.Should().BeEquivalentTo(data.ToExpandoObject());
        }

        [Fact]
        public void Evaluate_Object_UsingSystemTextJson_ShouldWork()
        {
            //arrange
            var evaluator = BuildExpressionEvaluatorWithSystemTextJsonSerializer();
            var expression = "({ foo: 'bar', fizz: 'buzz' })";
            var data = new { foo = "bar", fizz = "buzz" };

            //act
            var result = evaluator.Evaluate(expression, data);

            //assert
            result.Should().BeEquivalentTo(data.ToExpandoObject());
        }

        [Fact]
        public void Evaluate_LargeExpression_UsingNewtonsoft_ShouldWork()
        {
            //arrange
            var evaluator = BuildExpressionEvaluatorWithNewtonsoftJsonSerializer();
            var data = new { };
            var expression = File.ReadAllText(Path.Combine("Assets", "pets.expression.js.txt"));
            var args = new Dictionary<string, object>() { { "CONST", new { category = "Pugal" } } };

            //act
            dynamic result = evaluator.Evaluate(expression, data, args);

            //assert
            Assert.NotEmpty(result.pets);
        }

        [Fact]
        public void Evaluate_LargeExpression_UsingSystemTextJson_ShouldWork()
        {
            //arrange
            var evaluator = BuildExpressionEvaluatorWithSystemTextJsonSerializer();
            var data = new { };
            var expression = File.ReadAllText(Path.Combine("Assets", "pets.expression.js.txt"));
            var args = new Dictionary<string, object>() { { "CONST", new { category = "Pugal" } } };

            //act
            dynamic result = evaluator.Evaluate(expression, data, args);

            //assert
            Assert.NotEmpty(result.pets);
        }

        [Fact]
        public void Evaluate_EscapedJsonInput_ShouldWork()
        {
            //arrange
            var evaluator = BuildExpressionEvaluatorWithNewtonsoftJsonSerializer();
            var json = File.ReadAllText(Path.Combine("Assets", "inputWithEscapedJson.json"));
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<ExpandoObject>(json);
            var expression = "input._user";

            //act
            dynamic result = evaluator.Evaluate(expression, data);

            //assert
            Assert.NotEmpty(result.name);
        }

        [Fact]
        public void Evaluate_String_Concatenation_ShouldWork()
        {
            //arrange
            var evaluator = BuildExpressionEvaluatorWithNewtonsoftJsonSerializer();
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<ExpandoObject>(File.ReadAllText(Path.Combine("Assets", "string-concat.input.json")));
            var expression = File.ReadAllText(Path.Combine("Assets", "string-concat.expression.js.txt"));

            //act
            string result = (string)evaluator.Evaluate(expression, data, typeof(string), null);

            //assert
            result.Should().Be("hello world");
        }

        [Fact]
        public void Evaluate_String_Interpolation_ShouldWork()
        {
            //arrange
            var evaluator = BuildExpressionEvaluatorWithNewtonsoftJsonSerializer();
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<ExpandoObject>(File.ReadAllText(Path.Combine("Assets", "string-interpolation.input.json")));
            var expression = File.ReadAllText(Path.Combine("Assets", "string-interpolation.expression.js.txt"));

            //act
            string result = (string)evaluator.Evaluate(expression, data, typeof(string), null);

            //assert
            result.Should().Be("hello world is a greeting");
        }

        [Fact]
        public void Evaluate_Complex_String_Substitution_ShouldWork()
        {
            //arrange
            var evaluator = BuildExpressionEvaluatorWithNewtonsoftJsonSerializer();
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<ExpandoObject>(File.ReadAllText(Path.Combine("Assets", "string-substitution.input.json")));
            var expression = File.ReadAllText(Path.Combine("Assets", "string-substitution.expression.js.txt"));

            //act
            string result = (string)evaluator.Evaluate(expression, data, typeof(string), null);

            //assert
            result.Should().Be("Hello world");
        }

        [Fact]
        public void Evaluate_String_With_Escaped_Quotes_ShouldWork()
        {
            //arrange
            var evaluator = BuildExpressionEvaluatorWithNewtonsoftJsonSerializer();
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<ExpandoObject>(File.ReadAllText(Path.Combine("Assets", "string-quoted.input.json")));
            var expression = File.ReadAllText(Path.Combine("Assets", "string-quoted.expression.js.txt"));

            //act
            string result = (string)evaluator.Evaluate(expression, data, typeof(string), null);

            //assert
            result.Should().Be(@"bar is ""bar""");
        }

        static IExpressionEvaluator BuildExpressionEvaluatorWithNewtonsoftJsonSerializer()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddNewtonsoftJsonSerializer();
            services.AddJavaScriptExpressionEvaluator();
            return services.BuildServiceProvider().GetRequiredService<IExpressionEvaluator>();
        }

        static IExpressionEvaluator BuildExpressionEvaluatorWithSystemTextJsonSerializer()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddJsonSerializer();
            services.AddJavaScriptExpressionEvaluator(builder => builder.UseSerializer<Neuroglia.Serialization.JsonSerializer>());
            return services.BuildServiceProvider().GetRequiredService<IExpressionEvaluator>();
        }

    }

}
