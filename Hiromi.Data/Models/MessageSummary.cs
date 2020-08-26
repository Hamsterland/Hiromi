using System;
using System.Linq.Expressions;

namespace Hiromi.Data.Models
{
    public class MessageSummary
    {
        public ulong MessageId { get; set; }
  
        public ulong UserId { get; set; }
        
        public ulong ChannelId { get; set; }
        
        public ulong GuildId { get; set; }

        public DateTime TimeSent { get; set; }
        
        public static Expression<Func<Message, MessageSummary>> FromEntityProjection = message =>
            new MessageSummary
            {
                MessageId = message.MessageId,
                UserId = message.UserId,
                ChannelId = message.ChannelId,
                GuildId = message.GuildId,
                TimeSent = message.TimeSent
            };
    }
}