using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Hiromi.Services;
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
        public async Task GuildInfo()
        {
            var weekTotal = await _guildStatsService.GetMessageCountAsync(
                StatisticsSource.Guild,
                TimeSpan.FromDays(7), 
                Context.Guild.Id);

            var monthTotal = await _guildStatsService.GetMessageCountAsync(
                StatisticsSource.Guild,
                TimeSpan.FromDays(30), 
                Context.Guild.Id);
            
            var channelTotal = await _guildStatsService.GetMostMessageCountByChannelAsync(
                TimeSpan.FromDays(7),
                Context.Guild.Id);
            
            var embed = GuildStatsViews.FormatGuildInformation(Context.Guild, weekTotal, monthTotal, channelTotal);
            await ReplyAsync(embed: embed);
        }
    }
}