using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Hiromi.Services.Attributes;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Hiromi.Services.Hosted
{
    public class PostCommandService : IHostedService
    {
        private readonly CommandService _commandService;
        private readonly ILogger _logger; 

        public PostCommandService(
            CommandService commandService,
            ILogger logger)
        {
            _commandService = commandService;
            _logger = logger;
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
        
        private async Task CommandExecuted(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (result.IsSuccess)
            {
                foreach (var attribute in command.Value.Attributes)
                {
                    if (attribute is ConfirmAttribute)
                        await context.Message.AddReactionAsync(new Emoji("✅"));
                }
            }
            else
            {
                _logger.Fatal(result.ErrorReason);
            }
        }
    }
}