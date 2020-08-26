using System.Text;
using Discord;

namespace Hiromi.Services.Stats
{
    public static class GuildStatsViews
    {
        public static Embed FormatGuildInformation(IGuild guild, int week, int month)
        {
            var description = new StringBuilder()
                .AppendLine("**Statistics**")
                .AppendLine($"Last 7 Days: {week} messages")
                .AppendLine($"Last 30 Days: {month} messages")
                .ToString();

            return new EmbedBuilder()
                .WithColor(Constants.DefaultEmbedColour)
                .WithAuthor(author => author
                    .WithName($"{guild.Name} Information")
                    .WithIconUrl(guild.IconUrl))
                .WithDescription(description)
                .Build();
        }
    }
}