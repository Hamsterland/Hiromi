using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Hiromi.Data.Models;

namespace Hiromi.Services.Stats
{
    public interface IGuildStatsService
    {
        Task<int> GetMessageCountAsync(TimeSpan span, Expression<Func<Message, bool>> criteria);
        Task<IReadOnlyDictionary<ulong, int>> GetMostMessageCountByChannelAsync(ulong guildId, TimeSpan span);
    }
}