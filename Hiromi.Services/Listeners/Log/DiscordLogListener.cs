using System.Threading;
using System.Threading.Tasks;
using Discord;
using Hiromi.Services.Notifications;
using MediatR;
using Serilog;

namespace Hiromi.Services.Listeners.Log
{
    public class DiscordLogListener : INotificationHandler<DiscordLogNotification>
    {
        private readonly ILogger _logger;

        public DiscordLogListener(ILogger logger)
        {
            _logger = logger;
        }

        public Task Handle(DiscordLogNotification notification, CancellationToken cancellationToken)
        {
            var log = notification.LogMessage.ToString();

            switch (notification.LogMessage.Severity)
            {
                case LogSeverity.Critical:
                    _logger.Fatal(log);
                    break;
                
                case LogSeverity.Error:
                    _logger.Error(log);
                    break;
                
                case LogSeverity.Warning:
                    _logger.Warning(log);
                    break;
                
                case LogSeverity.Info:
                    _logger.Information(log);
                    break;
                
                case LogSeverity.Verbose:
                    _logger.Verbose(log);
                    break;
                
                case LogSeverity.Debug:
                    _logger.Debug(log);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}