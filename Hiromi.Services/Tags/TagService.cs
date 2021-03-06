﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Hiromi.Data;
using Hiromi.Data.Models;
using Hiromi.Data.Models.Channels;
using Hiromi.Data.Models.Tags;
using Hiromi.Services.Tags.Exceptions;
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
            
            var messageChannel = _discordSocketClient.GetChannel(channelId) as IMessageChannel;
            
            // Check if the channel is recorded 
            var channel = await _hiromiContext
                .Channels
                .Where(x => x.GuildId == guildId)
                .Where(x => x.ChannelId == channelId)
                .FirstOrDefaultAsync();
            
            if (channel is null)
            {
                channel = new Channel
                {
                    GuildId = guildId,
                    ChannelId = channelId,
                    AllowTags = true,
                    IsLogChannel = false
                };
                
                _hiromiContext.Add(channel);
                await _hiromiContext.SaveChangesAsync();
            }

            if (!channel.AllowTags)
            {
                return; 
            }
            
            // If the Tag is not found, show similar matches
            if (tag is null)
            {
                var tags = await GetTagSummaryMatches(guildId, name);
                var embed = TagViews.FormatSimilarTags(name, tags);
                await messageChannel.SendMessageAsync(embed: embed);
                return;
            }
            
            
            
            await messageChannel.SendMessageAsync(tag.Content, allowedMentions: AllowedMentions.None);

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
                throw new TagAlreadyExistsException();
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
        
        public async Task ModifyAllowTagsAsync(ulong guildId, ulong channelId, bool allowTags)
        {
            var channel = await _hiromiContext
                .Channels
                .Where(x => x.GuildId == guildId)
                .Where(x => x.ChannelId == channelId)
                .FirstOrDefaultAsync();

            if (channel is null)
            {
                channel = new Channel
                {
                    GuildId = guildId,
                    ChannelId = channelId,
                    AllowTags = true,
                    IsLogChannel = false
                };
                
                _hiromiContext.Add(channel);
            }

            channel.AllowTags = allowTags;
            await _hiromiContext.SaveChangesAsync();
        }
    }
}