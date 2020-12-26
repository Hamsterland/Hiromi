using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord;
using Humanizer;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Hiromi.Services.Stats
{
    /// <summary>
    /// Embed view models to display statistics data.
    /// </summary>
    public static partial class GuildStatsViews
    {
        /// <summary>
        /// Appends message statistics to a <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="builder">The builder instance.</param>
        /// <param name="weekTotal">The total number of messages sent in the last seven days.</param>
        /// <param name="monthTotal">The total number of messages sent in the last thirty days.</param>
        /// <param name="channelTotal">The Id of the most active channel and the number of messages sent.</param>
        /// <remarks>
        /// This method is used to add both Guild and user message statistics.
        /// </remarks>
        private static void AddMessageStatistics(
            StringBuilder builder,
            int weekTotal,
            (ulong, int) channelTotal)
        {
            var weekTotalInfo = "message".ToQuantity(weekTotal, "n0");
            var average = "message".ToQuantity(weekTotal / 7, "n0");

            var (channelId, total) = channelTotal;

            var channel = MentionUtils.MentionChannel(channelId);
            var channelTotalInfo = $"{channel} ({"message".ToQuantity(total, "n0")})";

            builder
                .AppendLine(Format.Bold("Statistics"))
                .AppendLine($"Last 7 Days: {weekTotalInfo}")
                .AppendLine($"Most Active Channel: {channelTotalInfo}")
                .AppendLine($"Average Per Day: {average}")
                .AppendLine();
        }
    }
}