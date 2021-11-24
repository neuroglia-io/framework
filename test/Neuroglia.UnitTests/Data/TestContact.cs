using Microsoft.EntityFrameworkCore;
using Neuroglia.Data;
using System;

namespace Neuroglia.UnitTests.Data
{

    public class TestContact
        : Entity<Guid>
    {

        public virtual string Tel { get; set; }

    }

}
