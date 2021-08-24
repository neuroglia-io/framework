using FluentAssertions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.Extensions.DependencyInjection;
using Neuroglia.UnitTests.Data;
using System;
using Xunit;

namespace Neuroglia.UnitTests.Cases.JsonPatch
{

    public class AttributeBasedObjectAdapterTests
    {

        public AttributeBasedObjectAdapterTests()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<IJsonPatchMetadataProvider, JsonPatchMetadataProvider>();
            services.AddSingleton<AttributeBasedObjectAdapter>();
            IServiceProvider provider = services.BuildServiceProvider();
            this.Adapter = provider.GetRequiredService<AttributeBasedObjectAdapter>();
        }

        AttributeBasedObjectAdapter Adapter { get; }

        [Fact]
        public void ApplyPatchTo()
        {
            //arrange
            var originalFirstName = "Fake First Name";
            var originalLastName = "Fake Last Name";
            var newFirstName = "New " + originalFirstName;
            var newLastName = "New " + originalLastName;
            var target = new TestPerson(originalFirstName, originalLastName);
            var patch = new JsonPatchDocument();
            patch.Operations.Add(new Operation(OperationType.Replace.ToString(), nameof(TestPerson.FirstName), string.Empty, newFirstName));
            patch.Operations.Add(new Operation(OperationType.Replace.ToString(), nameof(TestPerson.LastName), string.Empty, newLastName));

            //act
            patch.ApplyTo(target, this.Adapter);

            //assert
            target.FirstName.Should().Be(newFirstName);
            target.LastName.Should().Be(newLastName);
        }

    }

}
