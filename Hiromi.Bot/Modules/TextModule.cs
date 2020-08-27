using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Hiromi.Bot.Preconditions;
using Hiromi.Services.Attributes;
using Hiromi.Services.Text;

namespace Hiromi.Bot.Modules
{
    [Name("Text")]
    [Summary("Misc text-based commands")]
    [RequireEnabledInChannel]
    public class TextModule : ModuleBase<SocketCommandContext>
    {
        [Command("echo")]
        [Summary("Echoes a message")]
        public async Task Echo([Remainder] string message)
        {
            await ReplyAndDeleteAsync(message);
        }

        [Command("uwu")]
        [Summary("Uwufies a message")]
        public async Task Uwu([Remainder] string message)
        {
            var result = TextUtilities.Uwufy(message);
            await ReplyAndDeleteAsync(result);
        }

        private async Task ReplyAndDeleteAsync(string message)
        {
            await ReplyAsync($"{Format.Bold(Context.User.Username)}: {message}", allowedMentions: AllowedMentions.None);
            await Context.Message.DeleteAsync();
        }
        
        [HelpIgnore]
        [RequireDeveloper]
        [Command("status")]
        [Summary("Sets Hiromi's status")]
        public async Task Status(ActivityType type, [Remainder] string status)
        {
            await Context.Client.SetGameAsync(status, type: type);
            await ReplyAsync("Status changed.");
        }
    }
}