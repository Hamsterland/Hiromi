using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord;
using Discord.WebSocket;
using Humanizer;

namespace Hiromi.Services.Stats
{
    /// <summary>
    /// Embed views to display Guild statistic data.
    /// </summary>
    public static partial class GuildStatsViews
    {
        /// <summary>
        /// Formats Guild information into an <see cref="Embed"/> view.
        /// </summary>
        /// <param name="guild">The Guild.</param>
        /// <param name="weekTotal">The total number of messages sent in the last seven days.</param>
        /// <param name="monthTotal">The total number of messages sent in the last thirty days.</param>
        /// <param name="channelTotal">The Id of the most active channel and the number of messages sent.</param>
        /// <returns>
        /// A formatted <see cref="Embed"/> with Guild statistical information.
        /// </returns>
        public static Embed FormatGuildInformation(
            SocketGuild guild,
            int weekTotal, 
            int monthTotal,
            (ulong, int) channelTotal)
        {
            var builder = new StringBuilder();
            
            AddGuildMetadata(builder, guild);
            AddMessageStatistics(builder, weekTotal, monthTotal, channelTotal);
            AddGuildMemberInformation(builder, guild);
            AddGuildRoleInformation(builder, guild);

            return new EmbedBuilder()
                .WithColor(Constants.DefaultEmbedColour)
                .WithAuthor(author => author
                    .WithName($"{guild.Name} Guild Information")
                    .WithIconUrl(guild.IconUrl))
                .WithDescription(builder.ToString())
                .Build();
        }

        /// <summary>
        /// Appends Guild metadata to the <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="builder">"/>The builder.</param>
        /// <param name="guild">The Guild.</param>
        private static void AddGuildMetadata(StringBuilder builder, IGuild guild)
        {
            var owner = MentionUtils.MentionUser(guild.OwnerId);
            var created = $"{guild.CreatedAt.Humanize()} ({guild.CreatedAt:D})";

            builder
                .AppendLine(Format.Bold("Metadata"))
                .AppendLine($"Id: {guild.Id}")
                .AppendLine($"Owner: {owner}")
                .AppendLine($"Created: {created}")
                .AppendLine();
        }
        
        /// <summary>
        /// Appends Guild member information to the <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="guild">The Guild.</param>
        private static void AddGuildMemberInformation(StringBuilder builder, SocketGuild guild)
        {
            var memberTotal = guild.Users.Count;
            var onlineTotal = guild.Users.Count(x => x.Status != UserStatus.Offline);
            var botTotal = guild.Users.Count(x => x.IsBot);
            var humanTotal = memberTotal - botTotal;

            builder
                .AppendLine(Format.Bold("Member Information"))
                .AppendLine($"Total: {memberTotal} ({onlineTotal} Online)")
                .AppendLine($"Humans: {humanTotal}")
                .AppendLine($"Bots: {botTotal}")
                .AppendLine();
        }

        /// <summary>
        /// Appends Guild role information to the <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="guild">The Guild.</param>
        private static void AddGuildRoleInformation(StringBuilder builder, SocketGuild guild)
        {
            var roles = guild
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