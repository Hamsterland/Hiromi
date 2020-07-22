using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using Hiromi.Bot.TypeReaders;
using Hiromi.Services;
using Hiromi.Services.Attributes;
using Hiromi.Services.Tags;
using MoreLinq;

namespace Hiromi.Bot.Modules
{
    [Name("Tag")]
    [Summary("For fast retrieval of text and memes")]
    public class TagModule : InteractiveBase
    {
        private readonly ITagService _tagService;
        private readonly DiscordSocketClient _discordSocketClient;

        public TagModule(ITagService tagService, DiscordSocketClient discordSocketClient)
        {
            _tagService = tagService;
            _discordSocketClient = discordSocketClient;
        }

        [Command("invoke")]
        [Summary("Invokes a tag")]
        public async Task Invoke(long id)
        {
            await _tagService.InvokeTagAsync(Context.Guild.Id, Context.Channel.Id, x => x.Id == id);
        }
        
        [Confirm]
        [Command("tag create")]
        [Summary("Creates a tag")]
        public async Task Create(string name, [Remainder] string content)
        {
            await _tagService.CreateTagAsync(Context.Guild.Id, Context.User.Id, name, content);
        }

        [Confirm]
        [Command("tag delete")]
        [Summary("Deletes a tag by name")]
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
        [Summary("Deletes a tag by Id")]
        public async Task Delete(long id)
        {
            var tagSummary = await _tagService.GetTagSummary(Context.Guild.Id, x => x.Id == id);

            if (!_tagService.CanMaintain(Context.User as IGuildUser, tagSummary))
            {
                throw new Exception("Insufficient permissions");
            }
            
            await _tagService.DeleteTagAsync(Context.Guild.Id, x => x.Id == id);
        }

        [Confirm]
        [Command("tag transfer")]
        [Summary("Transfers a tag")]
        public async Task Transfer(IGuildUser user, long id)
        {
            var tagSummary = await _tagService.GetTagSummary(Context.Guild.Id, x => x.Id == id);
            
            if (!_tagService.CanMaintain(Context.User as IGuildUser, tagSummary))
            {
                throw new Exception("Insufficient permissions");
            }

            await _tagService.ModifyTagAsync(
                Context.Guild.Id, 
                x => x.Id == id, 
                x => x.OwnerId = user.Id);
        }

        [Command("tags")]
        [Summary("Lists a user's tags")]
        public async Task Tags(IGuildUser user = null)
        {
            user ??= Context.User as IGuildUser;
            var tags = await _tagService.GetTagSummaries(Context.Guild.Id, x => x.OwnerId == user.Id);
            var tagsList = tags.ToList();

            var fields = tagsList
                .Select(tagSummary => new EmbedFieldBuilder()
                    .WithName(tagSummary.Id.ToString())
                    .WithValue(tagSummary.Name)
                    .WithIsInline(true))
                .ToList();

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
                Options = new PaginatedAppearanceOptions { Timeout = TimeSpan.FromMinutes(1) }
            };

            await PagedReplyAsync(pager, default);
        }
    }
}