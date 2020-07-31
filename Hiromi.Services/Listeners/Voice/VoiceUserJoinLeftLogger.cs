using System.Linq;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Hiromi.Data;
using Hiromi.Services.Notifications;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hiromi.Services.Listeners.Voice
{
    public class VoiceUserJoinLeftLogger : INotificationHandler<UserVoiceStateUpdatedNotification>
    {
        private readonly HiromiContext _hiromiContext;
        private readonly DiscordSocketClient _discordSocketClient;

        public VoiceUserJoinLeftLogger(
            HiromiContext hiromiContext,
            DiscordSocketClient discordSocketClient)
        {
            _hiromiContext = hiromiContext;
            _discordSocketClient = discordSocketClient;
        }

        public async Task Handle(UserVoiceStateUpdatedNotification notification, CancellationToken cancellationToken)
        {
            var logChannel = await _hiromiContext
                .Channels
                .Where(x => x.GuildId == (notification.User as IGuildUser).GuildId)
                .Where(x => x.IsLogChannel)
                .FirstOrDefaultAsync(cancellationToken);

            if (logChannel is null)
            {
                return;
            }
            
            var channel = _discordSocketClient.GetChannel(logChannel.ChannelId) as ITextChannel;

            var builder = new EmbedBuilder()
                .WithColor(Constants.DefaultEmbedColour)
                .WithCurrentTimestamp();

            if (notification.VoiceState1.VoiceChannel is null && notification.VoiceState2.VoiceChannel != null)
            {
                var embed = builder
                    .WithTitle("User Joined Voice Channel")
                    .AddField("User", notification.User.Mention)
                    .AddField("Channel", notification.VoiceState2.VoiceChannel.Name)
                    .Build();

                await channel.SendMessageAsync(embed: embed);
                return;
            }

            if (notification.VoiceState1.VoiceChannel != null && notification.VoiceState2.VoiceChannel is null)
            {
                var embed = builder
                    .WithTitle("User Left Voice Channel")
                    .AddField("User", notification.User.Mention)
                    .AddField("Channel", notification.VoiceState1.VoiceChannel.Name)
                    .Build();

                await channel.SendMessageAsync(embed: embed);
                return;
            }

            if (notification.VoiceState1.VoiceChannel != null
                && notification.VoiceState2.VoiceChannel != null
                && notification.VoiceState1.VoiceChannel != notification.VoiceState2.VoiceChannel)
            {
                var embed = builder
                    .WithTitle("User Changed Voice Channel")
                    .AddField("User", notification.User.Mention)
                    .AddField("Old Channel", notification.VoiceState1.VoiceChannel.Name)
                    .AddField("New Channel", notification.VoiceState2.VoiceChannel.Name)
                    .Build();

                await channel.SendMessageAsync(embed: embed);
            }
        }
    }
}