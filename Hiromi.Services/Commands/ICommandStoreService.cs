using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hiromi.Services.Commands
{
    public interface ICommandStoreService
    {
        Task LoadEnabledCommands();
        void CacheCommands(ulong channelId, List<string> commands);
        void UnCacheCommands(ulong channelId, IEnumerable<string> commands);
        List<string> GetEnabledCommands(ulong channelId);
        Task StoreCommandsInDbAsync(ulong channelId, IEnumerable<string> commands);
        Task RemoveCommandsFromDbAsync(ulong channelId, IEnumerable<string> commands);
    }
}