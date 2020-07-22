using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Hiromi.Bot.TypeReaders
{
    public class CommandTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            var commandService = services.GetService<CommandService>();
            var command = commandService.Commands.FirstOrDefault(x => x.Name.ToLower() == input.ToLower());

            return Task.FromResult(command != null 
                ? TypeReaderResult.FromSuccess(command) 
                : TypeReaderResult.FromError(CommandError.ParseFailed, "No module matches the input."));
        }
    }
}