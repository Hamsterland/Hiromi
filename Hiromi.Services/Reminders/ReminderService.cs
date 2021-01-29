using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Hiromi.Data;
using Hiromi.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Hiromi.Services.Reminders
{
    public class ReminderService : IReminderService
    {
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly HiromiContext _hiromiContext;

        public ReminderService(
            DiscordSocketClient discordSocketClient, 
            HiromiContext hiromiContext)
        {
            _discordSocketClient = discordSocketClient;
            _hiromiContext = hiromiContext;
        }

        private readonly ConcurrentDictionary<int, Timer> _reminders = new ConcurrentDictionary<int, Timer>();

        public async Task CreateReminderAsync(ulong userId, ulong guildId, string message, TimeSpan duration)
        {
            var reminder = new Reminder
            {
                UserId = userId,
                GuildId = guildId,
                Message = message,
                TimeInvoked = DateTime.Now,
                Duration = duration
            };

            _hiromiContext.Add(reminder);
            await _hiromiContext.SaveChangesAsync();
            
            var timer = new Timer(async _ => await HandleReminderCallbackAsync(reminder), 
                null, 
                reminder.Duration, 
                Timeout.InfiniteTimeSpan);
            
            _reminders.TryAdd(reminder.Id, timer);
        }
        
        public async Task DeleteReminderAsync(int id)
        {
            var reminder = await _hiromiContext
                .Reminders
                .FindAsync(id);

            reminder.Completed = true;
            await _hiromiContext.SaveChangesAsync();

            _reminders.TryRemove(reminder.Id, out _);
        }
        
        public async Task HandleReminderCallbackAsync(Reminder reminder)
        {
            var user = _discordSocketClient.GetUser(reminder.UserId);
            var guild = _discordSocketClient.GetGuild(reminder.GuildId);
            await user?.SendMessageAsync(embed: ReminderViews.FormatReminder(reminder, guild));
            await DeleteReminderAsync(reminder.Id);
        }

        public async Task<IEnumerable<ReminderSummary>> GetActiveReminders(ulong userId)
        {
            return await _hiromiContext
                .Reminders
                .Where(x => x.UserId == userId)
                .Where(x => !x.Completed)
                .Select(ReminderSummary.FromEntityProjection)
                .ToListAsync();
        }
    
        public async Task<ReminderSummary> GetActiveReminder(ulong userId, int id)
        {
            var reminders = await GetActiveReminders(userId);
            return reminders.FirstOrDefault(x => x.Id == id);
        }

        public Task CacheReminders()
        {
            var reminders = _hiromiContext
                .Reminders
                .Where(x => !x.Completed);

            foreach (var reminder in reminders)
            {
                var timer = new Timer(async _ => await HandleReminderCallbackAsync(reminder),
                    null,
                    reminder.RemainingTime,
                    Timeout.InfiniteTimeSpan);

                _reminders.TryAdd(reminder.Id, timer);
            }

            return Task.CompletedTask;
        }
    }
}