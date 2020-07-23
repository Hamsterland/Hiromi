using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Hiromi.Services;
using Hiromi.Services.Attributes;
using Hiromi.Services.Exceptions;
using Hiromi.Services.Tags;
using MoreLinq;

namespace Hiromi.Bot.Modules
{
    [Name("Tag")]
    [Summary("For fast retrieval of text and memes")]
    public class TagModule : InteractiveBase
    {
        private readonly ITagService _tagService;

        public TagModule(ITagService tagService)
        {
            _tagService = tagService;
        }
        
        [Command("tag create")]
        [Summary("Creates a tag")]
        public async Task Create(string name, [Remainder] string content)
        {
            try
            {
                await _tagService.CreateTagAsync(Context.Guild.Id, Context.User.Id, name, content);
                await ReplyAsync($"Created tag \"{name}\" (See `tag info {name} for its Id`).");
            }
            catch (TagAlreadyExistsException)
            {
                await ReplyAsync($"A tag by name \"{name}\" already exists. If you are the owner of this tag " +
                                 "or have permission to manage messages, you can delete and recreate it.");
            }
        }
        
        [Command("tag delete")]
        [Summary("Deletes a tag by Id")]
        public async Task Delete(string name)
        {
            var tag = await _tagService.GetTagSummary(Context.Guild.Id, name);

            if (tag is null)
            {
                await ReplyAsync($"No tag by name \"{name}\" found.");
                return;
            }

            if (!tag.CanMaintain(Context.User as IGuildUser))
            {
                await ReplyAsync("You cannot delete this tag.");
                return;
            }

            await _tagService.DeleteTagAsync(Context.Guild.Id, name);
            await ReplyAsync($"Deleted tag \"{tag.Name}\" ({tag.Id}).");
        }
        
        [Command("tag transfer")]
        [Summary("Transfers a tag")]
        public async Task Transfer(IGuildUser user, string name)
        {
            var tag = await _tagService.GetTagSummary(Context.Guild.Id, name);

            if (tag is null)
            {
                await ReplyAsync($"No tag by name \"{name}\" found.");
                return;
            }

            if (!tag.CanMaintain(Context.User as IGuildUser))
            {
                await ReplyAsync("You cannot to transfer this tag.");
                return;
            }

            await _tagService.ModifyTagAsync(Context.Guild.Id, name, x => x.Name = name);
            await ReplyAsync($"Transferred tag \"{tag.Name}\" ({tag.Id}) to {user}.");
        }

        [Command("tag info")]
        [Summary("Provides tag information")]
        public async Task Info()
        {
            
        }

        [Command("tags")]
        [Summary("Lists a user's tags")]
        public async Task Tags(IGuildUser user = null)
        {
            user ??= Context.User as IGuildUser;
            var tags = await _tagService.GetTagSummaries(Context.Guild.Id, x => x.OwnerId == user.Id);
            var tagsList = tags.ToList();

            if (!tagsList.Any())
            {
                await ReplyAsync($"{user} does not have any tags.");
                return;
            }

            var fields = tagsList
                .Select(tagSummary => new EmbedFieldBuilder()
                    .WithName(tagSummary.Id.ToString())
                    .WithValue(tagSummary.Name)
                    .WithIsInline(true));

            var pages = new List<EmbedPage>(fields
                .Batch(10)
                .Select(x => new EmbedPage
                {
                    Author = new EmbedAuthorBuilder()
                        .WithName($"{user}'s Tags")
                        .WithIconUrl(user.GetAvatarUrl()),

                    TotalFieldMessage = "Tags",
                    Fields = x.ToList(),
                    Color = Constants.DefaultColour
                }));

            var pager = new PaginatedMessage
            {
                Pages = pages,
                Options = new PaginatedAppearanceOptions {Timeout = TimeSpan.FromMinutes(1)}
            };

            await PagedReplyAsync(pager, default);
        }
    }
}