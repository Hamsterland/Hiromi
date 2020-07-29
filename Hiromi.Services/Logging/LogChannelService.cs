using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hiromi.Data;
using Hiromi.Data.Models;
using Hiromi.Data.Models.Channels;
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
            var channel = await _hiromiContext
                .Channels
                .Where(x => x.GuildId == guildId)
                .Where(x => x.ChannelId == channelId)
                .FirstOrDefaultAsync();
            
            if (channel is null)
            {
                _hiromiContext.Add(new Channel
                {
                    GuildId = guildId,
                    ChannelId = channelId,
                    Commands = new List<string>(),
                    IsLogChannel = true
                });
            }
            else if (channel.IsLogChannel)
            {
                return;
            }
            else
            {
                channel.IsLogChannel = true;
            }
            
            await _hiromiContext.SaveChangesAsync();
        }

        public async Task RemoveLogChannelAsync(ulong guildId)
        {
            var channel = await _hiromiContext
                .Channels
                .Where(x => x.GuildId == guildId)
                .FirstOrDefaultAsync();

            if (channel is null)
            {
                return;
            }

            channel.IsLogChannel = false;
            await _hiromiContext.SaveChangesAsync();
        }

        public async Task<ChannelSummary> GetLogChannelSummary(ulong guildId)
        {
            return await _hiromiContext
                .Channels
                .Where(x => x.GuildId == guildId)
                .Where(x => x.IsLogChannel)
                .Select(ChannelSummary.FromEntityProjection)
                .FirstOrDefaultAsync();
        }
    }
}