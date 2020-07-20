using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Hiromi.Data
{
    public class HiromiContextFactory : IDesignTimeDbContextFactory<HiromiContext>
    {
        public HiromiContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<HiromiContext>()
                .Build();
            
            var optionsBuilder = new DbContextOptionsBuilder()
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
                .UseNpgsql(configuration["Postgres:Connection"]);
            
            return new HiromiContext(optionsBuilder.Options);
        }
    }
}