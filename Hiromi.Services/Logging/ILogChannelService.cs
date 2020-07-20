using System.Threading.Tasks;

namespace Hiromi.Services.Logging
{
    public interface ILogChannelService
    {
        Task SetLogChannel(ulong guildId, ulong channelId);
        Task RemoveLogChannel(ulong guildId);
    }
}