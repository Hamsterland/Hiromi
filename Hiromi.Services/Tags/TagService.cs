using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Hiromi.Data;
using Hiromi.Data.Models.Tags;
using Microsoft.EntityFrameworkCore;

namespace Hiromi.Services.Tags
{
    public class TagService : ITagService
    {
        private readonly HiromiContext _hiromiContext;
        private readonly DiscordSocketClient _discordSocketClient;

        public TagService(
            HiromiContext hiromiContext,
            DiscordSocketClient discordSocketClient)
        {
            _hiromiContext = hiromiContext;
            _discordSocketClient = discordSocketClient;
        }

        public async Task InvokeTagAsync(ulong guildId, ulong channelId, Expression<Func<TagEntity, bool>> criteria)
        {
            var tag = await _hiromiContext
                .Tags
                .Where(x => x.GuildId == guildId)
                .Where(criteria)
                .FirstOrDefaultAsync();

            if (tag is null)
                return;

            var channel = _discordSocketClient.GetChannel(channelId);
            await (channel as IMessageChannel).SendMessageAsync(tag.Content);

            tag.Uses++;
            await _hiromiContext.SaveChangesAsync();
        }

        public async Task CreateTagAsync(ulong guildId, ulong authorId, string name, string content)
        {
            var tag = await _hiromiContext
                .Tags
                .Where(x => x.GuildId == guildId)
                .Where(x => x.Name == name)
                .FirstOrDefaultAsync();

            if (tag != null)
                throw new Exception("Tag already exists");

            _hiromiContext.Add(new TagEntity
            {
                GuildId = guildId,
                AuthorId = authorId,
                OwnerId = authorId,
                Name = name,
                Content = content
            });

            await _hiromiContext.SaveChangesAsync();
        }

        public async Task ModifyTagAsync(ulong guildId, Expression<Func<TagEntity, bool>> criteria, Action<TagEntity> action)
        {
            var tag = await _hiromiContext
                .Tags
                .Where(x => x.GuildId == guildId)
                .Where(criteria)
                .FirstOrDefaultAsync();

            if (tag == null)
                return;

            action(tag);
            await _hiromiContext.SaveChangesAsync();
        }

        public async Task DeleteTagAsync(ulong guildId, Expression<Func<TagEntity, bool>> criteria)
        {
            var tag = await _hiromiContext
                .Tags
                .Where(x => x.GuildId == guildId)
                .Where(criteria)
                .FirstOrDefaultAsync();

            if (tag == null)
                return;

            _hiromiContext.Remove(tag);
            await _hiromiContext.SaveChangesAsync();
        }

        public async Task<TagSummary> GetTagSummary(ulong guildId, Expression<Func<TagEntity, bool>> criteria)
        {
            var tag = await _hiromiContext
                .Tags
                .Where(x => x.GuildId == guildId)
                .Where(criteria)
                .Select(TagSummary.FromEntityProjection)
                .FirstOrDefaultAsync();

            if (tag is null)
                throw new Exception("Tag not found");

            return tag;
        }

        public async Task<IEnumerable<TagSummary>> GetTagSummaries(ulong guildId, Expression<Func<TagEntity, bool>> criteria)
        {
            var tags = await _hiromiContext
                .Tags
                .Where(x => x.GuildId == guildId)
                .Where(criteria)
                .Select(TagSummary.FromEntityProjection)
                .ToListAsync();

            if (tags.Count < 0)
                throw new Exception("No Tags found"); 

            return tags;
        }

        public bool CanMaintain(IGuildUser user, TagSummary tagSummary)
        {
            return tagSummary.AuthorId == user.Id || user.GuildPermissions.ManageMessages;
        }
    }
}