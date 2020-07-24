using System;
using System.Net;
using System.Threading;

namespace Hiromi.Services.MyAnimeList
{
    public class MALStatusService
    {
        public bool IsUp;
        private const string Url = "https://myanimelist.net/";
        
        public void Start()
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
                            IsUp = true;
                        }
                    }
                }
                catch (WebException)
                {
                    IsUp = false;
                }
            }, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        }
    }
}