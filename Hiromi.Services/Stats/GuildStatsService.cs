using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Hiromi.Data;
using Hiromi.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Hiromi.Services.Stats
{
    public class GuildStatsService : IGuildStatsService
    {
        private readonly HiromiContext _hiromiContext;

        public GuildStatsService(HiromiContext hiromiContext)
        {
            _hiromiContext = hiromiContext;
        }

        public async Task<int> GetMessageCountAsync(TimeSpan span, Expression<Func<Message, bool>> criteria)
        {
            var messages = await _hiromiContext
                .Messages
                .Where(criteria)
                .ToListAsync();

            return messages.Count(x => x.TimeSent >= DateTime.Now.Date.Subtract(span));
        }

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