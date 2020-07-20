using System;
using System.Linq.Expressions;

namespace Hiromi.Data.Models.Tags
{
    public class TagSummary
    {
        public long Id { get; set; }
        public ulong AuthorId { get; set; }
        public ulong OwnerId { get; set; }
        public ulong GuildId { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public int Uses { get; set; }
        
        public static readonly Expression<Func<TagEntity, TagSummary>> FromEntityProjection = tagEntity => new TagSummary
        {
            Id = tagEntity.Id,
            AuthorId = tagEntity.AuthorId,
            OwnerId = tagEntity.OwnerId,
            GuildId = tagEntity.GuildId,
            Name = tagEntity.Name,
            Content = tagEntity.Content,
            Uses = tagEntity.Uses,
        };
    }
}