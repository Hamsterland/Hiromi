using System;
using System.Collections.Generic;
using System.Linq;
using Discord;
using Discord.Addons.Interactive;
using MoreLinq;

namespace Hiromi.Services.Tracker
{
    public static class TrackerViews
    {
        private static readonly Color TrackerColour = new Color(15, 157, 88);

        public static PaginatedMessage BuildMatchedSynopsesPager(IEnumerable<Synopsis> synopses, string query)
        {
            var pages = new List<EmbedPage>();
            
            pages
                .AddRange(synopses
                    .Take(40)
                    .Batch(8)
                    .Select(x =>
                    {
                        var fieldBuilders = new List<EmbedFieldBuilder>();

                        foreach (var synopsis in x)
                        {
                            fieldBuilders.Add(new EmbedFieldBuilder
                            {
                                Name = "Writer",
                                Value = synopsis.Claimant,
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
                            
                            fieldBuilders.Add(new EmbedFieldBuilder
                            {
                                Name = "Editor 1",
                                Value = synopsis.E1!.Claimant ?? "None",
                                IsInline = true
                            });
                            
                            fieldBuilders.Add(new EmbedFieldBuilder
                            {
                                Name = "Editor 2",
                                Value = synopsis.E2!.Claimant ?? "None",
                                IsInline = true
                            });
                            
                            fieldBuilders.Add(new EmbedFieldBuilder
                            {
                                Name = "Ready?",
                                Value = synopsis.Ready,
                                IsInline = true
                            });
                        }

                        return new EmbedPage
                        {
                            TotalFieldMessage = fieldBuilders.Count != 1 ? "Synopses" : "Synopsis",
                            TotalFieldCountConstant = 1f / 3f,
                            Fields = fieldBuilders,
                            Color = TrackerColour
                        };
                    }));
            
            return new PaginatedMessage
            {
                Author = new EmbedAuthorBuilder()
                    .WithName($"Synopses Matching \"{query}\"")
                    .WithIconUrl("https://i.imgur.com/cqkQDxC.png"),
                
                Options = new PaginatedAppearanceOptions
                {
                    DisplayInformationIcon = false,
                    Timeout = TimeSpan.FromSeconds(60)
                },
                
                Pages = pages,
            };
        }
        
        public static PaginatedMessage BuildSynopsesPager(IEnumerable<Synopsis> synopses, string username)
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
                                Value = synopsis.DateClaimed.ToString("d"),
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
                            Color = TrackerColour
                        };
                    }));

            return new PaginatedMessage
            {
                Author = new EmbedAuthorBuilder()
                    .WithName($"{username}'s Synopses")
                    .WithIconUrl("https://i.imgur.com/cqkQDxC.png"),
                
                Options = new PaginatedAppearanceOptions
                {
                    DisplayInformationIcon = false,
                    Timeout = TimeSpan.FromSeconds(30)
                },
                
                Pages = pages,
            };
        }
    }
}