using System.Linq;
using System.Threading.Tasks;
using Hiromi.Data;
using Hiromi.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Hiromi.Services.WelcomeResponses
{
    public class WelcomeResponseService
    {
        private readonly HiromiContext _hiromiContext;

        public WelcomeResponseService(HiromiContext hiromiContext)
        {
            _hiromiContext = hiromiContext;
        }

        public async Task SetWelcomeChannel(ulong channelId, ulong guildId)
        {
            var current = await _hiromiContext
                .WelcomeResponses
                .Where(x => x.GuildId == guildId)
                .FirstOrDefaultAsync();

            if (current is null)
            {
                _hiromiContext.Add(new WelcomeResponse
                {
                    ChannelId = channelId,
                    GuildId = guildId
                });
            }
            else
            {
                current.ChannelId = channelId;
            }

            await _hiromiContext.SaveChangesAsync();
        }

        public async Task SetWelcomeMessage(ulong channelId, ulong guildId, string message)
        {
            var current = await _hiromiContext
                .WelcomeResponses
                .Where(x => x.GuildId == guildId)
                .Where(x => x.ChannelId == channelId)
                .FirstOrDefaultAsync();

            if (current is null is false)
            {
                current.Message = message;
            }
            else
            {
                await SetWelcomeChannel(channelId, guildId);
                
                var added = await _hiromiContext
                    .WelcomeResponses
                    .Where(x => x.GuildId == guildId)
                    .Where(x => x.ChannelId == channelId)
                    .FirstOrDefaultAsync();

                added.Message = message;
            }

            await _hiromiContext.SaveChangesAsync();
        }
    }
}