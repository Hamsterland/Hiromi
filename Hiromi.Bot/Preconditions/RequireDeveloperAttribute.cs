using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;

namespace Hiromi.Bot.Preconditions
{
    public class RequireDeveloperAttribute : PreconditionAttribute
    {
        private readonly List<ulong> _developers = new List<ulong> { 330746772378877954 };
        
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            return Task.FromResult(_developers.Any(x => x == context.User.Id) 
                ? PreconditionResult.FromSuccess() 
                : PreconditionResult.FromError("User is not a developer"));
        }
    }
}