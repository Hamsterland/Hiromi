using System.Collections.Generic;
using System.Linq;
using Discord;
using Hiromi.Data.Models;
using Humanizer;

namespace Hiromi.Services.Reminders
{
    public static class ReminderViews
    {
        private const int _precision = 2;
        
        public static Embed FormatReminder(Reminder reminder, IGuild guild)
        {
            var duration = reminder
                .Duration
                .Humanize(_precision);
            
            return new EmbedBuilder()
                .WithColor(Constants.DefaultEmbedColour)
                .WithAuthor(author => author
                    .WithName($"Reminder From {guild.Name}")
                    .WithIconUrl(guild.IconUrl))
                .AddField("Content", reminder.Message)
                .WithFooter($"Scheduled {duration} ago")
                .Build();
        }

        public static Embed FormatActiveReminders(IEnumerable<ReminderSummary> reminders, IUser user)
        {
            var remindersList = reminders.ToList();

            var embed = new EmbedBuilder()
                .WithColor(Constants.DefaultEmbedColour)
                .WithAuthor(author => author
                    .WithName($"{user.Username}'s Reminders")
                    .WithIconUrl(user.GetAvatarUrl()));

            if (remindersList.Count > 0)
            {
                foreach (var reminder in remindersList)
                {
                    var title = $"Id: {reminder.Id}";
                    
                    var remaining = ReminderUtilities
                        .GetRemainingTime(reminder)
                        .Humanize(_precision);

                    var description = $"In {remaining}";
                    embed.AddField(title, description);
                }    
            }
            else
            {
                embed.WithDescription("You have no active reminders.");
            }
            
            return embed.Build();
        }

        public static Embed FormarReminderInfo(ReminderSummary reminder)
        {
            var invoked = reminder
                .TimeInvoked
                .ToString("D");
            
            var elapsed = ReminderUtilities
                .GetElapsedTime(reminder)
                .Humanize(_precision);

            var remaining = ReminderUtilities
                .GetRemainingTime(reminder)
                .Humanize(_precision);
            
            return new EmbedBuilder()
                .WithColor(Constants.DefaultEmbedColour)
                .WithTitle("Reminder Information")
                .AddField("Id", reminder.Id)
                .AddField("Message", reminder.Message)
                .AddField("Invoked", invoked)
                .AddField("Elapsed", elapsed)
                .AddField("Remaining", remaining)
                .Build();
        }
    }
}