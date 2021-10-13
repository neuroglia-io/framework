using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Data;
using Neuroglia.UnitTests.Data;
using Simple.OData.Client;
using System;
using System.Linq;
using Xunit;

namespace Neuroglia.UnitTests.Cases.Data.OData
{

    public class ODataListTests
    {

        public ODataListTests()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IODataClient>(new ODataClient("https://services.odata.org/V4/OData/OData.svc/"));
            this.ServiceProvider = services.BuildServiceProvider();
        }

        IServiceProvider ServiceProvider { get; }

        [Fact]
        public void Where_ShouldWork()
        {
            //arrange
            var list = new ODataList<PersonDetails>(this.ServiceProvider);
            Func<PersonDetails, bool> predicate = p => p.Age >= 21;

            //act
            var filtered = list.Where(predicate);

            //assert
            list.ToList().Where(predicate).Should().BeEquivalentTo(filtered);
        }

        [Fact]
        public void OrderBy_ShouldWork()
        {
            //arrange
            var list = new ODataList<PersonDetails>(this.ServiceProvider);
            Func<PersonDetails, byte> selector = p => p.Age;

            //act
            var ordered = list.OrderBy(selector);

            //assert
            list.ToList().OrderBy(selector).Should().BeEquivalentTo(ordered);
        }

        [Fact]
        public void OrderByDescending_ShouldWork()
        {
            //arrange
            var list = new ODataList<PersonDetails>(this.ServiceProvider);
            Func<PersonDetails, byte> selector = p => p.Age;

            //act
            var ordered = list.OrderByDescending(selector);

            //assert
            list.ToList().OrderByDescending(selector).Should().BeEquivalentTo(ordered);
        }

        [Fact]
        public void Select_ShouldWork()
        {
            //arrange
            var list = new ODataList<PersonDetails>(this.ServiceProvider);
            Func<PersonDetails, byte> selector = p => p.Age;

            //act
            var ordered = list.Select(selector);

            //assert
            list.ToList().Select(selector).Should().BeEquivalentTo(ordered);
        }

        [Fact]
        public void Skip_ShouldWork()
        {
            //arrange
            var list = new ODataList<PersonDetails>(this.ServiceProvider);
            int amount = 3;

            //act
            var ordered = list.Skip(amount);

            //assert
            list.ToList().Skip(amount).Should().BeEquivalentTo(ordered);
        }

        [Fact]
        public void Take_ShouldWork()
        {
            //arrange
            var list = new ODataList<PersonDetails>(this.ServiceProvider);
            int amount = 2;

            //act
            var ordered = list.Take(amount);

            //assert
            list.ToList().Take(amount).Should().BeEquivalentTo(ordered);
        }

    }

}
