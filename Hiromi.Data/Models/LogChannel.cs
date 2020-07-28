using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hiromi.Data.Models.Logging
{
    public class LogChannel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public ulong GuildId { get; set; }

        [Required]
        public ulong ChannelId { get; set; }
    }
    
    public class LogChannelConfigurator : IEntityTypeConfiguration<LogChannel>
    {
        public void Configure(EntityTypeBuilder<LogChannel> builder)
        {
            builder
                .Property(x => x.GuildId)
                .HasConversion<long>();

            builder
                .HasIndex(x => x.GuildId)
                .IsUnique();

            builder
                .Property(x => x.ChannelId)
                .HasConversion<long>();
            
            builder
                .HasIndex(x => x.ChannelId)
                .IsUnique();
        }
    }
}