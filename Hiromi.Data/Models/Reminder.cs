using System;
using System.ComponentModel.DataAnnotations;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hiromi.Data.Models
{
    public class Reminder
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public ulong UserId { get; set; }

        [Required]
        public ulong GuildId { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public TimeSpan Duration { get; set; }
        
        [Required] 
        public bool IsCompleted { get; set; }

        [Required]
        public bool IsSuccess { get; set; }
    }

    public class ReminderConfiguration : IEntityTypeConfiguration<Reminder>
    {
        public void Configure(EntityTypeBuilder<Reminder> builder)
        {
            builder
                .Property(x => x.UserId)
                .HasConversion<long>();

            builder
                .Property(x => x.GuildId)
                .HasConversion<long>();
        }
    }
}