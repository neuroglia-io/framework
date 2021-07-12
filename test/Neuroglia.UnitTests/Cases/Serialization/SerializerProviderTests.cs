using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Serialization;
using System.Linq;
using Xunit;

namespace Neuroglia.UnitTests.Cases.Serialization
{

    public class SerializerProviderTests
    {

        public SerializerProviderTests()
        {
            ServiceCollection services = new();
            services.AddNewtonsoftJsonSerializer();
            services.AddJsonSerializer();
            services.AddProtobufSerializer();
            services.AddAvroSerializer();
            services.AddYamlDotNetSerializer();
            this.SerializerProvider = services.BuildServiceProvider().GetRequiredService<ISerializerProvider>();
        }

        protected ISerializerProvider SerializerProvider { get; }

        [Fact]
        public void Get_JSON_Serializer_Should_Return_2()
        {
            this.SerializerProvider.GetJsonSerializers().Count().Should().Be(2);
        }

    }

}
