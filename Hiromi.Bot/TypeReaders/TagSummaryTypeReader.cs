using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Hiromi.Data;
using Hiromi.Data.Models.Tags;
using Hiromi.Services.Tags;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Hiromi.Bot.TypeReaders
{
    public class TagSummaryTypeReader : TypeReader
    {
        public override async Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            var hiromiContext = services.GetService<HiromiContext>();
            var tagService = services.GetService<ITagService>();

            var tag = await hiromiContext
                .Tags
                .Where(x => x.GuildId == context.Guild.Id)
                .Where(x => x.Name == input)
                .Select(TagSummary.FromEntityProjection)
                .FirstOrDefaultAsync();

            if (tag is null)
            {
                var tags = await tagService.GetTagSummaryMatches(context.Guild.Id, input);
                var tagsList = tags.ToList();

                var embed = tagService.FormatMatchedTags(input, tagsList);
                await context.Channel.SendMessageAsync(embed: embed);
                
                return TypeReaderResult.FromSuccess(tags);
            }
            
            return TypeReaderResult.FromSuccess(tag);
        }
    }
}