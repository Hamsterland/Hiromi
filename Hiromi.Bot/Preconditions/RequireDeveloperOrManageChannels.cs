using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Hiromi.Bot.Preconditions
{
    public class RequireDeveloperOrManageChannels : PreconditionAttribute
    {
        private readonly List<ulong> _developers = new List<ulong> { 330746772378877954 };
        
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (_developers.Contains(context.User.Id) || (context.User as IGuildUser).GuildPermissions.ManageChannels)
            {
                return Task.FromResult(PreconditionResult.FromSuccess());
            }

            return Task.FromResult(PreconditionResult.FromError("User is not a developer or cannot manage channels."));
        }
    }
}