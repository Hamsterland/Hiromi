using System;
using System.Threading.Tasks;
using Discord.Commands;
using Hiromi.Bot.Preconditions;
using Hiromi.Data.Models;
using Hiromi.Services.Reminders;
using Humanizer;

namespace Hiromi.Bot.Modules
{
    [Name("Reminder")]
    [Summary("Because you're too lazy to remember")]
    [RequireEnabledInChannel]
    public class ReminderModule : ModuleBase<SocketCommandContext>
    {
        private readonly IReminderService _reminderService;

        public ReminderModule(IReminderService reminderService)
        {
            _reminderService = reminderService;
        }

        [Command("remind")]
        [Summary("Schedules a reminder")]
        public async Task Remind(TimeSpan duration, [Remainder] string message)
        {
            var reminders = await _reminderService.GetActiveReminders(Context.User.Id);

            if (!ReminderUtilities.IsWithinMaxReminderLimit(reminders))
            {
                await ReplyAsync($"{Context.User.Mention} You cannot have more than {ReminderConstants.MaxRemindersPerUser} active reminders.");
                return;
            }
            
            await _reminderService.CreateReminderAsync(Context.User.Id, Context.Guild.Id, message, duration);
            await ReplyAsync($"{Context.User.Mention} I'll remind you in {duration.Humanize(2)}. Please keep direct messages open.");
        }

        [Command("reminders")]
        [Summary("Shows your active reminders")]
        public async Task Reminders()
        {
            var reminders = await _reminderService.GetActiveReminders(Context.User.Id);
            await ReplyAsync(embed: ReminderViews.FormatActiveReminders(reminders, Context.User));
        }

        [Command("reminder info")]
        [Summary("Shows reminder information")]
        public async Task Info(ReminderSummary reminder)
        {
            await ReplyAsync(embed: ReminderViews.FormarReminderInfo(reminder));
        }

        [Command("reminder delete")]
        [Summary("Deletes a reminder")]
        public async Task Delete(ReminderSummary reminder)
        {
            await _reminderService.DeleteReminderAsync(reminder.Id);
            await ReplyAsync($"{Context.User.Mention} Deleted reminder with Id {reminder.Id}.");
        }

        private async Task ReplyNotFound(int id)
        {
            await ReplyAsync($"{Context.User.Mention} You do not have an active reminder with Id {id}.");
        }
    }
}