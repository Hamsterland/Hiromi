using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Discord;
using Hiromi.Data.Models.Tags;

namespace Hiromi.Services.Tags
{
    public interface ITagService
    {
        Task InvokeTagAsync(ulong guildId, ulong channelId, string name);
        Task CreateTagAsync(ulong guildId, ulong authorId, string name, string content);
        Task ModifyTagAsync(TagSummary tagSummary, Action<TagSummary> action);
        Task DeleteTagAsync(TagSummary tagSummary);
        Task<TagSummary> GetTagSummary(ulong guildId, string name);
        Task<IEnumerable<TagSummary>> GetTagSummaries(ulong guildId, Expression<Func<TagEntity, bool>> criteria);
        bool CanMaintain(IGuildUser user, TagSummary tagSummary);
    }
}