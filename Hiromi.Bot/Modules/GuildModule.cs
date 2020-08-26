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
            var weekTotal = await _guildStatsService.GetMessageCountAsync(
                TimeSpan.FromDays(7), 
                x => x.GuildId == Context.Guild.Id);

            var monthTotal = await _guildStatsService.GetMessageCountAsync(
                TimeSpan.FromDays(30), 
                x => x.GuildId == Context.Guild.Id);
            
            var channelTotal = await _guildStatsService.GetMostMessageCountByChannelAsync(
                Context.Guild.Id, 
                TimeSpan.FromDays(7));
            
            var embed = GuildStatsViews.FormatGuildInformation(Context.Guild, weekTotal, monthTotal, channelTotal);
            await ReplyAsync(embed: embed);
        }
    }
}