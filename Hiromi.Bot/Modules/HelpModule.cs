using System.Linq;
using System.Threading.Tasks;
using Discord.Addons.Interactive;
using Discord.Commands;
using Hiromi.Services.Help;

namespace Hiromi.Bot.Modules
{
    [Name("Help")]
    [Summary("Ye old handbook")]
    public class HelpModule : InteractiveBase
    {
        private readonly CommandService _commandService;

        public HelpModule(CommandService commandService)
        {
            _commandService = commandService;
        }
        
        [Command("help")]
        [Summary("The starting point")]
        public async Task Help()
        {
            var modules = _commandService.Modules.OrderBy(x => x.Name);
            var pager = HelpViews.FormatHelp(modules);
            await PagedReplyAsync(pager, new ReactionList());
        }

        [Command("help")]
        [Summary("Module reference")]
        public async Task Help(ModuleInfo module)
        {
            var pager = HelpViews.FormatModuleHelp(module);
            await PagedReplyAsync(pager, new ReactionList());
        }
        
        [Command("help")]
        [Summary("Command reference")]
        public async Task Help([Remainder] CommandInfo command)
        {
            var embed = HelpViews.FormatCommandHelp(command);
            await ReplyAsync(embed: embed);
        }
    }
}