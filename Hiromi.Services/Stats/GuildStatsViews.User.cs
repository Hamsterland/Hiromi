using System;
using System.Linq;
using System.Text;
using Discord;
using Discord.WebSocket;
using Humanizer;

namespace Hiromi.Services.Stats
{
    /// <summary>
    /// Embed views to display user statistics data.
    /// </summary>
    public static partial class GuildStatsViews
    {
        /// <summary>
        /// Formats user information into an <see cref="Embed"/> view.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="guild">The Guild.</param>
        /// <param name="weekTotal">The total number of messages sent in the last seven days.</param>
        /// <param name="monthTotal">The total number of messages sent in the last thirty days.</param>
        /// <param name="channelTotal">The Id of the most active channel and the number of messages sent.</param>
        /// <param name="lastMessageDate">The most recent message sent by the user.</param>
        /// <returns>
        /// A formatted <see cref="Embed"/> with user statistical information.
        /// </returns>
        public static Embed FormatUserInformation(
            SocketGuildUser user,
            IGuild guild,
            int weekTotal, 
            int monthTotal,
            (ulong, int) channelTotal,
            DateTime lastMessageDate)
        {
            var builder = new StringBuilder();
            
            AddUserMetadata(builder, user, lastMessageDate);       
            AddMessageStatistics(builder, weekTotal, monthTotal, channelTotal);
            AddUserRoles(builder, user, guild);

            return new EmbedBuilder()
                .WithColor(Constants.DefaultEmbedColour)
                .WithAuthor(author => author
                    .WithName($"{user.Username}'s User Information")
                    .WithIconUrl(user.GetAvatarUrl()))
                .WithDescription(builder.ToString())
                .Build();
        }
        
        /// <summary>
        /// Appends user metadata to the <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="user">The user.</param>
        /// <param name="lastMessageDate">The most recent message sent by the user.</param>
        private static void AddUserMetadata(StringBuilder builder, IGuildUser user, DateTime lastMessageDate)
        {
            var firstSeen = $"{user.JoinedAt.Humanize()} ({user.JoinedAt:D})";
            var lastSeen = $"{DateTime.Now.Subtract(lastMessageDate).Humanize()} ago ({lastMessageDate:D})";

            builder
                .AppendLine(Format.Bold("Metadata"))
                .AppendLine($"Id: {user.Id}")
                .AppendLine($"Profile: {user.Mention}")
                .AppendLine($"Status: {user.Status}")
                .AppendLine($"First Seen: {firstSeen}")
                .AppendLine($"Last Seen: {lastSeen}")
                .AppendLine();
        }

        /// <summary>
        /// Appends user roles to the <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="user">The user.</param>
        /// <param name="guild">The guild.</param>
        private static void AddUserRoles(StringBuilder builder, SocketGuildUser user, IGuild guild)
        {
            var roles = user
                .Roles
                .Where(x => x.Id != guild.EveryoneRole.Id)
                .Where(x => x.Color != Color.Default)
                .Where(x => x.IsHoisted)
                .OrderByDescending(x => x.Position);
            
            builder
                .AppendLine(Format.Bold("Roles"))
                .AppendLine(roles.Select(x => x.Mention).Humanize())
                .AppendLine();
        }
    }
}