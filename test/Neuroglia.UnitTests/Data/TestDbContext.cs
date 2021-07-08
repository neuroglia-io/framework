using Microsoft.EntityFrameworkCore;

namespace Neuroglia.UnitTests.Data
{

    public class TestDbContext
        : DbContext
    {

        public TestDbContext(DbContextOptions<TestDbContext> options) 
            : base(options)
        {

        }

        public DbSet<TestPerson> Persons { get; protected set; }

    }

}
