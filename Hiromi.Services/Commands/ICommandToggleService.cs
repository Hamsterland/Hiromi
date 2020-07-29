using System.Threading.Tasks;
using Discord.Commands;

namespace Hiromi.Services.Commands
{
    public interface ICommandToggleService
    {
        Task EnableCommandAsync(ulong guildId, ulong channelId, CommandInfo command);
        Task DisableCommandAsync(ulong guildId, ulong channelId, CommandInfo command);
        Task EnableModuleAsync(ulong guildId, ulong channelId, ModuleInfo module);
        Task DisableModuleAsync(ulong guildId, ulong channelId, ModuleInfo module);
    }
}