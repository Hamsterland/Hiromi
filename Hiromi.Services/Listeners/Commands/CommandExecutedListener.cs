using System.Threading;
using System.Threading.Tasks;
using Discord.Commands;
using Hiromi.Services.Notifications;
using MediatR;
using Serilog;

namespace Hiromi.Services.Listeners.Commands
{
    public class CommandExecutedListener : INotificationHandler<CommandExecutedNotification>
    {
        private readonly ILogger _logger;

        public CommandExecutedListener(ILogger logger)
        {
            _logger = logger;
        }

        public Task Handle(CommandExecutedNotification notification, CancellationToken cancellationToken)
        {
            if (!notification.Result.IsSuccess)
            {
                _logger.Fatal(notification.Result.ErrorReason ?? "No Reason");
            }
            
            return Task.CompletedTask;
        }
    }
}