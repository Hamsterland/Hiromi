using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Hiromi.Data.Models;

namespace Hiromi.Services.Stats
{
    public interface IGuildStatsService
    {
        Task<int> GetMessagesCount(Expression<Func<Message, bool>> criteria, TimeSpan span);
    }
}