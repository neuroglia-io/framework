using FluentAssertions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Caching;
using Neuroglia.Data;
using Neuroglia.UnitTests.Data;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Neuroglia.UnitTests.Cases.JsonPatch
{

    public class AttributeBasedObjectAdapterTests
    {

        public AttributeBasedObjectAdapterTests()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddLogging();
            services.AddSingleton<IJsonPatchMetadataProvider, JsonPatchMetadataProvider>();
            services.AddSingleton<AttributeBasedObjectAdapter>();
            services.AddMemoryDistributedCache();
            services.AddDistributedCacheRepository<TestPerson, Guid>(ServiceLifetime.Singleton);
            IServiceProvider provider = services.BuildServiceProvider();
            this.Persons = provider.GetRequiredService<IRepository<TestPerson>>();
            this.Adapter = provider.GetRequiredService<AttributeBasedObjectAdapter>();
        }

        AttributeBasedObjectAdapter Adapter { get; }

        IRepository<TestPerson> Persons { get; }

        [Fact]
        public async Task ApplyPatchTo()
        {
            //arrange
            var friendId = (await this.Persons.AddAsync(new("Fake First Name", "Fake Last Naùe"))).Id;
            var originalFirstName = "Fake First Name";
            var originalLastName = "Fake Last Name";
            var newFirstName = "New " + originalFirstName;
            var newLastName = "New " + originalLastName;
            var newAddress = "New Address";
            var target = new TestPerson(originalFirstName, originalLastName);
            var patch = new JsonPatchDocument();
            patch.Operations.Add(new Operation(OperationType.Replace.ToString(), nameof(TestPerson.FirstName), string.Empty, newFirstName));
            patch.Operations.Add(new Operation(OperationType.Replace.ToString(), nameof(TestPerson.LastName), string.Empty, newLastName));
            patch.Operations.Add(new Operation(OperationType.Add.ToString(), nameof(TestPerson.Addresses), string.Empty, newAddress));
            patch.Operations.Add(new Operation(OperationType.Add.ToString(), nameof(TestPerson.FriendsIds), string.Empty, friendId));

            //act
            patch.ApplyTo(target, this.Adapter);

            //assert
            target.FirstName.Should().Be(newFirstName);
            target.LastName.Should().Be(newLastName);
            target.Addresses.Should().Contain(newAddress);
            target.FriendsIds.Should().Contain(friendId);
        }

    }

}
