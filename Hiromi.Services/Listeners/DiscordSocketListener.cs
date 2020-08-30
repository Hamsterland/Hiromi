using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Hiromi.Services.Listeners.Messages;
using Hiromi.Services.Notifications;
using MediatR;
using Microsoft.Extensions.Hosting;

namespace Hiromi.Services.Listeners
{
    public class DiscordSocketListener : IHostedService
    {
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly IMediator _mediator;

        public DiscordSocketListener(
            DiscordSocketClient discordSocketClient,
            IMediator mediator)
        {
            _discordSocketClient = discordSocketClient;
            _mediator = mediator;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _discordSocketClient.MessageReceived += MessageReceived;
            _discordSocketClient.MessageDeleted += MessageDeleted;
            _discordSocketClient.JoinedGuild += JoinedGuild;
            
            _discordSocketClient.Log += Log;
            
            return Task.CompletedTask;
        }
        
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _discordSocketClient.MessageReceived -= MessageReceived;
            _discordSocketClient.MessageDeleted -= MessageDeleted;
            _discordSocketClient.JoinedGuild -= JoinedGuild;

            _discordSocketClient.Log -= Log;
            
            return Task.CompletedTask;
        }

        private async Task MessageReceived(SocketMessage message)
        {
            await _mediator.Publish(new MessageReceivedNotification(message));
        }
        
        private async Task MessageDeleted(Cacheable<IMessage, ulong> message, ISocketMessageChannel channel)
        {
            await _mediator.Publish(new MessageDeletedNotification(message, channel));
        }
        
        private async Task JoinedGuild(SocketGuild guild)
        {
            await _mediator.Publish(new JoinedGuildNotification(guild));
        }
        
        private async Task Log(LogMessage logMessage)
        {
            await _mediator.Publish(new DiscordLogNotification(logMessage));
        }
    }
}