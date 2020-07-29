using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Hiromi.Data.Models.Channels;

namespace Hiromi.Data.Models
{
    public class ChannelSummary
    {
        public long Id { get; set; }
        
        public ulong GuildId { get; set; }
        
        public ulong ChannelId { get; set; }
        
        public List<string> Commands { get; set; } 
        
        public bool IsLogChannel { get; set; }

        public static readonly Expression<Func<Channel, ChannelSummary>> FromEntityProjection = channel =>
            new ChannelSummary
            {
                GuildId = channel.GuildId,
                ChannelId = channel.ChannelId,
                Commands = channel.Commands,
                IsLogChannel = channel.IsLogChannel
            };
    }
}