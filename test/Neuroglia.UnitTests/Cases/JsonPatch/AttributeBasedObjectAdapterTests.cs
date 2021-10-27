using FluentAssertions;
using Microsoft.AspNetCore.JsonPatch;
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
            var friend1Id = (await this.Persons.AddAsync(new("Fake First Name", "Fake Last Naùe"))).Id;
            var friend2Id = (await this.Persons.AddAsync(new("Fake First Name", "Fake Last Naùe"))).Id;
            var originalFirstName = "Fake First Name";
            var originalLastName = "Fake Last Name";
            var newFirstName = "New " + originalFirstName;
            var newLastName = "New " + originalLastName;
            var newAddress = "New Address";
            var newBirthday = DateTimeOffset.Now;
            var newContactTel = "+32474769395";
            var target = new TestPerson(originalFirstName, originalLastName);
            var patch = new JsonPatchDocument<TestPerson>();
            patch.Replace(p => p.FirstName, newFirstName);
            patch.Replace(p => p.LastName, newLastName);
            patch.Add(p => p.Addresses, newAddress);
            patch.Add(p => p.FriendsIds, friend1Id);
            patch.Add(p => p.FriendsIds, friend2Id);
            patch.Remove(p => p.FriendsIds, 1);
            patch.Replace(p => p.Birthday, newBirthday);
            patch.Add(p => p.Contacts, new TestContact() { Tel = "0474769395" });
            patch.Replace(p => p.Contacts[0].Tel, newContactTel);

            //act
            patch.ApplyTo(target, this.Adapter);

            //assert
            target.FirstName.Should().Be(newFirstName);
            target.LastName.Should().Be(newLastName);
            target.Addresses.Should().Contain(newAddress);
            target.FriendsIds.Should().Contain(friend1Id);
            target.Birthday.Should().Be(newBirthday.DateTime);
            target.Contacts[0].Tel.Should().Be(newContactTel);
        }

    }

}
