using Hiromi.Data.Models.Logging;
using Hiromi.Data.Models.Tags;
using Microsoft.EntityFrameworkCore;

namespace Hiromi.Data
{
    public class HiromiContext : DbContext
    {
        public DbSet<TagEntity> Tags { get; set; }

        public DbSet<LogChannel> LogChannels { get; set; }
        
        public HiromiContext(DbContextOptions options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(HiromiContext).Assembly);
        }
    }
}