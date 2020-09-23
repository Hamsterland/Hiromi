using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;

namespace Hiromi.Services.Tracker
{
    public class TrackerService : ITrackerService
    {
        private const string ApplicationName = "Hiromi";

        public ClaimDistribution GetClaimDistribution(Tracker tracker, string username)
        {
            var anime = 0;
            var manga = 0;
            var novel = 0;

            foreach (var row in tracker.Progress.Skip(1))
            {
                AppendClaimInformation(row); 
            }
            
            foreach (var row in tracker.Archive.Skip(1))
            {
                AppendClaimInformation(row);
            }

            void AppendClaimInformation(IList<object> row)
            {
                if (row.Count > 1 && row[3].ToString().ToLower() == username)
                {
                    switch (row[2])
                    {
                        case "Anime":
                        {
                            anime++;
                            break;
                        }
                        case "Manga":
                        {
                            manga++;
                            break;
                        }
                        case "Novel":
                        {
                            novel++;
                            break;
                        }
                    }
                }
            }
            
            return new ClaimDistribution(anime, manga, novel);
        }
        
        public ProgressActivity GetProgressActivity(Tracker tracker, string username)
        {
            var writes = 0;
            var edits = 0;

            foreach (var row in tracker.Progress.Skip(1))
            {
                if (row.Count > 1)
                {
                    if (row[3].ToString().ToLower() == username)
                    {
                        writes++;
                    }

                    if (row[7].ToString().ToLower() == username || row[9].ToString().ToLower() == username)
                    {
                        edits++;
                    }
                }
            }

            return new ProgressActivity(writes, edits);
        }

        public ArchiveActivity GetArchiveActivity(Tracker tracker, string username)
        {
            var writes = 0;
            var edits = 0;
            var coordinatorEdits = 0;    
            
            foreach (var row in tracker.Archive.Skip(1))
            {
                if (row.Count > 1)
                {
                    if (row[3].ToString().ToLower() == username)
                    {
                        writes++;
                    }
                
                    if (row[7].ToString().ToLower()  == username || row[9].ToString().ToLower()  == username)
                    {
                        edits++;
                    }

                    if (row[11].ToString().ToLower() == username)
                    {
                        coordinatorEdits++;
                    }
                }
            }

            return new ArchiveActivity(writes, edits, coordinatorEdits);
        }
        
        public async Task<TrackerStatistics> GetUserActivity(string username)
        {
            var tracker = await GetTrackerAsync();
            username = username.ToLower();

            if (tracker.Progress != null && tracker.Progress.Count > 0)
            {
                var progressActivity = GetProgressActivity(tracker, username);
                var archiveActivity = GetArchiveActivity(tracker, username);
                var claimDistribution = GetClaimDistribution(tracker, username);
                return new TrackerStatistics(progressActivity, archiveActivity, claimDistribution);
            }

            return null;
        }
        
        public async Task<Tracker> GetTrackerAsync()
        {
            await using var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read);
            var credential = await GoogleCredential.FromStreamAsync(stream, CancellationToken.None);
            credential = credential.CreateScoped();
            
            var service = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });
            
            const string spreadsheetId = "1wbIqigHMGFWZebum8mQjSL5eQzSAXuWZS-8ewbFX4PI";
            const string inProgress = "Synopses In Progress";
            const string archive = "Archive";
            
            var progressRequest = service.Spreadsheets.Values.Get(spreadsheetId, inProgress);
            var progressResponse = await progressRequest.ExecuteAsync();
            var progressValues = progressResponse.Values;

            var archiveRequest = service.Spreadsheets.Values.Get(spreadsheetId, archive);
            var archiveResponse = await archiveRequest.ExecuteAsync();
            var archiveValues = archiveResponse.Values;

            return new Tracker(progressValues, archiveValues);
        }
    }
}