using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hiromi.Data;
using Hiromi.Data.Models;
using Hiromi.Services.Notifications;
using MediatR;

namespace Hiromi.Services.Listeners.Guilds
{
    public class JoinedGuildListener : INotificationHandler<JoinedGuildNotification>
    {
        private readonly HiromiContext _hiromiContext;

        public JoinedGuildListener(HiromiContext hiromiContext)
        {
            _hiromiContext = hiromiContext;
        }

        public async Task Handle(JoinedGuildNotification notification, CancellationToken cancellationToken)
        {
            var guild = notification.Guild;

            var current = await _hiromiContext
                .Guilds
                .FirstOrDefaultAsync(x => x.GuildId == notification.Guild.Id, cancellationToken);

            if (current != null)
            {
                return;
            }

            _hiromiContext.Add(new Guild
            {
                GuildId = guild.Id,
                AllowTags = true,
                AllowQuotes = true
            });

            await _hiromiContext.SaveChangesAsync(cancellationToken);
        }
    }
}