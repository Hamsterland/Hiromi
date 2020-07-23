using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Hiromi.Services.MyAnimeList
{
    public class MALStatusService
    {
        public bool MalStatus;
        private const string Url = "https://myanimelist.net/";

        public MALStatusService()
        {
            StartAsync();
        }
        
        public void StartAsync()
        {
            var timer = new Timer(e =>
            {
                var request = (HttpWebRequest) WebRequest.Create(Url);
                request.Timeout = 15000;
                request.Method = "HEAD"; 
                
                try
                {
                    using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            MalStatus = true;
                        }
                    }
                }
                catch (WebException)
                {
                    MalStatus = false;
                }
            }, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        }
    }
}