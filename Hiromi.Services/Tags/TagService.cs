using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
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
                var tags = await _hiromiContext
                    .Tags
                    .FromSqlRaw("SELECT * FROM \"Tags\" WHERE SIMILARITY(\"Name\", {0}) > 0.1", name)
                    .Select(TagSummary.FromEntityProjection)
                    .ToListAsync();

                var builder = new StringBuilder()
                    .AppendLine($"No tag called \"{name}\" found. Did you mean?")
                    .AppendLine()
                    .AppendLine("```");

                foreach (var t in tags)
                {
                    builder.AppendLine(t.Name);
                }

                builder.AppendLine("```");
                
                await channel.SendMessageAsync(builder.ToString());
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