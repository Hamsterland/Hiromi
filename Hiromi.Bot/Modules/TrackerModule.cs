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
using Serilog;

namespace Hiromi.Bot.Modules
{
    [Name("Tracker")]
    [Summary("Interact with the Tracker")]
    public class TrackerModule : InteractiveBase
    {
        private readonly ITrackerService _trackerService;
        private readonly ILogger _logger;

        public TrackerModule(ITrackerService trackerService, ILogger logger)
        {
            _trackerService = trackerService;
            _logger = logger;
        }

        [Command("synopses", RunMode = RunMode.Async)]
        [Summary("Shows current synopses.")]
        public async Task Synopses(string username)
        {
            try
            {
                var warning = await ReplyAsync($"{Context.User.Mention} I am querying the Tracker, this may take a moment.");
                var synopses = await _trackerService.GetUserSynopsesAsync(username);
                
                var pager = TrackerViews.BuildSynopsesPager(synopses, username);
                var reactions = pager.Pages.Count() == 1 ? default : new ReactionList();

                await warning.DeleteAsync();
                await PagedReplyAsync(pager, reactions);
            }
            catch (Exception)
            {
                await ReplyAsync($"{Context.User.Mention} I could not find any synopses written by the user \"{username}\"");
            }
        }
    }
}