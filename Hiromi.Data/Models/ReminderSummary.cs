using System;

namespace Hiromi.Data.Models
{
    public class ReminderSummary
    {
        public int Id { get; set; }

        public ulong UserId { get; set; }

        public ulong GuildId { get; set; }

        public string Message { get; set; }

        public TimeSpan Duration { get; set; }
    }
}