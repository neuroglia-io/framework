using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Data;
using Neuroglia.UnitTests.Data;
using Simple.OData.Client;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Neuroglia.UnitTests.Cases.Data.OData
{

    public class ODataQueryableTests
    {

        public ODataQueryableTests()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IODataClient>(new ODataClient("https://services.odata.org/V4/OData/OData.svc/"));
            services.AddPluralizer();
            this.ServiceProvider = services.BuildServiceProvider();
        }

        IServiceProvider ServiceProvider { get; }

        [Fact]
        public async Task CountAsync_ShouldWork()
        {
            //arrange
            IQueryable<PersonDetails> query = new ODataQueryable<PersonDetails>(this.ServiceProvider);
            Expression<Func<PersonDetails, bool>> predicate = p => p.Age >= 28;

            //act
            var count = await query.CountAsync(predicate);

            //assert
            query.ToList().Where(predicate.Compile()).Count().Should().Be(count);
            query.ToList().Count(predicate.Compile()).Should().Be(count);
        }

        [Fact]
        public void Expand_ShouldWork()
        {
            //arrange
            IODataQueryable<Person> query = new ODataQueryable<Person>(this.ServiceProvider);
            Expression<Func<Person, PersonDetails>> selector = p => p.PersonDetail;

            //act
            var filtered = query.Expand(selector);

            //assert
            filtered.ToList().All(p => p.PersonDetail != null).Should().BeTrue();
        }

        [Fact]
        public void Where_ShouldWork()
        {
            //arrange
            IQueryable<PersonDetails> query = new ODataQueryable<PersonDetails>(this.ServiceProvider);
            Func<PersonDetails, bool> predicate = p => p.Age >= 28;

            //act
            var filtered = query.Where(predicate);

            //assert
            query.ToList().Where(predicate).Should().BeEquivalentTo(filtered);
        }

        [Fact]
        public void OrderBy_ShouldWork()
        {
            //arrange
            IQueryable<PersonDetails> query = new ODataQueryable<PersonDetails>(this.ServiceProvider);
            Func<PersonDetails, byte> selector = p => p.Age;

            //act
            var ordered = query.OrderBy(selector);

            //assert
            query.ToList().OrderBy(selector).Should().BeEquivalentTo(ordered);
        }

        [Fact]
        public void OrderByDescending_ShouldWork()
        {
            //arrange
            IQueryable<PersonDetails> query = new ODataQueryable<PersonDetails>(this.ServiceProvider);
            Func<PersonDetails, byte> selector = p => p.Age;

            //act
            var ordered = query.OrderByDescending(selector);

            //assert
            query.ToList().OrderByDescending(selector).Should().BeEquivalentTo(ordered);
        }

        [Fact]
        public void Select_ShouldWork()
        {
            //arrange
            IQueryable<PersonDetails> query = new ODataQueryable<PersonDetails>(this.ServiceProvider);
            Func<PersonDetails, byte> selector = p => p.Age;

            //act
            var ordered = query.Select(selector);

            //assert
            query.ToList().Select(selector).Should().BeEquivalentTo(ordered);
        }

        [Fact]
        public void Skip_ShouldWork()
        {
            //arrange
            IQueryable<PersonDetails> query = new ODataQueryable<PersonDetails>(this.ServiceProvider);
            int amount = 3;

            //act
            var ordered = query.Skip(amount);

            //assert
            query.ToList().Skip(amount).Should().BeEquivalentTo(ordered);
        }

        [Fact]
        public void Take_ShouldWork()
        {
            //arrange
            IQueryable<PersonDetails> query = new ODataQueryable<PersonDetails>(this.ServiceProvider);
            int amount = 2;

            //act
            var ordered = query.Take(amount);

            //assert
            query.ToList().Take(amount).Should().BeEquivalentTo(ordered);
        }

    }

}
