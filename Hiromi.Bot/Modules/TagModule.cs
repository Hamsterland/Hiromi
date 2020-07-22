using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Hiromi.Services;
using Hiromi.Services.Attributes;
using Hiromi.Services.Tags;
using MoreLinq;

namespace Hiromi.Bot.Modules
{
    [Name("Tag")]
    public class TagModule : InteractiveBase
    {
        private readonly ITagService _tagService;

        public TagModule(ITagService tagService)
        {
            _tagService = tagService;
        }

        [Command("invoke")]
        public async Task Invoke(long id)
        {
            await _tagService.InvokeTagAsync(Context.Guild.Id, Context.Channel.Id, x => x.Id == id);
        }
        
        [Confirm]
        [Command("tag create")]
        public async Task Create(string name, [Remainder] string content)
        {
            await _tagService.CreateTagAsync(Context.Guild.Id, Context.Channel.Id, name, content);
        }

        [Confirm]
        [Command("tag delete")]
        public async Task Delete(string name)
        {
            var tagSummary = await _tagService.GetTagSummary(Context.Guild.Id, x => x.Name == name);

            if (!_tagService.CanMaintain(Context.User as IGuildUser, tagSummary))
            {
                throw new Exception("Insufficient permissions");
            }
            
            await _tagService.DeleteTagAsync(Context.Guild.Id, x => x.Name == name);
        }
        
        [Confirm]
        [Command("tag delete")]
        public async Task Delete(long id)
        {
            var tagSummary = await _tagService.GetTagSummary(Context.Guild.Id, x => x.Id == id);

            if (!_tagService.CanMaintain(Context.User as IGuildUser, tagSummary))
            {
                throw new Exception("Insufficient permissions");
            }
            
            await _tagService.DeleteTagAsync(Context.Guild.Id, x => x.Id == id);
        }

        [Command("tags")]
        public async Task Tags(IGuildUser user)
        {
            var tags = await _tagService.GetTagSummaries(Context.Guild.Id, x => x.OwnerId == user.Id);
            var tagsList = tags;

            var fields = tagsList
                .Select((tagSummary, i) => new EmbedFieldBuilder()
                    .WithName(tagSummary.Id.ToString())
                    .WithValue(tagSummary.Name)
                    .WithIsInline(true))
                .ToList();

            var pages = new List<EmbedPage>(fields
                .Batch(10)
                .Select(x => new EmbedPage
                {
                    Title = $"{user}'s Tags",
                    AlteranteAuthorIcon = user.GetAvatarUrl(),
                    TotalFieldMessage = "Tags",
                    Fields = x.ToList(),
                    Color = Constants.Default
                }));

            var pager = new PaginatedMessage
            {
                Pages = pages,
                Options = new PaginatedAppearanceOptions
                {
                    Timeout = TimeSpan.FromMinutes(1)
                }
            };

            await PagedReplyAsync(pager, default);
        }
    }
}