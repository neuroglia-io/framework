using ProtoBuf;

namespace Neuroglia.UnitTests.Data
{

    [ProtoContract]
    public class TestAddress
    {

        [ProtoMember(1)]
        public string Street { get; set; }

        [ProtoMember(2)]
        public string PostalCode { get; set; }

        [ProtoMember(3)]
        public string City { get; set; }

        [ProtoMember(4)]
        public string Country { get; set; }

    }

}
