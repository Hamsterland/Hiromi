using System.Threading.Tasks;
using Hiromi.Data;
using Hiromi.Data.Models.Logging;
using Microsoft.EntityFrameworkCore;

namespace Hiromi.Services.Logging
{
    public class LogChannelService : ILogChannelService
    {
        private readonly HiromiContext _hiromiContext;

        public LogChannelService(HiromiContext hiromiContext)
        {
            _hiromiContext = hiromiContext;
        }

        public async Task SetLogChannel(ulong guildId, ulong channelId)
        {
            var logChannel = await _hiromiContext
                .LogChannels
                .FirstOrDefaultAsync(x => x.GuildId == guildId);
            
            if (logChannel is null)
            {
                _hiromiContext.Add(new LogChannel
                {
                    GuildId = guildId,
                    ChannelId = channelId
                });
            }
            else
            {
                logChannel.ChannelId = channelId;
            }
            
            await _hiromiContext.SaveChangesAsync();
        }

        public async Task RemoveLogChannel(ulong guildId)
        {
            var logChannel = await _hiromiContext
                .LogChannels
                .FirstOrDefaultAsync(x => x.GuildId == guildId);

            if (logChannel != null)
                _hiromiContext.Remove(logChannel);

            await _hiromiContext.SaveChangesAsync();
        }
    }
}