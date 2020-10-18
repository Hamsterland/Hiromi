using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Hiromi.Bot.Preconditions;
using Hiromi.Services;
using Hiromi.Services.Tracker;
using MoreLinq;

namespace Hiromi.Bot.Modules
{
    [Name("Tracker")]
    [Summary("Interact with the Tracker")]
    [RequireDeveloper]
    public class TrackerModule : InteractiveBase
    {
        private readonly ITrackerService _trackerService;

        public TrackerModule(ITrackerService trackerService)
        {
            _trackerService = trackerService;
        }

        [Command("synopses", RunMode = RunMode.Async)]
        [Summary("Shows current synopses.")]
        public async Task Synopses(string username)
        {
            var warning = await ReplyAsync($"{Context.User.Mention} I am querying the Tracker, this may take a moment.");
            var synopses = await _trackerService.GetUserSynopses(username);
            var pager = TrackerViews.BuildSynopsesPager(synopses, username);
            await warning.DeleteAsync();
            var reactions = pager.Pages.Count() == 1 ? default : new ReactionList();
            await PagedReplyAsync(pager, reactions);
        }
    }
}