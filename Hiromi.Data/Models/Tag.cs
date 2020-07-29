using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hiromi.Data.Models.Tags
{
    public class Tag
    {
        [Key]
        [Required]
        public long Id { get; set; }

        [Required]
        public ulong AuthorId { get; set; }

        [Required]
        public ulong OwnerId { get; set; }
        
        [Required] 
        public ulong GuildId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Content { get; set; }
        
        [Required]
        public int Uses { get; set; }

        [Required]
        public DateTime TimeCreated { get; set; } = DateTime.Now;
    }
    
    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder
                .HasAlternateKey(x => new {x.GuildId, x.Name});
            
            builder
                .Property(x => x.AuthorId)
                .HasConversion<long>();

            builder
                .Property(x => x.OwnerId)
                .HasConversion<long>();

            builder
                .Property(x => x.GuildId)
                .HasConversion<long>();
        }
    }
}