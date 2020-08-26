using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Hiromi.Services.Notifications;
using MediatR;

// Adapted from Modix 
// https://github.com/discord-csharp/MODiX/blob/278d522c3265728b422c409f67fdac17555361b4/Modix.Services/Quote/MessageLinkBehavior.cs

namespace Hiromi.Listeners
{
    public class MessageLinkListener : INotificationHandler<MessageReceivedNotification>
    {
        private readonly DiscordSocketClient _discordSocketClient;

        public MessageLinkListener(DiscordSocketClient discordSocketClient)
        {
            _discordSocketClient = discordSocketClient;
        }

        private readonly Regex _messageLinkRegex = new Regex(
            @"(?<Prelink>\S+\s+\S*)?(?<OpenBrace><)?https?://(?:(?:ptb|canary)\.)?discord(app)?\.com/channels/(?<GuildId>\d+)/(?<ChannelId>\d+)/(?<MessageId>\d+)/?(?<CloseBrace>>)?(?<Postlink>\S*\s+\S+)?",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        public async Task Handle(MessageReceivedNotification notification, CancellationToken cancellationToken)
        {
            if (!(notification.Message is IUserMessage message)
                || !(message.Author is IGuildUser user))
            {
                return;
            }

            var matches = _messageLinkRegex.Matches(message.Content);
            foreach (Match match in matches)
            {
                if (ulong.TryParse(match.Groups["GuildId"].Value, out _)
                    && ulong.TryParse(match.Groups["ChannelId"].Value, out var channelId)
                    && ulong.TryParse(match.Groups["MessageId"].Value, out var messageId))
                {
                    var channel = _discordSocketClient.GetChannel(channelId);
                    if (channel is IGuildChannel guildChannel && channel is ISocketMessageChannel messageChannel)
                    {
                        var botUser = await guildChannel.Guild.GetCurrentUserAsync();
                        var botChannelPermissions = botUser.GetPermissions(guildChannel);
                        var userChannelPermissions = user.GetPermissions(guildChannel);

                        if (!botChannelPermissions.ViewChannel
                            || !userChannelPermissions.ViewChannel
                            || guildChannel.GuildId != (message.Channel as IGuildChannel).GuildId)
                        {
                            return;
                        }

                        var cacheMode = botChannelPermissions.ReadMessageHistory
                            ? CacheMode.AllowDownload
                            : CacheMode.CacheOnly;

                        var retrievedMessage = await messageChannel.GetMessageAsync(messageId, cacheMode);
                        if (retrievedMessage != null)
                        {
                            await message.Channel.SendMessageAsync(embed: CreateEmbed(user, retrievedMessage));
                        }
                    }
                }
            }
        }

        private static Embed CreateEmbed(IUser quoter, IMessage message)
        {
            var builder = new EmbedBuilder();
            if (message.Embeds.Count > 0)
            {
                var embed = message.Embeds.First();
                if (embed.Type == EmbedType.Rich)
                    builder = embed.ToEmbedBuilder();
            }
            else
            {
                builder
                    .WithColor(252, 166, 205)
                    .WithDescription(message.Content);
                    
                if (message.Attachments.Count > 0)
                {
                    if (message.Attachments.Any(x => x.IsSpoiler()))
                    {
                        builder.AddField("Warning",
                            "This message contains some spoilers (mostly attachments) that Hiromi cannot hide.");
                    }
                    else
                    {
                        if (message.Attachments.Count == 1)
                        {
                            builder.WithImageUrl(message.Attachments.First().Url);
                        }
                        else
                        {
                            foreach (var attatchement in message.Attachments)
                                builder.AddField(attatchement.Filename, attatchement.Url);
                        }
                    }
                }
            }
            
            builder
                .AddField("Quoted by", $"{quoter.Mention} from {Format.Bold(GetJumpUrlForEmbed(message))}")
                .WithAuthor(author =>
                    author
                        .WithName($"{message.Author}")
                        .WithIconUrl(message.Author.GetAvatarUrl()));

            return builder.Build();
        }

        private static string GetJumpUrlForEmbed(IMessage message)
        {
            return Format.Url($"#{message.Channel.Name} (click here)", message.GetJumpUrl());
        }
    }
}