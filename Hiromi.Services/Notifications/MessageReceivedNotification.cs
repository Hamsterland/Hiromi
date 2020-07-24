using Discord.WebSocket;
using MediatR;

namespace Hiromi.Services.Notifications
{
    public class MessageReceivedNotification : INotification
    {
        public SocketMessage Message { get; }
        
        public MessageReceivedNotification(SocketMessage message)
        {
            Message = message;
        }
    }
}