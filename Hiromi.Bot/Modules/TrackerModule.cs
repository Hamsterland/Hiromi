using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Hiromi.Services;
using Hiromi.Services.Tracker;

namespace Hiromi.Bot.Modules
{
    [Name("Tracker")]
    [Summary("Rewrite activity stats")]
    public class TrackerModule : ModuleBase<SocketCommandContext>
    {
        private readonly ITrackerService _trackerService;

        public TrackerModule(ITrackerService trackerService)
        {
            _trackerService = trackerService;
        }

        [Command("activity")]
        [Summary("Retrieves a user's activity")]
        public async Task Activity(string username)
        {
            var activity = await _trackerService.GetUserActivity(username);

            var claims = $@"
Anime: {activity.ClaimDistribution.Anime} ({activity.ClaimDistribution.AnimePercentage:F}%)
Manga: {activity.ClaimDistribution.Manga} ({activity.ClaimDistribution.MangaPercentage:F}%)
Novel: {activity.ClaimDistribution.Novel} ({activity.ClaimDistribution.NovelPercentage:F}%)
";
            
            var embed = new EmbedBuilder()
                .WithColor(Constants.DefaultEmbedColour)
                .WithTitle($"{username}'s Activity")
                .AddField("In Progress", $"Writes: {activity.ProgressActivity.Writes}\nEdits: {activity.ProgressActivity.Edits}", true)
                .AddField("Archived", $"Writes: {activity.ArchiveActivity.Writes}\nEdits: {activity.ArchiveActivity.Edits}", true)
                .AddField("Coordinator Edits", activity.ArchiveActivity.CoordinatorEdits, true)
                .AddField("Total", $"Writes: {activity.TotalWrites}\nEdits: {activity.TotalEdits}", true)
                .AddField("Written Claim Distribution", claims)
                .Build();

            await ReplyAsync(embed: embed);
        }
    }
}