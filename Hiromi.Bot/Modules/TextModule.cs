using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Hiromi.Bot.Modules
{
    [Name("Text")]
    public class TextModule : ModuleBase<SocketCommandContext>
    {
        [Command("echo")]
        public async Task Echo([Remainder] string message)
        {
            await ReplyAsync(message);
            await Context.Message.DeleteAsync();
        }

        [Command("status")]
        public async Task Status([Remainder] string status)
        {
            await Context.Client.SetGameAsync(status, type: ActivityType.Playing);
        }
    }
}