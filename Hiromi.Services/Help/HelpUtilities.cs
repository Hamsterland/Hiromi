using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using MoreLinq;

namespace Hiromi.Services.Help
{
    public class HelpUtilities
    {
        public static void AddHelpPages(ModuleInfo module, IEnumerable<EmbedFieldBuilder> fields, ref List<EmbedPage> pages)
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
                            Color = Constants.DefaultEmbedColour
                        };
                    }));
        }
        
        public static IEnumerable<EmbedFieldBuilder> GetCommandUsagesFromModule(ModuleInfo module)
        {
            var fields = new List<EmbedFieldBuilder>();
            
            fields.AddRange(module.Commands
                .Select(command => new EmbedFieldBuilder()
                    .WithName(GetCommandUsage(command))
                    .WithValue(command.Summary)));

            return fields;
        }

        public static string GetCommandUsage(CommandInfo command)
        {
            var sb = new StringBuilder()
                .Append(command.Name);

            if (command.Parameters.Any())
            {
                foreach (var param in command.Parameters)
                {
                    sb.Append(!param.IsOptional
                        ? $" [{param.Name}]"
                        : $" <{param.Name}>");
                }
            }
            
            return sb.ToString();
        }
    }
}