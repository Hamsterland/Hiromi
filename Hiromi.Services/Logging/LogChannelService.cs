using System;
using System.Linq;
using System.Linq.Expressions;
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

        public async Task SetLogChannelAsync(ulong guildId, ulong channelId)
        {
            var logChannel = await _hiromiContext
                .LogChannels
                .Where(x => x.GuildId == guildId)
                .FirstOrDefaultAsync();
            
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

        public async Task RemoveLogChannelAsync(ulong guildId)
        {
            var logChannel = _hiromiContext
                .LogChannels
                .Where(x => x.GuildId == guildId)
                .FirstOrDefaultAsync();

            if (logChannel != null)
            {
                _hiromiContext.Remove(logChannel);
            }

            await _hiromiContext.SaveChangesAsync();
        }

        public async Task<LogChannelSummary> GetLogChannelSummary(ulong guildId)
        {
            return await _hiromiContext
                .LogChannels
                .Where(x => x.GuildId == guildId)
                .Select(LogChannelSummary.FromEntityProjection)
                .FirstOrDefaultAsync();
        }
    }
}