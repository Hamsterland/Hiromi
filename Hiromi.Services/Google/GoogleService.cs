﻿using System;    
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;

namespace Hiromi.Services.Google
{
    public class GoogleService : IGoogleService
    {
        private readonly string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        private const string ApplicationName = "Hiromi";

        public async Task<Activity> GetUserActivity(string user)
        {
            var tracker = await GetTrackerAsync();
            user = user.ToLower();

            if (tracker.Progress != null && tracker.Progress.Count > 0)
            {
                var progressWrites = 0;
                var progressEdits = 0;
                
                foreach (var row in tracker.Progress.Skip(1))
                {
                    if (row.Count > 1)
                    {
                        if (row[3].ToString().ToLower() == user)
                        {
                            progressWrites++;
                        }
                
                        if (row[7].ToString().ToLower()  == user || row[9].ToString().ToLower()  == user)
                        {
                            progressEdits++;
                        }
                    }
                }

                var archiveWrites = 0;
                var archiveEdits = 0;
                
                foreach (var row in tracker.Archive.Skip(1))
                {
                    if (row.Count > 1)
                    {
                        if (row[3].ToString().ToLower() == user)
                        {
                            archiveWrites++;
                        }
                
                        if (row[7].ToString().ToLower()  == user || row[9].ToString().ToLower()  == user)
                        {
                            archiveEdits++;
                        }
                    }
                }
                
                return new Activity(progressWrites, progressEdits, archiveWrites, archiveEdits);
            }

            return null;
        }
        
        public async Task<Tracker> GetTrackerAsync()
        {
            UserCredential credential;

            await using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                const string credPath = "token.json";
                
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                
                Console.WriteLine("Credential file saved to: " + credPath);
            }
            
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
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