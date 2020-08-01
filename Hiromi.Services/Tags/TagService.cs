using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Interactive;
using Discord.WebSocket;
using Hiromi.Data;
using Hiromi.Data.Models.Tags;
using Hiromi.Services.Tags.Exceptions;
using Microsoft.EntityFrameworkCore;
using MoreLinq;

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
                var tags = await GetTagSummaryMatches(guildId, name);
                var embed = FormatMatchedTags(name, tags);
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

        public async Task<bool> CanMaintain(string name, IGuildUser user)
        {
            var tag = await _hiromiContext
                .Tags
                .Where(x => x.GuildId == user.GuildId)
                .Where(x => x.Name == name)
                .FirstOrDefaultAsync();
            
            return tag.OwnerId == user.Id || user.GuildPermissions.ManageMessages;
        }
        
        public Embed FormatMatchedTags(string name, IEnumerable<TagSummary> matches)
        {
            var embed = new EmbedBuilder().WithColor(Constants.DefaultEmbedColour);
            
            var tags = matches.ToList();
            if (tags.Count > 0)
            {
                var builder = new StringBuilder()
                    .AppendLine($"No tag called \"{name}\" found. Did you mean?")
                    .AppendLine("```");
                
                foreach (var match in tags)
                {
                    builder.AppendLine(match.Name);
                }

                builder.AppendLine("```");
                embed.WithDescription(builder.ToString());
            }
            else
            {
                embed.WithDescription($"No tag called \"{name}\"");
            }

            return embed.Build();
        }

        public Embed FormatTagInfo(IUser author, IUser owner, TagSummary tag)
        {
            return new EmbedBuilder()
                .WithColor(Constants.DefaultEmbedColour)
                .AddField("Name", tag.Name)
                .AddField("Uses", tag.Uses)
                .AddField("Owner", owner)
                .AddField("Author", author)
                .Build();
        }

        public PaginatedMessage FormatUserTags(IUser user, IEnumerable<TagSummary> tags)
        {
            var fields = tags
                .Select(tagSummary => new EmbedFieldBuilder()
                    .WithValue(tagSummary.Name)
                    .WithIsInline(true));

            var pages = new List<EmbedPage>(fields
                .Batch(10)
                .Select(x => new EmbedPage
                {
                    Author = new EmbedAuthorBuilder()
                        .WithName($"{user}'s Tags")
                        .WithIconUrl(user.GetAvatarUrl()),

                    TotalFieldMessage = "Tags",
                    Fields = x.ToList(),
                    Color = Constants.DefaultEmbedColour
                }));

            return new PaginatedMessage
            {
                Pages = pages,
                Options = new PaginatedAppearanceOptions { Timeout = TimeSpan.FromMinutes(1) }
            };
        }

        public PaginatedMessage FormatGuildTags(IGuild guild, IEnumerable<TagSummary> tags)
        {
            var fields = tags
                .Select(tagSummary => new EmbedFieldBuilder()
                    .WithValue(tagSummary.Name)
                    .WithIsInline(true));
            
            var pages = new List<EmbedPage>(fields
                .Batch(10)
                .Select(x => new EmbedPage
                {
                    Author = new EmbedAuthorBuilder()
                        .WithName($"{guild} Tags")
                        .WithIconUrl(guild.IconUrl),

                    TotalFieldMessage = "Tags",
                    Fields = x.ToList(),
                    Color = Constants.DefaultEmbedColour
                }));
            
            return new PaginatedMessage
            {
                Pages = pages,
                Options = new PaginatedAppearanceOptions { Timeout = TimeSpan.FromMinutes(1) }
            };
        }
    }
}