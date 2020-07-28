using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;

namespace Hiromi.Services.Commands
{
    public class CommandToggleService : ICommandToggleService
    {
        private readonly ICommandStoreService _commandStoreService;

        public CommandToggleService(ICommandStoreService commandStoreService)
        {
            _commandStoreService = commandStoreService;
        }

        public async Task EnableCommandAsync(ulong channelId, CommandInfo command)
        {
            _commandStoreService.CacheCommands(channelId, new List<string> {command.Name});
            await _commandStoreService.StoreCommandsInDbAsync(channelId, new List<string> {command.Name});
        }

        public async Task DisableCommandAsync(ulong channelId, CommandInfo command)
        {
            var commands = _commandStoreService.GetEnabledCommands(channelId);

            if (commands.Contains(command.Name))
            {
                commands.Remove(command.Name);
                _commandStoreService.CacheCommands(channelId, commands);
                await _commandStoreService.StoreCommandsInDbAsync(channelId, commands);
            }
        }

        public async Task EnableModuleAsync(ulong channelId, ModuleInfo module)
        {
            var commands = module
                .Commands
                .Select(command => command.Name)
                .ToList();
            
            _commandStoreService.CacheCommands(channelId, commands);
            await _commandStoreService.StoreCommandsInDbAsync(channelId, commands);
        }
        
        public async Task DisableModuleAsync(ulong channelId, ModuleInfo module)
        {
            var commands = module
                .Commands
                .Select(x => x.Name)
                .ToList();
            
            if (module.Commands.Count > 0)
            {
                _commandStoreService.UnCacheCommands(channelId, commands);
                await _commandStoreService.RemoveCommandsFromDbAsync(channelId, commands);
            }
        }
    }
}