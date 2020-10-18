using System;

namespace Hiromi.Services.Tracker
{
    public class Synopsis : Claim
    {
        public string SeriesTitle { get; set; }
        public string SeriesHyperlink { get; set; }
        public ClaimType ClaimType { get; set; }
        public string Document { get; set; }
        public bool Final { get; set; }
        public Claim? E1 { get; set; }
        public Claim? E2 { get; set; }
        public Ready Ready { get; set; }
        public Claim? E3 { get; set; }
    }
}