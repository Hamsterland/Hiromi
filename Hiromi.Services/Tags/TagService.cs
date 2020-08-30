using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Hiromi.Data;
using Hiromi.Data.Models;
using Hiromi.Data.Models.Tags;
using Hiromi.Services.Tags.Exceptions;
using Humanizer;
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

        public async Task ModifyAllowTagsAsync(ulong guildId, bool allow)
        {
            var guild = await _hiromiContext
                .Guilds
                .Where(x => x.GuildId == guildId)
                .FirstOrDefaultAsync();

            if (guild is null)
            {
                _hiromiContext.Add(new Guild
                {
                    GuildId = guildId,
                    AllowTags = allow,
                    AllowQuotes = true
                });
            }
            else
            {
                guild.AllowTags = allow;
            }
            
            await _hiromiContext.SaveChangesAsync();
        }
        
        public async Task InvokeTagAsync(ulong guildId, ulong channelId, string name)
        {
            var tag = await _hiromiContext
                .Tags
                .Where(x => x.GuildId == guildId)
                .Where(x => x.Name == name)
                .FirstOrDefaultAsync();
            
            var channel = _discordSocketClient.GetChannel(channelId) as IMessageChannel;
            
            if (tag is null)
            {
                var tags = await GetTagSummaryMatches(guildId, name);
                var embed = TagViews.FormatSimilarTags(name, tags);
                await channel.SendMessageAsync(embed: embed);
                return;
            }
            
            await channel.SendMessageAsync(tag.Content, allowedMentions: AllowedMentions.None);

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
            {
                throw new TagAlreadyExistsException(default);
            }

            _hiromiContext.Add(new Tag
            {
                GuildId = guildId,
                AuthorId = authorId,
                OwnerId = authorId,
                Name = name,
                Content = content
            });
            
            await _hiromiContext.SaveChangesAsync();
        }

        public async Task ModifyTagAsync(ulong guildId, string name, Action<Tag> action)
        {
            var tag = await _hiromiContext
                .Tags
                .Where(x => x.GuildId == guildId)
                .Where(x => x.Name == name)
                .FirstOrDefaultAsync();
            
            action(tag);
            await _hiromiContext.SaveChangesAsync();
        }

        public async Task DeleteTagAsync(ulong guildId, string name)
        {
            var tag = await _hiromiContext
                .Tags
                .Where(x => x.GuildId == guildId)
                .Where(x => x.Name == name)
                .FirstOrDefaultAsync();

            _hiromiContext.Remove(tag);
            await _hiromiContext.SaveChangesAsync();
        }

        public async Task<TagSummary> GetTagSummaryAsync(ulong guildId, string name)
        {
            return await _hiromiContext
                .Tags
                .Where(x => x.GuildId == guildId)
                .Where(x => x.Name == name)
                .Select(TagSummary.FromEntityProjection)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TagSummary>> GetTagSummaries(ulong guildId, Expression<Func<Tag, bool>> criteria)
        {
            return await _hiromiContext
                .Tags
                .Where(x => x.GuildId == guildId)
                .Where(criteria)
                .Select(TagSummary.FromEntityProjection)
                .ToListAsync();
        }

        public async Task<IEnumerable<TagSummary>> GetTagSummaryMatches(ulong guildId, string name)
        {
            return await _hiromiContext
                .Tags
                .FromSqlRaw("SELECT * FROM \"Tags\" WHERE SIMILARITY(\"Name\", {0}) > 0.1 AND \"GuildId\" = {1}", name, (long) guildId)
                .Select(TagSummary.FromEntityProjection)
                .ToListAsync();
        }

        public async Task<bool> HasMaintenancePermissions(string name, IGuildUser user)
        {
            var tag = await _hiromiContext
                .Tags
                .Where(x => x.GuildId == user.GuildId)
                .Where(x => x.Name == name)
                .FirstOrDefaultAsync();
            
            return tag.OwnerId == user.Id || user.GuildPermissions.ManageMessages;
        }
    }
}