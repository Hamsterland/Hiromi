using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hiromi.Data.Models
{
    public class WelcomeResponse
    {
        [Key]
        [Required]
        public int Id { get; set; }
        
        [Required]
        public ulong ChannelId { get; set; }
        
        [Required]
        public ulong GuildId { get; set; }
        
        [Required]
        public string Message { get; set; }
    }

    public class WelcomeResponseConfiguration : IEntityTypeConfiguration<WelcomeResponse>
    {
        public void Configure(EntityTypeBuilder<WelcomeResponse> builder)
        {
            builder
                .Property(x => x.ChannelId)
                .HasConversion<long>();

            builder
                .Property(x => x.GuildId)
                .HasConversion<long>();

            builder
                .HasIndex(x => x.ChannelId)
                .IsUnique();

            builder
                .HasIndex(x => x.GuildId)
                .IsUnique();
        }
    }
}