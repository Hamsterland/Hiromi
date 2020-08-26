using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Hiromi.Services.Notifications;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Hiromi.Services.Listeners.Messages
{
    public class MessageCommandListener : INotificationHandler<MessageReceivedNotification>
    {
        private readonly IConfiguration _configuration;
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly CommandService _commandService;
        private readonly IServiceProvider _serviceProvider;

        public MessageCommandListener(
            IConfiguration configuration,
            DiscordSocketClient discordSocketClient, 
            CommandService commandService, 
            IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _discordSocketClient = discordSocketClient;
            _commandService = commandService;
            _serviceProvider = serviceProvider;
        }
        
        public async Task Handle(MessageReceivedNotification notification, CancellationToken cancellationToken)
        {
            if (!(notification.Message is SocketUserMessage message)
                || !(message.Author is IGuildUser user)
                || user.IsBot)
            {
                return;
            }
            
            var argPos = 0;
            var prefix = _configuration["Discord:Prefix"];
            
            if (message.HasStringPrefix(prefix, ref argPos))
            {
                var context = new SocketCommandContext(_discordSocketClient, message);
                var x = await _commandService.ExecuteAsync(context, argPos, _serviceProvider);

                if (!x.IsSuccess)
                {
                    Serilog.Log.Logger.Fatal(x.ErrorReason);
                }
            }
        }
    }
}