using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Hiromi.Services.Commands;
using Hiromi.Services.Notifications;
using Hiromi.Services.Tags;
using MediatR;

namespace Hiromi.Services.Listeners.Messages
{
    public class MessageTagListener : INotificationHandler<MessageReceivedNotification>
    {
        private readonly ITagService _tagService;
        private readonly ICommandStoreService _commandStoreService;

        public MessageTagListener(
            ITagService tagService, 
            ICommandStoreService commandStoreService)
        {
            _tagService = tagService;
            _commandStoreService = commandStoreService;
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

            var enabledCommands = _commandStoreService.GetEnabledCommands(channel.Id);

            if (!enabledCommands.Contains("tag"))
            {
                return;
            }

            if (message.Content.Length <= 0)
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