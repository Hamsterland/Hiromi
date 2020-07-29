using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Hiromi.Bot.Preconditions;
using Hiromi.Services.Commands;

namespace Hiromi.Bot.Modules
{
    [Name("Channel")]
    [Summary("Handles channel configuration")]
    [Remarks("To be merged with the LogChannel module")]
    [RequireDeveloperOrPermission(GuildPermission.ManageChannels)]
    public class ChannelModule : ModuleBase<SocketCommandContext>
    {
        private readonly ICommandToggleService _commandToggleService;

        public ChannelModule(ICommandToggleService commandToggleService)
        {
            _commandToggleService = commandToggleService;
        }

        [Command("enable command")]
        [Summary("Enables a command in a channel")]
        public async Task EnableCommand(CommandInfo command, IGuildChannel channel = null)
        {
            channel ??= Context.Channel as IGuildChannel;

            await _commandToggleService.EnableCommandAsync(Context.Guild.Id, channel.Id, command);
            await ReplyAsync($"Enabled command \"{command.Name}\" in <#{channel.Id}> (if it wasn't already enabled).");
        }

        [Command("enable module")]
        [Summary("Enables a module in a channel")]
        public async Task EnableModule(ModuleInfo module, IGuildChannel channel = null)
        {
            channel ??= Context.Channel as IGuildChannel;

            await _commandToggleService.EnableModuleAsync(Context.Guild.Id, channel.Id, module);
            await ReplyAsync($"Enabled module \"{module.Name}\" in <#{channel.Id}> (if it wasn't already enabled).");
        }

        [Command("disable command")]
        [Summary("Disables a command in a channel")]
        public async Task DisableCommand(CommandInfo command, IGuildChannel channel = null)
        {
            channel ??= Context.Channel as IGuildChannel;

            await _commandToggleService.DisableCommandAsync(Context.Guild.Id, channel.Id, command);
            await ReplyAsync($"Disabled command \"{command.Name}\" in <#{channel.Id}> (if it wasn't already disabled).");
        }

        [Command("disable module")]
        [Summary("Enables a module in a channel")]
        public async Task DisableModule(ModuleInfo module, IGuildChannel channel = null)
        {
            channel ??= Context.Channel as IGuildChannel;

            await _commandToggleService.DisableModuleAsync(Context.Guild.Id, channel.Id, module);
            await ReplyAsync($"Disabled module \"{module.Name}\" in <#{channel.Id}> (if it wasn't already disabled).");
        }
    }
}