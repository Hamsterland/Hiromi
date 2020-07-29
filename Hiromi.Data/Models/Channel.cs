using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hiromi.Data.Models.Channels
{
    public class Channel
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public ulong GuildId { get; set; }
        
        [Required]
        public ulong ChannelId { get; set; }
        
        [Required]
        public List<string> Commands { get; set; } = new List<string>();

        [Required] 
        public bool IsLogChannel { get; set; }
    }

    public class ChannelConfiguration : IEntityTypeConfiguration<Channel>
    {
        public void Configure(EntityTypeBuilder<Channel> builder)
        {
            builder
                .Property(x => x.ChannelId)
                .HasConversion<long>();

            builder
                .HasIndex(x => x.ChannelId)
                .IsUnique();
        }
    }
}