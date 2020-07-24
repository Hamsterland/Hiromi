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
        public async Task Delete(TagSummary tag)
        {
            if (!tag.CanMaintain(Context.User as IGuildUser))
            {
                await ReplyAsync("You cannot delete this tag.");
                return;
            }

            await _tagService.DeleteTagAsync(tag.GuildId, tag.Name);
            await ReplyAsync($"Deleted tag \"{tag.Name}\" ({tag.Id}).");
        }

        [Command("tag rename")]
        [Summary("Renames a tag")]
        public async Task Rename(TagSummary tag, string name)
        {
            if (!tag.CanMaintain(Context.User as IGuildUser))
            {
                await ReplyAsync($"{Context.User.Mention} you cannot rename tag \"{tag.Name}\"");
                return;
            }
            
            await _tagService.ModifyTagAsync(tag.GuildId, tag.Name, x => x.Name = name);
            await ReplyAsync($"Renamed tag \"{tag.Name}\" to \"{name}\".");
        }

        [Command("tag transfer")]
        [Summary("Transfers ownership of a tag")]
        public async Task Transfer(TagSummary tag, IGuildUser user)
        {
            if (!tag.CanMaintain(Context.User as IGuildUser))
            {
                await ReplyAsync($"{Context.User.Mention} you cannot transfer tag \"{tag.Name}\"");
                return;
            }
            
            await _tagService.ModifyTagAsync(tag.GuildId, tag.Name, x => x.OwnerId = user.Id);
            await ReplyAsync($"Transferred tag \"{tag.Name}\" ({tag.Id}) to {user}.");
        }

        [Command("tag info")]
        [Summary("Provides tag information")]
        public async Task Info(TagSummary tag)
        {
            var author = Context.Guild.GetUser(tag.AuthorId);
            var owner = Context.Guild.GetUser(tag.OwnerId);
            
            var embed = _tagService.FormatTagInfo(author, owner, tag);
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