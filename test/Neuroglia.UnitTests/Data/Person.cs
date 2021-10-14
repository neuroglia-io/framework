using Neuroglia.Data;

namespace Neuroglia.UnitTests.Data
{

    [ODataEntity("Persons")]
    public class Person
    {

        public int ID { get; set; }

        public string Name { get; set; }

        public PersonDetails PersonDetail { get; set; }

    }

}
