using System;
using System.Globalization;

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
    }
}