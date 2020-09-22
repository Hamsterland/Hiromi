using Discord;
using Discord.Commands;
using MediatR;

namespace Hiromi.Services.Notifications
{
    public class CommandExecutedNotification : INotification
    {
        public Optional<CommandInfo> Command { get; }
        public ICommandContext Context { get; }
        public IResult Result { get; }

        public CommandExecutedNotification(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            Command = command;
            Context = context;
            Result = result;
        }
    }
}