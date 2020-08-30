using Discord.WebSocket;
using MediatR;

namespace Hiromi.Services.Notifications
{
    public class JoinedGuildNotification : INotification
    {
        public SocketGuild Guild { get;}
        
        public JoinedGuildNotification(SocketGuild guild)
        {
            Guild = guild;
        }
    }
}