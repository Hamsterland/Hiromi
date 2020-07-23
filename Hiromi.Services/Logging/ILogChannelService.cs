using System.Threading.Tasks;
using Hiromi.Data.Models.Logging;

namespace Hiromi.Services.Logging
{
    public interface ILogChannelService
    {
        Task SetLogChannelAsync(ulong guildId, ulong channelId);
        Task RemoveLogChannelAsync(ulong guildId);
        Task<LogChannelSummary> GetLogChannelSummary(ulong guildId);
    }
}