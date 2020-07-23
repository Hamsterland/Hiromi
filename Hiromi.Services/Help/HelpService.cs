using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using MoreLinq;

namespace Hiromi.Services.Help
{
    public class HelpService : IHelpService
    {
        public void AddCommandUsage(ModuleInfo module, ref List<EmbedFieldBuilder> fields)
        {
            fields.AddRange(module.Commands
                .Select(command => new EmbedFieldBuilder()
                    .WithName(GetCommandUsage(command))
                    .WithValue(command.Summary)));
        }

        public void AddHelpPages(ModuleInfo module, IEnumerable<EmbedFieldBuilder> fields, ref List<EmbedPage> pages)
        {
            pages
                .AddRange(fields
                    .Batch(5)
                    .Select(x =>
                    {
                        var list = x.ToList();
                        return new EmbedPage
                        {
                            Title = $"{module.Name} Commands",
                            TotalFieldMessage = list.Count != 1 ? "Commands" : "Command",
                            Description = module.Summary,
                            Fields = list.ToList(),
                            Color = Constants.DefaultColour
                        };
                    }));
        }

        public string GetCommandUsage(CommandInfo commandInfo)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(commandInfo.Name);

            if (commandInfo.Parameters.Count > 0)
            {
                foreach (var parameter in commandInfo.Parameters)
                {
                    stringBuilder.Append(!parameter.IsOptional
                        ? $" [{parameter.Name}]"
                        : $" <{parameter.Name}>");
                }
            }

            return stringBuilder.ToString();
        }
    }
}