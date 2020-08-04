using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Hiromi.Services;
using Hiromi.Services.Help;

namespace Hiromi.Modules
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
        [Alias("h")]
        [Summary("The starting point")]
        public async Task Help()
        {
            var modules = _commandService.Modules.OrderBy(x => x.Name);
            var pages = new List<EmbedPage>();
            
            foreach (var module in modules)
            {
                var fields = HelpUtilities.GetCommandUsagesFromModule(module);
                HelpUtilities.AddHelpPages(module, fields, ref pages);
            }
            
            var pagedEmbed = new PaginatedMessage
            {
                Pages = pages,
                Options = new PaginatedAppearanceOptions {Timeout = TimeSpan.FromSeconds(30)}
            };

            await PagedReplyAsync(pagedEmbed, new ReactionList());
        }

        [Command("help")]
        [Alias("h")]
        [Summary("Module reference")]
        public async Task Help(ModuleInfo module)
        {
            var fields = HelpUtilities.GetCommandUsagesFromModule(module);
            var pages = new List<EmbedPage>();
            
            HelpUtilities.AddHelpPages(module, fields, ref pages);
            
            var pagedEmbed = new PaginatedMessage
            {
                Pages = pages
            };
            
            await PagedReplyAsync(pagedEmbed, new ReactionList());
        }
        
        [Command("help")]
        [Alias("h")]
        [Summary("Command reference")]
        public async Task Help([Remainder] CommandInfo command)
        {
            var embed = new EmbedBuilder()
                .WithColor(Constants.DefaultEmbedColour)
                .WithTitle(command.Name)
                .AddField("Usage", HelpUtilities.GetCommandUsage(command))
                .AddField("Summary", command.Summary);
        
            if (command.Aliases.Count != 0)
            {
                embed.AddField("Aliases", string.Join(',', command.Aliases));
            }
            
            if (command.Remarks != null)
            {
                embed.AddField("Remarks", command.Remarks);
            }
        
            await ReplyAsync(embed: embed.Build());
        }
    }
}