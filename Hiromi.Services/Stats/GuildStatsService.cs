using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hiromi.Data;
using Hiromi.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Hiromi.Services.Stats
{
    /// <inheritdoc/> 
    public class GuildStatsService : IGuildStatsService
    {
        private readonly HiromiContext _hiromiContext;

        public GuildStatsService(HiromiContext hiromiContext)
        {
            _hiromiContext = hiromiContext;
        }
        
        /// <inheritdoc/>
        public async Task<int> GetMessageCountAsync(StatisticsSource source, TimeSpan span, ulong guildId, ulong userId = 0)
        {
            var messages = new List<MessageSummary>();
            
            switch (source)
            {
                case StatisticsSource.Guild:
                {
                    messages = await _hiromiContext
                        .Messages
                        .AsNoTracking()
                        .Where(x => x.GuildId == guildId)
                        .Select(MessageSummary.FromEntityProjection)
                        .ToListAsync();

                    break;
                }
                case StatisticsSource.User:
                {
                    messages = await _hiromiContext
                        .Messages
                        .AsNoTracking()
                        .Where(x => x.GuildId == guildId)
                        .Where(x => x.UserId == userId)
                        .Select(MessageSummary.FromEntityProjection)
                        .ToListAsync();

                    break;
                }
            }
         
            var earliest = DateTime.Now.Subtract(span);
            return messages.Count(x => x.TimeSent >= earliest);
        }

        /// <inheritdoc/> 
        public async Task<(ulong, int)> GetMostActiveChannelAndMessageCountAsync(StatisticsSource source, TimeSpan span, ulong guildId, ulong userId = 0)
        {
            var messages = new List<MessageSummary>();
            
            switch (source)
            {
                case StatisticsSource.Guild:
                {
                    messages = await _hiromiContext
                        .Messages
                        .AsNoTracking()
                        .Where(x => x.GuildId == guildId)
                        .Select(MessageSummary.FromEntityProjection)
                        .ToListAsync();

                    break;
                }
                case StatisticsSource.User:
                {
                    messages = await _hiromiContext
                        .Messages
                        .AsNoTracking()
                        .Where(x => x.GuildId == guildId)
                        .Where(x => x.UserId == userId)
                        .Select(MessageSummary.FromEntityProjection)
                        .ToListAsync();

                    break;
                }
            }

            var earliest = DateTime.Now.Subtract(span);
            var filtered = messages.Where(x => x.TimeSent > earliest);
            
            var final = new List<ulong>();
            final.AddRange(filtered.Select(x => x.ChannelId));

            var (channelId, count) = final
                .GroupBy(x => x)
                .ToDictionary(x => x.Key, x => x.Count())
                .OrderByDescending(x => x.Value)
                .FirstOrDefault();

            return (channelId, count);
        }

        /// <inheritdoc/> 
        public async Task<MessageSummary> GetSecondLastMessageFromUserAsync(ulong guildId, ulong userId)
        {
            var messages = await _hiromiContext
                .Messages
                .AsNoTracking()
                .Where(x => x.GuildId == guildId)
                .Where(x => x.UserId == userId)
                .Select(MessageSummary.FromEntityProjection)
                .ToListAsync();

            return messages[^1];
        }
    }
}