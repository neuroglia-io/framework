using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Data.Expressions;
using Neuroglia.Data.Expressions.JQ;
using Neuroglia.Serialization;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Text.Json;
using Xunit;

namespace Neuroglia.UnitTests.Cases.Data.Expressions
{

    public class JQExpressionEvaluatorTests
    {

        [Fact]
        public void Evaluate_PrimitiveOutput_UsingNewtonsoft_ShouldWork()
        {
            //arrange
            var evaluator = BuildExpressionEvaluatorWithNewtonsoftJsonSerializer();
            var value = 97;
            var expression = "${ .value }";
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
            var value = 97;
            var expression = "${ . }";
            var data = new { value };

            //act
            var result = evaluator.Evaluate<ExpandoObject>(expression, data);

            //assert
            result.Should().BeEquivalentTo(data.ToExpandoObject());
        }

        [Fact]
        public void Evaluate_ComplexTypeInput_UsingNewtonsoft_ShouldWork()
        {
            //arrange
            var evaluator = BuildExpressionEvaluatorWithNewtonsoftJsonSerializer();
            var bar = "bar";
            var baz = new { foo = "bar" };
            var obj = new { foo = "${ .bar }", bar = "foo", baz = baz };
            var data = new { bar = bar };
            var expectedResult = new { foo = bar, bar = "foo", baz = baz.ToExpandoObject() };

            //act
            var result = evaluator.Evaluate(obj, data);

            //assert
            result.Should().BeEquivalentTo(expectedResult.ToExpandoObject());
        }

        [Fact]
        public void Evaluate_PrimitiveOutput_UsingSystemTextJson_ShouldWork()
        {
            //arrange
            var evaluator = BuildExpressionEvaluatorWithSystemTextJsonSerializer();
            var value = 97;
            var expression = "${ .value }";
            var data = new { value };

            //act
            var result = evaluator.Evaluate<int>(expression, data);

            //assert
            result.Should().Be(value);
        }

        [Fact]
        public void Evaluate_ComplexTypeOutput_UsingSystemTextJson_ShouldWork()
        {
            //arrange
            var evaluator = BuildExpressionEvaluatorWithSystemTextJsonSerializer();
            var value = 97;
            var expression = "${ . }";
            var data = new { value }.ToExpandoObject();

            //act
            var result = evaluator.Evaluate<ExpandoObject>(expression, data);

            //assert
            Assert.Equal(((System.Text.Json.JsonElement)((dynamic)result).value).GetInt64(), value);
        }

        [Fact]
        public void Evaluate_LargeData_UsingNewtonsoft_ShouldWork()
        {
            //arrange
            var evaluator = BuildExpressionEvaluatorWithNewtonsoftJsonSerializer();
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ExpandoObject>>(File.ReadAllText(Path.Combine("Assets", "dogs.json")));
            var expression = ". | map(select(.category.name == $CONST.category))[0]";
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
            var expression = ". | map(select(.category.name == $CONST.category))[0]";
            var args = new Dictionary<string, object>() { { "CONST", new { category = "Pugal" } } };

            //act
            var result = evaluator.Evaluate(expression, data, args);

            //assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void Evaluate_LargeExpression_UsingNewtonsoft_ShouldWork()
        {
            //arrange
            var evaluator = BuildExpressionEvaluatorWithNewtonsoftJsonSerializer();
            var data = new { };
            var expression = File.ReadAllText(Path.Combine("Assets", "pets.expression.json"));
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
            var expression = File.ReadAllText(Path.Combine("Assets", "pets.expression.json"));
            var args = new Dictionary<string, object>() { { "CONST", new { category = "Pugal" } } };

            //act
            dynamic result = evaluator.Evaluate(expression, data, args);
            if(result is JsonElement jsonElem)
                result = jsonElem.Deserialize<ExpandoObject>();

            //assert
            Assert.NotNull(result);
        }

        static IExpressionEvaluator BuildExpressionEvaluatorWithNewtonsoftJsonSerializer()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddNewtonsoftJsonSerializer();
            services.AddJQExpressionEvaluator();
            return services.BuildServiceProvider().GetRequiredService<IExpressionEvaluator>();
        }

        static IExpressionEvaluator BuildExpressionEvaluatorWithSystemTextJsonSerializer()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddJsonSerializer();
            services.AddJQExpressionEvaluator(builder => builder.UseSerializer<Neuroglia.Serialization.JsonSerializer>());
            return services.BuildServiceProvider().GetRequiredService<IExpressionEvaluator>();
        }

    }

}
