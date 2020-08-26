using System;
using System.Threading.Tasks;
using Discord.Commands;
using Hiromi.Services.Stats;

namespace Hiromi.Bot.Modules
{
    [Name("Guild Information")]
    [Summary("Nice to know about your own Guild")]
    public class GuildModule : ModuleBase<SocketCommandContext>
    {
        private readonly IGuildStatsService _guildStatsService;

        public GuildModule(IGuildStatsService guildStatsService)
        {
            _guildStatsService = guildStatsService;
        }

        [Command("serverinfo")]
        [Alias("guildinfo")]
        [Summary("Shows Guild information")]
        public async Task Info()
        {
            var week = await _guildStatsService.GetMessagesCount(x => x.GuildId == Context.Guild.Id, TimeSpan.FromDays(7));
            var month = await _guildStatsService.GetMessagesCount(x => x.GuildId == Context.Guild.Id, TimeSpan.FromDays(30));
            var embed = GuildStatsViews.FormatGuildInformation(Context.Guild, week, month);
            await ReplyAsync(embed: embed);
        }
    }
}