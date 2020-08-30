using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hiromi.Data.Models
{
    public class Message
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public ulong MessageId { get; set; }
        
        [Required]
        public ulong UserId { get; set; }
        
        [Required]
        public ulong ChannelId { get; set; }
        
        [Required]
        public ulong GuildId { get; set; }

        [Required] 
        public DateTime TimeSent { get; } = DateTime.Now;
    }

    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder
                .HasIndex(x => x.MessageId)
                .IsUnique();
            
            builder
                .Property(x => x.MessageId)
                .HasConversion<long>();

            builder
                .Property(x => x.UserId)
                .HasConversion<long>();

            builder
                .Property(x => x.ChannelId)
                .HasConversion<long>();

            builder
                .Property(x => x.GuildId)
                .HasConversion<long>();
        }
    }
}