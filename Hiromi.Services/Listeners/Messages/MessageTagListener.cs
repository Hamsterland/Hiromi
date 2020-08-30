using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Hiromi.Data;
using Hiromi.Services.Notifications;
using Hiromi.Services.Tags;
using MediatR;

namespace Hiromi.Services.Listeners.Messages
{
    public class MessageTagListener : INotificationHandler<MessageReceivedNotification>
    {
        private readonly ITagService _tagService;
        private readonly HiromiContext _hiromiContext;

        public MessageTagListener(ITagService tagService, HiromiContext hiromiContext)
        {
            _tagService = tagService;
            _hiromiContext = hiromiContext;
        }
    
        private readonly Regex _inlineTagRegex = new Regex(@"^\$([\s\S]*)", 
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        public async Task Handle(MessageReceivedNotification notification, CancellationToken cancellationToken)
        {
            if (!(notification.Message is IUserMessage message)
                || !(message.Author is IGuildUser user)
                || !(message.Channel is ITextChannel channel)
                || user.IsBot)
            {
                return;
            }

            var guild = await _hiromiContext
                .Guilds
                .FirstOrDefaultAsync(x => x.GuildId == channel.GuildId, cancellationToken);

            if (guild is null || !guild.AllowTags)
            {
                return;
            }
            
            var match = _inlineTagRegex.Match(message.Content);
            if (match.Success)
            {
                var name = match.Groups[1].Value;
                await _tagService.InvokeTagAsync(user.GuildId, channel.Id, name);
            }
        }
    }
}