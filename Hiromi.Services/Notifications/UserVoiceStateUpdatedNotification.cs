using Discord.WebSocket;
using MediatR;

namespace Hiromi.Services.Notifications
{
    public class UserVoiceStateUpdatedNotification : INotification
    {
        public SocketUser User { get; }
        public SocketVoiceState VoiceState1 { get; }
        public SocketVoiceState VoiceState2 { get; }

        public UserVoiceStateUpdatedNotification(
            SocketUser user,
            SocketVoiceState voiceState1, 
            SocketVoiceState voiceState2)
        {
            User = user;
            VoiceState1 = voiceState1;
            VoiceState2 = voiceState2;
        }
    }
}