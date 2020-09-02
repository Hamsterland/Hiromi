using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Hiromi.Data;
using Hiromi.Services.Stats;

namespace Hiromi.Bot.Modules
{
    [Name("Information")]
    [Summary("Some interesting information")]
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        private readonly IGuildStatsService _guildStatsService;

        public InfoModule(IGuildStatsService guildStatsService)
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

            var channelTotal = await _guildStatsService.GetMostActiveChannelAndMessageCountAsync(
                StatisticsSource.Guild,
                TimeSpan.FromDays(7),
                Context.Guild.Id);

            var embed = GuildStatsViews.FormatGuildInformation(Context.Guild, weekTotal, monthTotal, channelTotal);
            await ReplyAsync(embed: embed);
        }

        [Command("userinfo")]
        [Alias("info")]
        [Summary("Shows user information")]
        public async Task UserInfo()
        {
            var weekTotal = await _guildStatsService.GetMessageCountAsync(
                StatisticsSource.User,
                TimeSpan.FromDays(7),
                Context.Guild.Id,
                Context.User.Id);

            var monthTotal = await _guildStatsService.GetMessageCountAsync(
                StatisticsSource.User,
                TimeSpan.FromDays(30),
                Context.Guild.Id,
                Context.User.Id);

            var channelTotal = await _guildStatsService.GetMostActiveChannelAndMessageCountAsync(
                StatisticsSource.User,
                TimeSpan.FromDays(7),
                Context.Guild.Id,
                Context.User.Id);

            var lastMessage = await _guildStatsService.GetSecondLastMessageFromUserAsync(Context.Guild.Id, Context.User.Id);

            var embed = GuildStatsViews.FormatUserInformation(
                Context.User as SocketGuildUser,
                Context.Guild,
                weekTotal,
                monthTotal,
                channelTotal,
                lastMessage.TimeSent);

            await ReplyAsync(embed: embed);
        }
    }
}