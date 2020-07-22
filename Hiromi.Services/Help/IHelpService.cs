using System.Collections.Generic;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;

namespace Hiromi.Services.Help
{
    public interface IHelpService
    {
        void AddCommandUsage(ModuleInfo module, ref List<EmbedFieldBuilder> fields);
        void AddHelpPages(ModuleInfo module, IEnumerable<EmbedFieldBuilder> fields, ref List<EmbedPage> pages);
        string GetCommandUsage(CommandInfo commandInfo);
    }
}