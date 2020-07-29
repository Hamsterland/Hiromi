using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Hiromi.Bot.Preconditions
{
    public class RequireDeveloperOrPermissionAttribute : PreconditionAttribute
    {
        public GuildPermission Permission;

        public RequireDeveloperOrPermissionAttribute(GuildPermission permission)
        {
            Permission = permission;
        }
        
        private readonly List<ulong> _developers = new List<ulong> { 330746772378877954 };
        
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if ((context.User as IGuildUser).GuildPermissions.Has(Permission) || _developers.Contains(context.User.Id))
            {
                return Task.FromResult(PreconditionResult.FromSuccess());
            }

            return Task.FromResult(PreconditionResult.FromError("Insufficient Permissions"));
        }
    }
}