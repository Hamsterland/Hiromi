using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;
using Hiromi.Services.Notifications;
using MediatR;
using Microsoft.Extensions.Hosting;

namespace Hiromi.Services.Listeners
{
    public class DiscordListener : IHostedService
    {
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly IMediator _mediator;

        public DiscordListener(
            DiscordSocketClient discordSocketClient,
            IMediator mediator)
        {
            _discordSocketClient = discordSocketClient;
            _mediator = mediator;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _discordSocketClient.MessageReceived += MessageReceived;

            _discordSocketClient.UserVoiceStateUpdated += UserVoiceStaeUpdated;

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _discordSocketClient.MessageReceived -= MessageReceived;
            
            _discordSocketClient.UserVoiceStateUpdated -= UserVoiceStaeUpdated;
            
            return Task.CompletedTask;
        }

        private Task MessageReceived(SocketMessage message)
        {
            _mediator.Publish(new MessageReceivedNotification(message));
            return Task.CompletedTask;
        }

        private Task UserVoiceStaeUpdated(SocketUser User, SocketVoiceState VoiceState1, SocketVoiceState VoiceState2)
        {
            _mediator.Publish(new UserVoiceStateUpdatedNotification(User, VoiceState1, VoiceState2));
            return Task.CompletedTask;
        }
    }
}