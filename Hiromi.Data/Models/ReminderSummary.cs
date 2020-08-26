using System;
using System.Linq.Expressions;
using Hiromi.Data.Models.Channels;

namespace Hiromi.Data.Models
{
    public class ReminderSummary
    {
        public int Id { get; set; }

        public ulong UserId { get; set; }

        public ulong GuildId { get; set; }

        public string Message { get; set; }

        public DateTime TimeInvoked { get; set; }
        
        public TimeSpan Duration { get; set; }

        public TimeSpan RemainingTime => TimeInvoked.Add(Duration).Subtract(DateTime.Now);

        public TimeSpan ElapsedTime => Duration.Subtract(RemainingTime);
        
        public static readonly Expression<Func<Reminder, ReminderSummary>> FromEntityProjection = reminder =>
            new ReminderSummary
            {
                Id = reminder.Id,
                UserId = reminder.UserId,
                GuildId = reminder.GuildId,
                Message = reminder.Message,
                TimeInvoked = reminder.TimeInvoked,
                Duration = reminder.Duration
            };
    }
}