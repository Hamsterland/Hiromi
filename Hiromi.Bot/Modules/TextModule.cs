using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Hiromi.Bot.Preconditions;
using Hiromi.Services.Attributes;
using Hiromi.Services.Monkey;
using Hiromi.Services.Text;

namespace Hiromi.Bot.Modules
{
    [Name("Text")]
    [Summary("Misc text-based commands")]
    [RequireEnabledInChannel]
    public class TextModule : ModuleBase<SocketCommandContext>
    {
        private readonly IMonkeyService _monkeyService;

        public TextModule(IMonkeyService monkeyService)
        {
            _monkeyService = monkeyService;
        }

        [Command("where")]
        [Summary("Where??!??!?!")]
        public async Task Where(string name)
        {
            var path = _monkeyService.DrawMonkey(name);
            await Context.Channel.SendFileAsync(path);
        }
        
        [Command("echo")]
        [Summary("Echoes a message")]
        public async Task Echo([Remainder] string message)
        {
            await ReplyAsync(message);
        }

        [Command("uwu")]
        [Summary("Uwufies a message")]
        public async Task Uwu([Remainder] string message)
        {
            var result = TextUtilities.Uwufy(message);
            await ReplyAsync(result);
        }

        private async Task ReplyAsync(string message)
        {
            await ReplyAsync($"{Format.Bold(Context.User.ToString())}: {message}", allowedMentions: AllowedMentions.None);
            await Context.Message.DeleteAsync();
        }
        
        [HelpDisplay(HelpDisplayOptions.Hide)]
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