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
    public class UserJoinLeftListenerLogger : INotificationHandler<UserVoiceStateUpdatedNotification>
    {
        private readonly HiromiContext _hiromiContext;
        private readonly DiscordSocketClient _discordSocketClient;

        public UserJoinLeftListenerLogger(
            HiromiContext hiromiContext, 
            DiscordSocketClient discordSocketClient)
        {
            _hiromiContext = hiromiContext;
            _discordSocketClient = discordSocketClient;
        }
        
        public async Task Handle(UserVoiceStateUpdatedNotification notification, CancellationToken cancellationToken)
        {
            var logChannel = await _hiromiContext.LogChannels
                .Where(x => x.GuildId == (notification.User as IGuildUser).GuildId)
                .FirstOrDefaultAsync(cancellationToken);
            
            var channel = _discordSocketClient.GetChannel(logChannel.ChannelId);
            
            if (notification.VoiceState1.VoiceChannel is null && notification.VoiceState2.VoiceChannel != null)
            {
                var embed = new EmbedBuilder()
                    .WithColor(Constants.Default)
                    .WithTitle("User Joined Voice Channel")
                    .AddField("User", notification.User.Mention)
                    .AddField("Channel", notification.VoiceState2.VoiceChannel.Name)
                    .WithCurrentTimestamp()
                    .Build();
                
                await (channel as ITextChannel).SendMessageAsync(embed: embed);
                return;
            }
            
            if (notification.VoiceState1.VoiceChannel != null && notification.VoiceState2.VoiceChannel is null)
            {
                var embed = new EmbedBuilder()
                    .WithColor(Constants.Default)
                    .WithTitle("User Left Voice Channel")
                    .AddField("User", notification.User.Mention)
                    .AddField("Channel", notification.VoiceState1.VoiceChannel.Name)
                    .WithCurrentTimestamp()
                    .Build();
             
                await (channel as ITextChannel).SendMessageAsync(embed: embed);
                return; 
            }

            if (notification.VoiceState1.VoiceChannel != null && notification.VoiceState2.VoiceChannel != null)
            {
                if (notification.VoiceState1.VoiceChannel != notification.VoiceState2.VoiceChannel)
                {
                    var embed = new EmbedBuilder()
                        .WithColor(Constants.Default)
                        .WithTitle("User Changed Voice Channel")
                        .AddField("User", notification.User.Mention)
                        .AddField("Old Channel", notification.VoiceState1.VoiceChannel.Name)
                        .AddField("New Channel", notification.VoiceState2.VoiceChannel.Name)
                        .WithCurrentTimestamp()
                        .Build();
             
                    await (channel as ITextChannel).SendMessageAsync(embed: embed);
                }
            }
        }
    }
}