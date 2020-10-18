using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Addons.Interactive;
using Discord.Commands;
using Hiromi.Services.Tracker;
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

        [Command("synopsis", RunMode = RunMode.Async)]
        [Alias("search")]
        [Summary("Looks for a synopsis in the Tracker. The search is not perfect so please be as precise as possible. I.e \"Code Geass Hangyaku\" vs \"Code Geass\". It is also case-sensitive.")]
        public async Task Synopsis([Remainder] string query)
        {
            try
            {
                var warning = await ReplyAsync($"{Context.User.Mention} I am querying the Tracker, this may take a moment.");
                var synopses = await _trackerService.GetSynopsesAsync(query);
                
                var pager = TrackerViews.BuildMatchedSynopsesPager(synopses, query);
                var reactions = pager.Pages.Count() == 1 ? default : new ReactionList();

                await warning.DeleteAsync();
                await PagedReplyAsync(pager, reactions);
            }
            catch (Exception)
            {
                await ReplyAsync($"{Context.User.Mention} I could not find any synopses matching the name \"{query}\"");
            }
        }
        
        [Command("synopses", RunMode = RunMode.Async)]
        [Summary("Shows current synopses")]
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