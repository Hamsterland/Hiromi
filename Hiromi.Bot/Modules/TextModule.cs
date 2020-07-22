using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Hiromi.Bot.Modules
{
    [Name("Text")]
    [Summary("Misc text-based commands")]
    public class TextModule : ModuleBase<SocketCommandContext>
    {
        [Command("echo")]
        [Summary("Echoes a message")]
        public async Task Echo([Remainder] string message)
        {
            await ReplyAsync(message);
            await Context.Message.DeleteAsync();
        }
        
        [Command("status")]
        [Summary("Sets Hiromi's status")]
        public async Task Status(ActivityType type, [Remainder] string status)
        {
            await Context.Client.SetGameAsync(status, type: type);
        }
    }
}