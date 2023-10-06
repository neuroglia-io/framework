using Neuroglia.Data;

namespace Neuroglia.UnitTests.Data;

public class PersonTrait
    : Entity<Guid>
{

    public PersonTrait() : base(Guid.NewGuid()) { }

    public virtual string Name { get; set; } = null!;

    public virtual decimal Value { get; set; }

}
