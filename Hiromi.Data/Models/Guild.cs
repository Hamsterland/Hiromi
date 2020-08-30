using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hiromi.Data.Models
{
    public class Guild
    {
        [Key]
        [Required]
        public int Id { get; set; }
        
        [Required]
        public ulong GuildId { get; set; }
        
        [Required]
        public bool AllowTags { get; set; }
        
        [Required]
        public bool AllowQuotes { get; set; }
    }
    
    public class GuildConfiguration : IEntityTypeConfiguration<Guild>
    {
        public void Configure(EntityTypeBuilder<Guild> builder)
        {
            builder
                .HasIndex(x => x.GuildId)
                .IsUnique();
            
            builder
                .Property(x => x.GuildId)
                .HasConversion<long>();
        }
    }
}