using System;
using System.Collections.Generic;
using System.Linq;
using Hiromi.Data.Models;

namespace Hiromi.Services.Reminders
{
    public static class ReminderUtilities
    {
        public static bool IsWithinMaxReminderLimit(IEnumerable<ReminderSummary> reminders)
        {
            return reminders.Count() <= ReminderConstants.MaxRemindersPerUser;
        }

        public static TimeSpan GetRemainingTime(ReminderSummary reminder)
        {
            var endDate = reminder.TimeInvoked.Add(reminder.Duration);
            return endDate.Subtract(DateTime.Now);
        }

        public static TimeSpan GetElapsedTime(ReminderSummary reminder)
        {
            var remainingTime = GetRemainingTime(reminder);
            return reminder.Duration.Subtract(remainingTime);
        }
    }
}