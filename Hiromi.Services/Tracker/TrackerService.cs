using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace Hiromi.Services.Tracker
{
    public class TrackerService : ITrackerService
    {
        public async Task<List<Synopsis>> GetUserSynopses(string username)
        {
            var synopses = new List<Synopsis>();
            var tracker = await GetTrackerAsync();

            foreach (var sheet in tracker.Sheets)
            {
                var data = sheet.Data;
                
                var writes = data
                    .Select(x => x.RowData)
                    .First()
                    .Where(x => x.Values.Count >= 11)
                    .Where(x => x.Values[3].FormattedValue == username);
                
                static bool ParseFinal(string text)
                {
                    return text switch
                    {
                        "Yes" => true,
                        _ => false
                    };
                }

                var result = writes
                    .Select(write => write.Values)
                    .Select(cells => new Synopsis
                    {
                        DateClaimed = cells[0].FormattedValue,
                        SeriesTitle = cells[1].FormattedValue,
                        SeriesHyperlink = cells[1].Hyperlink ?? "No Hyperlink",
                        ClaimType = Enum.Parse<ClaimType>(cells[2].FormattedValue),
                        Claimant = cells[3].FormattedValue,
                        Document = cells[4].Hyperlink ?? "No Hyperlink",
                        Final = ParseFinal(cells[5].FormattedValue)
                    });

                synopses.AddRange(result);
            }
            
            // var progress = tracker.Sheets[0];
            // var data = progress.Data;
            //
            // var writes = data
            //     .Select(x => x.RowData)
            //     .First()
            //     .Where(x => x.Values[3].FormattedValue == username);
            //
            // static bool ParseFinal(string text)
            // {
            //     return text switch
            //     {
            //         "Yes" => true,
            //         _ => false
            //     };
            // }
            //
            // return writes
            //     .Select(write => write.Values)
            //     .Select(cells => new Synopsis
            //     {
            //         DateClaimed = cells[0].FormattedValue,
            //         SeriesTitle = cells[1].FormattedValue,
            //         SeriesHyperlink = cells[1].Hyperlink ?? "No Hyperlink",
            //         ClaimType = Enum.Parse<ClaimType>(cells[2].FormattedValue),
            //         Claimant = cells[3].FormattedValue,
            //         Document = cells[4].Hyperlink ?? "No Hyperlink",
            //         Final = ParseFinal(cells[5].FormattedValue)
            //     })
            //     .ToList();

            return synopses;
        }

        public async Task<Spreadsheet> GetTrackerAsync()
        {
            await using var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read);

            var credential = GoogleCredential
                .FromStream(stream)
                .CreateScoped();

            var sheetsService = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Hiromi"
            });
            
            const string spreadsheetId = "1wbIqigHMGFWZebum8mQjSL5eQzSAXuWZS-8ewbFX4PI";

            var request = sheetsService
                .Spreadsheets
                .Get(spreadsheetId);

            request.IncludeGridData = true;
            request.Ranges = new[] {"'Synopses In Progress'!A3:K", "'Archive'!A2:L"};

            return await request.ExecuteAsync();
        }
    }
}