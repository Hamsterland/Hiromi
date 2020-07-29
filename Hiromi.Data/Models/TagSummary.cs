using System;
using System.Linq.Expressions;
using Discord;

namespace Hiromi.Data.Models.Tags
{
    public class TagSummary
    {
        public ulong AuthorId { get; set; }
        public ulong OwnerId { get; set; }
        public ulong GuildId { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public int Uses { get; set; }
        public DateTime TimeCreated { get; set; }

        public bool CanMaintain(IGuildUser user)
        {
            return OwnerId == user.Id || user.GuildPermissions.ManageMessages;
        }
        
        public static readonly Expression<Func<Tag, TagSummary>> FromEntityProjection = tagEntity => new TagSummary
        {
            AuthorId = tagEntity.AuthorId,
            OwnerId = tagEntity.OwnerId,
            GuildId = tagEntity.GuildId,
            Name = tagEntity.Name,
            Content = tagEntity.Content,
            Uses = tagEntity.Uses,
        };
    }
}