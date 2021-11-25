using CloudNative.CloudEvents;
using Neuroglia.UnitTests.Data;
using System;

namespace Neuroglia.UnitTests.Factories
{

    internal static class CloudEventFactory
    {

        internal static CloudEvent Create()
        {
            var source = new Uri("https://fake.source.unit.test");
            var type = "test.unit.source.fake/events/fake";
            var street = "Fake Street";
            var zipCode = "Fake Zip Code";
            var city = "Fake City";
            var country = "Fake Country";
            var data = new TestAddress() { Street = street, ZipCode = zipCode, City = city, Country = country };
            return new()
            {
                Id = Guid.NewGuid().ToString(),
                Source = source,
                Type = type,
                Data = data
            };
        }

    }

}
