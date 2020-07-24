using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Hiromi.Data.Models.Tags;
using Hiromi.Services.Tags.Exceptions;
using Hiromi.Services.Tags;


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
                await ReplyAsync($"A tag by name \"{name}\" already exists.");
            }
        }
        
        [Command("tag delete")]
        [Summary("Deletes a tag by Id")]
        public async Task Delete(TagSummary tagSummary)
        {
            if (!tagSummary.CanMaintain(Context.User as IGuildUser))
            {
                await ReplyAsync("You cannot delete this tag.");
                return;
            }

            await _tagService.DeleteTagAsync(tagSummary);
            await ReplyAsync($"Deleted tag \"{tagSummary.Name}\" ({tagSummary.Id}).");
        }

        [Command("rename")]
        [Summary("Renames a tag")]
        public async Task Rename(TagSummary tag, string name)
        {
            if (!tag.CanMaintain(Context.User as IGuildUser))
            {
                await ReplyAsync($"{Context.User.Mention} you cannot rename tag \"{tag.Name}\"");
                return;
            }
            
            await _tagService.ModifyTagAsync(tag, x => x.Name = name);
            await ReplyAsync($"Renamed tag \"{tag.Name}\" to \"{name}\".");
        }

        [Command("tag transfer")]
        [Summary("Transfers ownership of a tag")]
        public async Task Transfer(TagSummary tagSummary, IGuildUser user)
        {
            if (!tagSummary.CanMaintain(Context.User as IGuildUser))
            {
                await ReplyAsync($"{Context.User.Mention} you cannot transfer tag \"{tagSummary.Name}\"");
                return;
            }
            
            await _tagService.ModifyTagAsync(tagSummary, x => x.OwnerId = user.Id);
            await ReplyAsync($"Transferred tag \"{tagSummary.Name}\" ({tagSummary.Id}) to {user}.");
        }

        [Command("tag info")]
        [Summary("Provides tag information")]
        public async Task Info(TagSummary tagSummary)
        {
            var author = Context.Guild.GetUser(tagSummary.AuthorId);
            var owner = Context.Guild.GetUser(tagSummary.OwnerId);
            var embed = _tagService.FormatTagInfo(author, owner, tagSummary);
            await ReplyAsync(embed: embed);
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

            var pager = _tagService.FormatUserTags(user, tagsList);
            await PagedReplyAsync(pager, default);
        }
    }
}