using System.Threading;
using System.Threading.Tasks;
using Hiromi.Services.Notifications;
using MediatR;

namespace Hiromi.Services.Listeners.Users
{
    public class GuildWelcomeListener : INotificationHandler<UserJoinedNotification>
    {
        public async Task Handle(UserJoinedNotification notification, CancellationToken cancellationToken)
        {
            
        }
    }
}