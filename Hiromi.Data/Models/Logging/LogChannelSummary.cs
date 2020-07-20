using System;
using System.Linq.Expressions;

namespace Hiromi.Data.Models.Logging
{
    public class LogChannelSummary
    {
        public int Id { get; set; }
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }

        public static readonly Expression<Func<LogChannel, LogChannelSummary>> FromEntityProjection = logChannel => new LogChannelSummary
            {
                Id = logChannel.Id,
                ChannelId = logChannel.ChannelId,
                GuildId = logChannel.GuildId
            };
    }
}