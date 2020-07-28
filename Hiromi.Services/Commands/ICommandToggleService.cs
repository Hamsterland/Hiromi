using System.Threading.Tasks;
using Discord.Commands;

namespace Hiromi.Services.Commands
{
    public interface ICommandToggleService
    {
        Task EnableCommandAsync(ulong channelId, CommandInfo command);
        Task DisableCommandAsync(ulong channelId, CommandInfo command);
        Task EnableModuleAsync(ulong channelId, ModuleInfo module);
        Task DisableModuleAsync(ulong channelId, ModuleInfo module);
    }
}