using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Hiromi.Bot.Preconditions;
using Hiromi.Data.Models.Tags;
using Hiromi.Services;
using Hiromi.Services.Tags.Exceptions;
using Hiromi.Services.Tags;

namespace Hiromi.Bot.Modules
{
    [Name("Tag")]
    [Summary("For fast retrieval of text and memes")]
    [RequireEnabledChannel]
    public class TagModule : InteractiveBase
    {
        private readonly ITagService _tagService;

        public TagModule(ITagService tagService)
        {
            _tagService = tagService;
        }

        [Command("tag")]
        [Summary("Invokes a tag")]
        public async Task Tag(string name)
        {
            await _tagService.InvokeTagAsync(Context.Guild.Id, Context.Channel.Id, name);
        }
        
        [Command("tag create")]
        [Summary("Creates a tag")]
        public async Task Create(string name, [Remainder] string content)
        {
            if (name.Length > 50)
            {
                await ReplyAsync("Tag names cannot be greater than 50 characters.");
                return;
            }
            
            var current = await _tagService.GetTagSummaries(Context.Guild.Id, x => x.OwnerId == Context.User.Id);
            
            if (current.Count() == 15)
            {
                await ReplyAsync($"{Context.User.Mention} you have reached your limit of 15 tags.");
                return;
            }
            
            try
            {
                await _tagService.CreateTagAsync(Context.Guild.Id, Context.User.Id, name, content);
                await ReplyAsync($"Created tag \"{name}\".");
            }
            catch (TagAlreadyExistsException)
            {
                await ReplyAsync($"A tag by name \"{name}\" already exists.");
            }
        }

        [Command("tag delete")]
        [Summary("Deletes a tag by Id")]
        public async Task Delete(string name)
        {
            if (!await _tagService.CanMaintain(name, Context.User as IGuildUser))
            {
                await ReplyAsync("You cannot delete this tag.");
                return;
            }

            await _tagService.DeleteTagAsync(Context.Guild.Id, name);
            await ReplyAsync($"Deleted tag \"{name}\".");
        }


        [Command("tag transfer")]
        [Summary("Transfers ownership of a tag")]
        public async Task Transfer(string name, IGuildUser user)
        {
            if (!await _tagService.CanMaintain(name, Context.User as IGuildUser))
            {
                await ReplyAsync($"{Context.User.Mention} you cannot transfer tag \"{name}\"");
                return;
            }

            await _tagService.ModifyTagAsync(Context.Guild.Id, name, x => x.OwnerId = user.Id);
            await ReplyAsync($"Transferred tag \"{name}\" to {user}.");
        }

        [Command("tag info")]
        [Summary("Provides tag information")]
        public async Task Info(string name)
        {
            var tag = await _tagService.GetTagSummaryAsync(Context.Guild.Id, name);

            if (tag is null)
            {
                await ReplyAsync($"No tag called \"{name}\" found.");
                return;
            }

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

            var sb = new StringBuilder()
                .AppendLine("```");

            foreach (var tag in tags)
            {
                sb.AppendLine(tag.Name);
            }

            sb.AppendLine("```");

            var embed = new EmbedBuilder()
                .WithColor(Constants.DefaultEmbedColour)
                .WithAuthor(author =>
                    author
                        .WithName($"{user}'s Tags")
                        .WithIconUrl(user.GetAvatarUrl()))
                .WithDescription(sb.ToString())
                .Build();

            await ReplyAsync(embed: embed);
        }
    }
}