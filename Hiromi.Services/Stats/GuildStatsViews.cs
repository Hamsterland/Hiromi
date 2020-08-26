using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord;
using Discord.WebSocket;
using Humanizer;
using MoreLinq;

namespace Hiromi.Services.Stats
{
    public static class GuildStatsViews
    {
        public static Embed FormatGuildInformation(SocketGuild guild, int week, int month, IReadOnlyDictionary<ulong, int> channelTotal)
        {
            var builder = new StringBuilder();
            AddGuildMetadata(builder, guild);
            AddGuildStatistics(builder, week, month, channelTotal);
            AddMemberInformation(builder, guild);
            AddRoleInformation(builder, guild);

            return new EmbedBuilder()
                .WithColor(Constants.DefaultEmbedColour)
                .WithAuthor(author => author
                    .WithName($"{guild.Name} Information")
                    .WithIconUrl(guild.IconUrl))
                .WithDescription(builder.ToString())
                .WithThumbnailUrl(guild.IconUrl)
                .Build();
        }

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

        private static void AddGuildStatistics(StringBuilder builder, int weekTotal, int monthTotal, IReadOnlyDictionary<ulong, int> channelTotal)
        {
            var weekTotalInfo = "message".ToQuantity(weekTotal, "n0");
            var monthTotalInfo = "message".ToQuantity(monthTotal, "n0");
            var average = "message".ToQuantity(monthTotal / 30, "n0");
            
            var (key, total) = channelTotal
                .OrderByDescending(x => x.Value)
                .FirstOrDefault();

            var channel = MentionUtils.MentionChannel(key);
            var channelTotalInfo = $"{channel} ({"message".ToQuantity(total, "n0")})";

            builder
                .AppendLine(Format.Bold("Statistics"))
                .AppendLine($"Last 7 Days: {weekTotalInfo}")
                .AppendLine($"Last 30 Days: {monthTotalInfo}")
                .AppendLine($"Most Active Channel: {channelTotalInfo}")
                .AppendLine($"Average Per Day: {average}")
                .AppendLine();
        }

        private static void AddMemberInformation(StringBuilder builder, SocketGuild guild)
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

        private static void AddRoleInformation(StringBuilder builder, SocketGuild guild)
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