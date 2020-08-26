using System;
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

        public async Task<int> GetMessagesCount(Expression<Func<Message, bool>> criteria, TimeSpan span)
        {
            var messages = await _hiromiContext
                .Messages
                .Where(criteria)
                .ToListAsync();

            var filtered = messages
                .Where(x => x.TimeSent >= DateTime.Now.Date.Subtract(span))
                .ToList();
            
            return filtered.Count;
        }
    }
}