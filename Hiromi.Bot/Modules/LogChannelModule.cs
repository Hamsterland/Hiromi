﻿using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Hiromi.Services.Attributes;
using Hiromi.Services.Logging;
using Serilog;

namespace Hiromi.Bot.Modules
{
    [Name("Log")]
    [RequireUserPermission(GuildPermission.ManageChannels)]
    public class LogModule : ModuleBase<SocketCommandContext>
    {
        private readonly ILogChannelService _logChannelService;

        public LogModule(ILogChannelService logChannelService)
        {
            _logChannelService = logChannelService;
        }

        [Confirm]
        [Command("log channel set")]
        public async Task LogChannelSet(ITextChannel channel = null)
        {
            channel ??= Context.Channel as ITextChannel;
            await _logChannelService.SetLogChannel(Context.Guild.Id, channel.Id);
        }
        
        [Confirm]
        [Command("log channel remove")]
        public async Task LogChannelRemove()
        {
            Log.Information("Reached");
            await _logChannelService.RemoveLogChannel(Context.Guild.Id);
        }
    }
}