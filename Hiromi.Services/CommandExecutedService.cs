using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Hiromi.Services.Attributes;
using Microsoft.Extensions.Hosting;

namespace Hiromi.Services
{
    public class CommandExecutedService : IHostedService
    {
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly CommandService _commandService;

        public CommandExecutedService(DiscordSocketClient discordSocketClient, CommandService commandService)
        {
            _discordSocketClient = discordSocketClient;
            _commandService = commandService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _commandService.CommandExecuted += CommandExecuted;
            return Task.CompletedTask;
        }
        
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _commandService.CommandExecuted -= CommandExecuted;
            return Task.CompletedTask;
        }
        
        private static async Task CommandExecuted(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (result.IsSuccess)
            {
                foreach (var attribute in command.Value.Attributes)
                {
                    if (attribute is ConfirmAttribute)
                        await context.Message.AddReactionAsync(new Emoji("✅"));
                }

                return; 
            }

            // TODO: Improve Command Error Callback
            await context.Message.AddReactionAsync(new Emoji("❌"));
            await context.Channel.SendMessageAsync(result.ErrorReason);
        }
    }
}