using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace Mouts.SalesRecords.Infra.Database
{
    public class SalesContextFactory : IDesignTimeDbContextFactory<SalesContext>
    {
        public SalesContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SalesContext>();

            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=salesrecord;Username=postgres;Password=pass123");

            return new SalesContext(optionsBuilder.Options);
        }
    }
}
