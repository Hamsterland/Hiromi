using System;
using System.Linq.Expressions;

namespace Hiromi.Data.Models
{
    public class WelcomeResponseSummary
    {
        public ulong ChannelId { get; set; }
        public ulong GuildId { get; set; }
        public string Message { get; set; }
        
        public static readonly Expression<Func<WelcomeResponse, WelcomeResponseSummary>> FromEntityProjection = welcome =>
            new WelcomeResponseSummary
            {
               ChannelId = welcome.ChannelId,
               GuildId = welcome.GuildId,
               Message = welcome.Message
            };
    }
}