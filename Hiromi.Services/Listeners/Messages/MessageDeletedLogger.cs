using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Hiromi.Services.Logging;
using Hiromi.Services.Notifications;
using MediatR;

namespace Hiromi.Services.Listeners.Messages
{
    public class MessageDeletedLogger : INotificationHandler<MessageDeletedNotification>
    {
        private readonly ILogChannelService _logChannelService;
        private readonly DiscordSocketClient _discordSocketClient;

        public MessageDeletedLogger(
            ILogChannelService logChannelService, 
            DiscordSocketClient discordSocketClient)
        {
            _logChannelService = logChannelService;
            _discordSocketClient = discordSocketClient;
        }

        public async Task Handle(MessageDeletedNotification notification, CancellationToken cancellationToken)
        {
            var logChannel = await _logChannelService.GetLogChannelSummary((notification.Channel as IGuildChannel).GuildId);

            if (logChannel is null)
            {
                return;
            }

            var channel = _discordSocketClient.GetChannel(logChannel.ChannelId);

            if (!(channel is ITextChannel textChannel))
            {
                return;
            }

            var embed = new EmbedBuilder()
                .WithTitle("Message Deleted")
                .WithColor(Constants.DefaultEmbedColour);

            if (!notification.Message.HasValue)
            {
                embed.WithDescription("Due to reasons you won't understand, I was not able to retrieve the message content.");
            }
            else
            {
                embed
                    .AddField("User", notification.Message.Value.Author)
                    .AddField("Content", notification.Message.Value.Content);
            }

            await textChannel.SendMessageAsync(embed: embed.Build());
        }
    }
}