using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Hiromi.Services;
using Hiromi.Services.Attributes;
using Hiromi.Services.Logging;


namespace Hiromi.Bot.Modules
{
    [Name("Log")]
    [Summary("Where will I log?")]
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
        [Summary("Sets the log channel")]
        public async Task LogChannelSet(ITextChannel channel = null)
        {
            channel ??= Context.Channel as ITextChannel;
            await _logChannelService.SetLogChannelAsync(Context.Guild.Id, channel.Id);
        }
        
        [Confirm]
        [Command("log channel remove")]
        [Summary("Removes the log channel (disables logging)")]
        public async Task LogChannelRemove()
        {
            await _logChannelService.RemoveLogChannelAsync(Context.Guild.Id);
        }

        [Command("log channel info")]
        [Summary("Shows the current log channel")]
        public async Task Info()
        {
            var logChannel = await _logChannelService.GetLogChannelSummary(Context.Guild.Id);

            if (logChannel is null)
            {
                await ReplyAsync("There is no log channel set. Please set one via the `log channel set` command.");
                return;
            }

            var channel = Context.Guild.GetChannel(logChannel.ChannelId);

            if (channel is null)
            {
                await ReplyAsync(
                    $"The expected log channel with Id of `{channel.Id}` was not found as it was either deleted" +
                    " or Hiromi lost access to it. Please ensure Hiromi has access or set a new log channel via the " +
                    "`log channel set` command.");
                
                return;
            }
            
            var embed = new EmbedBuilder()
                .WithColor(Constants.DefaultColour)
                .WithAuthor(author =>
                    author
                        .WithName($"{Context.Guild} Log Channel")
                        .WithIconUrl(Context.Guild.IconUrl))
                .AddField("Channel", channel.Name)
                .AddField("Id", channel.Id)
                .Build();

            await ReplyAsync(embed: embed);
        }
    }
}