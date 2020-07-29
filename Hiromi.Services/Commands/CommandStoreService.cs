

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hiromi.Data;
using Hiromi.Data.Models.Channels;
using Microsoft.EntityFrameworkCore;

namespace Hiromi.Services.Commands
{
    public class CommandStoreService : ICommandStoreService
    {
        private readonly HiromiContext _hiromiContext;

        public CommandStoreService(HiromiContext hiromiContext)
        {
            _hiromiContext = hiromiContext;
        }
        
        private readonly ConcurrentDictionary<ulong, List<string>> _enabledCommands = new ConcurrentDictionary<ulong, List<string>>();

        public Task LoadEnabledCommands()
        {
            var channels = _hiromiContext.Channels;

            foreach (var channel in channels)
            {
                CacheCommands(channel.ChannelId, channel.Commands);
            }

            return Task.CompletedTask;
        }
        
        public void CacheCommands(ulong channelId, List<string> commands)
        {
            if (_enabledCommands.ContainsKey(channelId))
            {
                var current = GetEnabledCommands(channelId);
                var updated = current.Union(commands).ToList();
                _enabledCommands.TryUpdate(channelId, updated, current);
            }
            else
            {
                _enabledCommands.TryAdd(channelId, commands);
            }
        }

        public void UnCacheCommands(ulong channelId, IEnumerable<string> commands)
        {
            if (_enabledCommands.ContainsKey(channelId))
            {
                var current = GetEnabledCommands(channelId);
                var updated = current.Except(commands).ToList();
                _enabledCommands.TryUpdate(channelId, updated, current);
            }
        }

        public List<string> GetEnabledCommands(ulong channelId)
        {
            return !_enabledCommands.ContainsKey(channelId) ? null : _enabledCommands[channelId];
        }

        public async Task StoreCommandsInDbAsync(ulong guildId, ulong channelId, IEnumerable<string> commands)
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
                    IsLogChannel = false
                });
            }
            
            var current = await _hiromiContext
                .Channels
                .Where(x => x.GuildId == guildId)
                .Where(x => x.ChannelId == channelId)
                .FirstOrDefaultAsync();
            
            foreach (var command in commands)
            {
                if (current.Commands.Contains(command))
                {
                    continue;
                }
                
                current.Commands.Add(command);
            }
            
            await _hiromiContext.SaveChangesAsync();
        }

        public async Task RemoveCommandsFromDbAsync(ulong guildId, ulong channelId, IEnumerable<string> commands)
        {
            var channel = await _hiromiContext
                .Channels
                .Where(x => x.GuildId == guildId)
                .Where(x => x.ChannelId == channelId)
                .FirstOrDefaultAsync();

            if (channel is null)
            {
                return;
            }

            foreach (var command in commands)
            {
                if (channel.Commands.Contains(command))
                {
                    channel.Commands.Remove(command);
                }
            }

            await _hiromiContext.SaveChangesAsync();
        }
    }
}