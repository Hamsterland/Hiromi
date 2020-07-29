using Discord;
using MediatR;

namespace Hiromi.Services.Notifications
{
    public class DiscordLogNotification : INotification
    {
        public LogMessage LogMessage { get; }
        
        public DiscordLogNotification(LogMessage logMessage)
        {
            LogMessage = logMessage;
        }
    }
}