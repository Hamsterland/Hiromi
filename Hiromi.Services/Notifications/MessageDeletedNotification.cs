using Discord;
using Discord.WebSocket;
using MediatR;

namespace Hiromi.Services.Notifications
{
    public class MessageDeletedNotification : INotification
    {
        public Cacheable<IMessage, ulong> Message { get; }
        public ISocketMessageChannel Channel { get; }

        public MessageDeletedNotification(
            Cacheable<IMessage, ulong> message,
            ISocketMessageChannel channel)
        {
            Message = message;
            Channel = channel;
        }
    }
}