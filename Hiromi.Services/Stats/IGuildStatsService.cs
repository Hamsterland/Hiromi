using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Hiromi.Data.Models;

namespace Hiromi.Services.Stats
{
    /// <summary>
    /// Describes a service that performs Guild statistic operations.
    /// </summary>
    public interface IGuildStatsService
    {
        /// <summary>
        /// Retrieves the number of messages sent in a guild from a given <see cref="TimeSpan"/> compared to
        /// the current time.
        /// </summary>
        /// <param name="source">The statistics source.</param>
        /// <param name="span">The time duration to take the message count from.</param>
        /// <param name="guildId">The Guild Id.</param>
        /// <param name="userId">The user Id.</param>
        /// <returns>
        /// The number of messages sent.
        /// </returns>
        Task<int> GetMessageCountAsync(StatisticsSource source, TimeSpan span, ulong guildId, ulong userId = 0);

        /// <summary>
        /// Retrieves the Id and message count from the most active channel in a Guild or from a user from a
        /// given <see cref="TimeSpan"/> compared tot he current time.
        /// </summary>
        /// <param name="source">The statistics source.</param>
        /// <param name="span">The time duration to take the activity from.</param>
        /// <param name="guildId">The Guild Id.</param>
        /// <param name="userId"></param>
        /// <returns>
        /// The Id and message count of the most active channel in a Guild or from a user.
        /// </returns>
        Task<(ulong, int)> GetMostActiveChannelAndMessageCountAsync(StatisticsSource source, TimeSpan span, ulong guildId, ulong userId = 0);

        /// <summary>
        /// Retrieves information about the last message sent by a user.
        /// </summary>
        /// <param name="guildId">The Guild Id.</param>
        /// <param name="userId">The user Id.</param>
        /// <returns>
        /// The last message sent.
        /// </returns>
        Task<MessageSummary> GetLastMessageFromUserAsync(ulong guildId, ulong userId);
    }
}