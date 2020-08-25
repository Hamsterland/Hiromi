using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hiromi.Data.Models;

namespace Hiromi.Services.Reminders
{
    public interface IReminderService
    {
        Task CreateReminderAsync(ulong userId, ulong guildId, string message, TimeSpan duration);
        Task DeleteReminderAsync(int id);
        Task HandleReminderCallbackAsync(Reminder reminder);
        Task<IEnumerable<ReminderSummary>> GetActiveReminders(ulong userId);
        Task<ReminderSummary> GetActiveReminder(ulong userId, int id);
    }
}