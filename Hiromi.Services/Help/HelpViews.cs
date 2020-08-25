using System;
using System.Collections.Generic;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;

namespace Hiromi.Services.Help
{
    public static class HelpViews
    {
        private const int HELP_EMBED_SECONDS_TIMEOUT = 60;
        
        public static PaginatedMessage FormatHelp(IEnumerable<ModuleInfo> modules)
        {
            var pages = new List<EmbedPage>();
            
            foreach (var module in modules)
            {
                var fields = HelpUtilities.GetCommandUsagesFromModule(module);
                HelpUtilities.AddHelpPages(module, fields, ref pages);
            }
            
            return new PaginatedMessage
            {
                Pages = pages,
                Options = new PaginatedAppearanceOptions { Timeout = TimeSpan.FromSeconds(HELP_EMBED_SECONDS_TIMEOUT) }
            };
        }

        public static PaginatedMessage FormatModuleHelp(ModuleInfo module)
        {
            var pages = new List<EmbedPage>();
            var fields = HelpUtilities.GetCommandUsagesFromModule(module);
            
            HelpUtilities.AddHelpPages(module, fields, ref pages);
            
            return new PaginatedMessage
            {
                Pages = pages,
                Options = new PaginatedAppearanceOptions { Timeout = TimeSpan.FromSeconds(HELP_EMBED_SECONDS_TIMEOUT) }
            };
        }

        public static Embed FormatCommandHelp(CommandInfo command)
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

            return embed.Build();
        }
    }
}