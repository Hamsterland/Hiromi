using System.Threading;
using System.Threading.Tasks;
using Discord;
using Hiromi.Data;
using Hiromi.Data.Models;
using Hiromi.Services.Notifications;
using MediatR;

namespace Hiromi.Services.Listeners.Messages
{
    public class MessageReceivedListener : INotificationHandler<MessageReceivedNotification>
    {
        private readonly HiromiContext _hiromiContext;

        public MessageReceivedListener(HiromiContext hiromiContext)
        {
            _hiromiContext = hiromiContext;
        }

        public async Task Handle(MessageReceivedNotification notification, CancellationToken cancellationToken)
        {
            var received = notification.Message;
            
            var message = new Message
            {
                MessageId = received.Id,
                UserId = received.Author.Id,
                ChannelId = received.Channel.Id,
                GuildId = (received.Channel as IGuildChannel).Guild.Id
            };
            
            _hiromiContext.Add(message);
            await _hiromiContext.SaveChangesAsync(cancellationToken);
        }
    }
}