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
                        .Where(x => x.GuildId == guildId)
                        .Select(MessageSummary.FromEntityProjection)
                        .ToListAsync();

                    break;
                }
                case StatisticsSource.User:
                {
                    messages = await _hiromiContext
                        .Messages
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
        public async Task<IReadOnlyDictionary<ulong, int>> GetMostMessageCountByChannelAsync(TimeSpan span, ulong guildId)
        {
            var messages = await _hiromiContext
                .Messages
                .Where(x => x.GuildId == guildId)
                .ToListAsync();

            var filtered = messages
                .Where(x => x.TimeSent >= DateTime.Now.Date.Subtract(span))
                .Select(x => x.ChannelId);
            
            return filtered
                .GroupBy(x => x)
                .ToDictionary(x => x.Key, x => x.Count());
        }
    }
}