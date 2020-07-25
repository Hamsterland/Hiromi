using System.ComponentModel.Design;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Hiromi.Services.Notifications;
using Hiromi.Services.Tags;
using MediatR;

namespace Hiromi.Services.Listeners.Messages
{
    public class MessageTagListener : INotificationHandler<MessageReceivedNotification>
    {
        private readonly ITagService _tagService;

        public MessageTagListener(ITagService tagService)
        {
            _tagService = tagService;
        }

        private readonly Regex _inlineTagRegex = new Regex(@"\$(\S+)\b",
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

            if (channel.Id == 636631013715476493 
                || channel.Id == 636631078693765144
                || channel.Id == 636630976373587994
                || channel.Id == 636684229652119579
                || channel.Id == 381890266052689920
                || channel.Id == 700496613537480864)
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