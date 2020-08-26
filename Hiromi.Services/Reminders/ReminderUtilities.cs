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
    }
}