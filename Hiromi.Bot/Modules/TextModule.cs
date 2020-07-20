using System.Threading.Tasks;
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
    }
}