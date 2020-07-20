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

            var match = _inlineTagRegex.Match(message.Content);
            if (match.Success)
            {
                var name = match.Groups[1].Value;
                await _tagService.InvokeTagAsync(user.GuildId, channel.Id, x => x.Name == name);
            }
        }
    }
}