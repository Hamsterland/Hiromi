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
            var pager = BuildSynopsesPager(synopses);
            await warning.DeleteAsync();
            await PagedReplyAsync(pager, new ReactionList());
        }

        private PaginatedMessage BuildSynopsesPager(List<Synopsis> synopses)
        {
            var pages = new List<EmbedPage>();
            
            pages
                .AddRange(synopses
                    .Batch(8)
                    .Select(x =>
                    {
                        var fieldBuilders = new List<EmbedFieldBuilder>();

                        foreach (var synopsis in x)
                        {
                            fieldBuilders.Add(new EmbedFieldBuilder
                            {
                                Name = "Claimed",
                                Value = synopsis.DateClaimed,
                                IsInline = true
                            });

                            fieldBuilders.Add(new EmbedFieldBuilder
                            {
                                Name = "Series",
                                Value = $"[{synopsis.SeriesTitle}]({synopsis.Document})",
                                IsInline = true
                            });

                            fieldBuilders.Add(new EmbedFieldBuilder
                            {
                                Name = "Type",
                                Value = $"{synopsis.ClaimType}",
                                IsInline = true
                            });
                        }

                        return new EmbedPage
                        {
                            TotalFieldMessage = fieldBuilders.Count != 1 ? "Synopses" : "Synopsis",
                            TotalFieldCountConstant = 1f / 3f,
                            Fields = fieldBuilders,
                            Color = Constants.DefaultEmbedColour
                        };
                    }));

            return new PaginatedMessage
            {
                Author = new EmbedAuthorBuilder()
                    .WithName(Context.User.ToString())
                    .WithIconUrl(Context.User.GetAvatarUrl()),
                
                Pages = pages
            };
        }
    }
}