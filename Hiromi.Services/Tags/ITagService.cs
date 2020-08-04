using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Interactive;
using Hiromi.Data.Models.Tags;

namespace Hiromi.Services.Tags
{
    public interface ITagService
    {
        Task InvokeTagAsync(ulong guildId, ulong channelId, string name);
        Task CreateTagAsync(ulong guildId, ulong authorId, string name, string content);
        Task ModifyTagAsync(ulong guildId, string name, Action<Tag> action);
        Task DeleteTagAsync(ulong guildId, string name);
        Task<TagSummary> GetTagSummaryAsync(ulong guildId, string name);
        Task<IEnumerable<TagSummary>> GetTagSummaries(ulong guildId, Expression<Func<Tag, bool>> criteria);
        Task<IEnumerable<TagSummary>> GetTagSummaryMatches(ulong guildId, string name);
        Task<bool> HasMaintenancePermissions(string name, IGuildUser user);
    }
}