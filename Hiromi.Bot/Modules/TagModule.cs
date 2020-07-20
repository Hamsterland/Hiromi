using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Hiromi.Services.Attributes;
using Hiromi.Services.Tags;

namespace Hiromi.Bot.Modules
{
    [Name("Tag")]
    public class TagModule : ModuleBase<SocketCommandContext>
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
                throw new Exception("Insufficient permissions");
            
            await _tagService.DeleteTagAsync(Context.Guild.Id, x => x.Name == name);
        }
        
        [Confirm]
        [Command("tag delete")]
        public async Task Delete(long id)
        {
            var tagSummary = await _tagService.GetTagSummary(Context.Guild.Id, x => x.Id == id);
            
            if (!_tagService.CanMaintain(Context.User as IGuildUser, tagSummary))
                throw new Exception("Insufficient permissions");
            
            await _tagService.DeleteTagAsync(Context.Guild.Id, x => x.Id == id);
        }
    }
}