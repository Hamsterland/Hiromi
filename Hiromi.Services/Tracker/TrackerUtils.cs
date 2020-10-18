using System;
using System.Globalization;
using System.Linq.Expressions;
using Discord.Commands;
using Google.Apis.Sheets.v4.Data;

namespace Hiromi.Services.Tracker
{
    public static class TrackerUtils
    {
        public static DateTime ParseDateClaimed(string date)
        {
            var formats = new[] { "M/d/yyyy", "MM/d/yyyy", "M/dd/yyyy", "MM/dd/yyyy" };
            // ReSharper disable once InconsistentNaming
            var enUS = new CultureInfo("en-US");

            var parsed = new DateTime();
            
            foreach (var format in formats)
            {
                var success = DateTime.TryParseExact(date, format, enUS, DateTimeStyles.None, out parsed);

                if (success)
                {
                    break;
                }
            }

            return parsed;
        }

        public static bool ParseFinalEdit(string final)
        {
            return final switch
            {
                "Yes" => true,
                _ => false
            };
        }

        public static Ready ParseReady(string ready)
        {
            return ready switch
            {
                "Yes" => Ready.Yes,
                "No" => Ready.No,
                "Final" => Ready.Final,
                _ => Ready.Unknown
            };
        }

        public static Synopsis ToSynopsis(this RowData row)
        {
            return new Synopsis
            {
                DateClaimed = ParseDateClaimed(row.Values[0].FormattedValue),
                SeriesTitle = row.Values[1].FormattedValue,
                ClaimType = Enum.Parse<ClaimType>(row.Values[2].FormattedValue),
                Claimant = row.Values[3].FormattedValue,
                Document = row.Values[4].Hyperlink,
                Final = ParseFinalEdit(row.Values[5].FormattedValue),

                E1 = new Claim
                {
                    DateClaimed = ParseDateClaimed(row.Values[6].FormattedValue),
                    Claimant = row.Values[7].FormattedValue
                },

                E2 = new Claim
                {
                    DateClaimed = ParseDateClaimed(row.Values[8].FormattedValue),
                    Claimant = row.Values[9].FormattedValue
                },

                Ready = ParseReady(row.Values[10].FormattedValue)
            };
        }
    }
}