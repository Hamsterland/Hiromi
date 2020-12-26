using System;
using System.Data;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Hiromi.Bot.Preconditions;
using Hiromi.Services.Tags.Exceptions;
using Hiromi.Services.Tags;

namespace Hiromi.Bot.Modules
{
    [Name("Tag")]
    [Summary("For fast retrieval of text and memes")]
    public class TagModule : ModuleBase<SocketCommandContext>
    {
        private readonly ITagService _tagService;

        public TagModule(ITagService tagService)
        {
            _tagService = tagService;
        }

        [Command("tags enable")]
        [Summary("Enables tags in a channel")]
        [RequireDeveloperOrPermission(GuildPermission.ManageMessages)]
        public async Task Enable(IGuildChannel channel)
        {
            await _tagService.ModifyAllowTagsAsync(Context.Guild.Id, channel.Id, true);
            await ReplyAsync($"Enabled tags in #{channel.Name}");
        }
        
        [Command("tags disable")]
        [Summary("Disables tags in a channel")]
        [RequireDeveloperOrPermission(GuildPermission.ManageMessages)]
        public async Task Disable(IGuildChannel channel)
        {
            await _tagService.ModifyAllowTagsAsync(Context.Guild.Id, channel.Id, false);
            await ReplyAsync($"Disabled tags in #{channel.Name}");
        }

        [Command("tags enable *")]
        [Summary("Enables tags")]
        [RequireDeveloperOrPermission(GuildPermission.ManageMessages)]
        public async Task EnableGlobal()
        {
            var channels = Context.Guild.Channels;
            
            foreach (var channel in channels)
            {
                await _tagService.ModifyAllowTagsAsync(Context.Guild.Id, channel.Id, true);
            }

            await ReplyAsync("Enabled tags across the Guild.");
        }
        
        [Command("tags disable *")]
        [Summary("Enables tags")]
        [RequireDeveloperOrPermission(GuildPermission.ManageMessages)]
        public async Task DisableGlobal()
        {
            var channels = Context.Guild.Channels;
            
            foreach (var channel in channels)
            {
                await _tagService.ModifyAllowTagsAsync(Context.Guild.Id, channel.Id, false);
            }

            await ReplyAsync("Disabled tags across the Guild.");
        }

        [Command("tag")]
        [Summary("Invokes a tag")]
        [RequireEnabledInChannel]
        public async Task Tag(string name)
        {
            await _tagService.InvokeTagAsync(Context.Guild.Id, Context.Channel.Id, name);
        }
        
        [Command("tag create")]
        [Summary("Creates a tag")]
        [RequireEnabledInChannel]
        public async Task Create(string name, [Remainder] string content)
        {
            if (!TagUtilities.IsWithinNameLimit(name))
            {
                await ReplyAsync("Tag names cannot be greater than 50 characters.");
                return;
            }
            
            var current = await _tagService.GetTagSummaries(Context.Guild.Id, x => x.OwnerId == Context.User.Id);
            
            if (!TagUtilities.IsWithinMaxTagLimit(current))
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
        [RequireEnabledInChannel]
        public async Task Delete(string name)
        {
            if (!await _tagService.HasMaintenancePermissions(name, Context.User as IGuildUser))
            {
                await ReplyAsync("You cannot delete this tag.");
                return;
            }

            await _tagService.DeleteTagAsync(Context.Guild.Id, name);
            await ReplyAsync($"Deleted tag \"{name}\".");
        }
        

        [Command("tag info")]
        [Summary("Provides tag information")]
        [RequireEnabledInChannel]
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
            var embed = TagViews.FormatTagInfo(author, owner, tag);
            await ReplyAsync(embed: embed);
        }

        [Command("tags")]
        [Summary("Lists a user's tags")]
        [RequireEnabledInChannel]
        public async Task Tags(IGuildUser user = null)
        {
            user ??= Context.User as IGuildUser;
            var tags = await _tagService.GetTagSummaries(Context.Guild.Id, x => x.OwnerId == user.Id);
            var embed = TagViews.FormatUserTags(user, tags);
            await ReplyAsync(embed: embed);
        }
    }
}