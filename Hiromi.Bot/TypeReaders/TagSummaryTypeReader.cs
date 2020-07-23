using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Hiromi.Data;
using Hiromi.Data.Models.Tags;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Hiromi.Bot.TypeReaders
{
    public class TagSummaryTypeReader : TypeReader
    {
        public override async Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            var hiromiContext = services.GetService<HiromiContext>();
            
            var tag = await hiromiContext
                .Tags
                .Where(x => x.GuildId == context.Guild.Id)
                .Where(x => x.Name == input)
                .Select(TagSummary.FromEntityProjection)
                .FirstOrDefaultAsync();

            if (tag is null)
            {
                var tags = await hiromiContext
                    .Tags
                    .FromSqlRaw("SELECT * FROM \"Tags\" WHERE SIMILARITY(\"Name\", {0}) > 0.1 AND \"GuildId\" = {1}", input, (long) context.Guild.Id)
                    .Select(TagSummary.FromEntityProjection)
                    .ToListAsync();

                if (tags.Count > 0)
                {
                    var builder = new StringBuilder()
                        .AppendLine($"No tag called \"{input}\" found. Did you mean?")
                        .AppendLine("```");

                    foreach (var t in tags)
                    {
                        builder.AppendLine(t.Name);
                    }

                    builder.AppendLine("```");
                    await context.Channel.SendMessageAsync(builder.ToString());
                }
                else
                {
                    await context.Channel.SendMessageAsync($"No tag called \"{input}\" found.");
                }

                return TypeReaderResult.FromSuccess(tags);
            }
            
            return TypeReaderResult.FromSuccess(tag);
        }
    }
}