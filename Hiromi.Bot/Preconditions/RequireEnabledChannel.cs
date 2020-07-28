using System;
using System.Threading.Tasks;
using Discord.Commands;
using Hiromi.Services.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Hiromi.Bot.Preconditions
{
    public class RequireEnabledChannel : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var commandStoreService = services.GetService<ICommandStoreService>();
            var commands = commandStoreService.GetEnabledCommands(context.Channel.Id);
            
            if (commands != null && commands.Contains(command.Name))
            {
                return Task.FromResult(PreconditionResult.FromSuccess());
            }
            
            return Task.FromResult(PreconditionResult.FromError("This command is not enabled in this channel."));
        }
    }
}