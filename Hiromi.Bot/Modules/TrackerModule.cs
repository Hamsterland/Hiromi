using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Hiromi.Services;
using Hiromi.Services.Google;

namespace Hiromi.Bot.Modules
{
    [Name("Tracker")]
    [Summary("Rewrite activity stats")]
    public class TrackerModule : ModuleBase<SocketCommandContext>
    {
        private readonly IGoogleService _googleService;

        public TrackerModule(IGoogleService googleService)
        {
            _googleService = googleService;
        }

        [Command("activity")]
        [Summary("Retrieves a user's activity")]
        public async Task Activity(string username)
        {
            var activity = await _googleService.GetUserActivity(username);

            var embed = new EmbedBuilder()
                .WithColor(Constants.DefaultEmbedColour)
                .WithTitle($"{username}'s Activity")
                .AddField("In Progress", $"Writes: {activity.ProgressWrites}\nEdits: {activity.ProgressEdits}")
                .AddField("Archived", $"Writes: {activity.ArchiveWrites}\nEdits: {activity.ArchiveEdits}")
                .AddField("Total", $"Writes: {activity.TotalWrites}\nEdits: {activity.TotalEdits}")
                .Build();

            await ReplyAsync(embed: embed);
        }
    }
}