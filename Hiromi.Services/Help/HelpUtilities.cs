using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Hiromi.Services.Attributes;
using MoreLinq;

namespace Hiromi.Services.Help
{
    public static class HelpUtilities
    {
        public static void AddHelpPages(ModuleInfo module, IEnumerable<EmbedFieldBuilder> fields, ref List<EmbedPage> pages)
        {
            foreach (var attr in module.Attributes)
            {
                if (!(attr is HelpDisplay helpDisplay)) 
                    continue;
                
                if (helpDisplay.Option is HelpDisplayOptions.Hide)
                {
                    return; 
                }
            }
            
            pages
                .AddRange(fields
                    .Batch(5)
                    .Select(builders =>
                    {
                        var buildersList = builders.ToList();
                        return new EmbedPage
                        {
                            Title = $"{module.Name} Commands",
                            TotalFieldMessage = buildersList.Count != 1 ? "Commands" : "Command",
                            Description = module.Summary,
                            Fields = buildersList,
                            Color = Constants.DefaultEmbedColour
                        };
                    }));
        }
        
        public static IEnumerable<EmbedFieldBuilder> GetCommandUsagesFromModule(ModuleInfo module)
        {
            var fields = (from command in module.Commands
                where !command.Attributes.Any(x => x is HelpDisplay)
                select new EmbedFieldBuilder()
                    .WithName(GetCommandUsage(command))
                    .WithValue(command.Summary!))
                .ToList();

            // fields.AddRange(module.Commands
            //     .Select(command => new EmbedFieldBuilder()
            //         .WithName(GetCommandUsage(command))
            //         .WithValue(command.Summary)));

            return fields;
        }

        public static string GetCommandUsage(CommandInfo command)
        {
            var sb = new StringBuilder().Append(command.Name);

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