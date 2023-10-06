using Neuroglia.Data;

namespace Neuroglia.UnitTests.Data;

public class Contact
    : Entity<Guid>
{

    public virtual string Tel { get; set; } = null!;

}
