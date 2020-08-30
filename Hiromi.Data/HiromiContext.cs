using Hiromi.Data.Models;
using Hiromi.Data.Models.Tags;
using Microsoft.EntityFrameworkCore;
using Channel = Hiromi.Data.Models.Channels.Channel;

namespace Hiromi.Data
{
    public class HiromiContext : DbContext
    {
        public DbSet<Tag> Tags { get; set; }
        
        public DbSet<Channel> Channels { get; set; }
        
        public DbSet<Reminder> Reminders { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<Guild> Guilds { get; set; }
        
        public HiromiContext(DbContextOptions options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(HiromiContext).Assembly);
        }
    }
}