using Discord.WebSocket;
using MediatR;

namespace Hiromi.Services.Notifications
{
    public class UserJoinedNotification : INotification
    {
        public SocketGuildUser User { get; }

        public UserJoinedNotification(SocketGuildUser user)
        {
            User = user;
        }
    }
}