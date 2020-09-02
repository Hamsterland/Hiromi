using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord;
using Hiromi.Data.Models.Tags;

namespace Hiromi.Services.Tags
{
    public static class TagViews
    {
        public static Embed FormatTagInfo(IUser author, IUser owner, TagSummary tag)
        {
            return new EmbedBuilder()
                .WithColor(Constants.DefaultEmbedColour)
                .AddField("Name", tag.Name)
                .AddField("Uses", tag.Uses)
                .AddField("Owner", owner)
                .AddField("Author", author)
                .Build();
        }

        public static Embed FormatUserTags(IUser user, IEnumerable<TagSummary> tags)
        {
            var sb = new StringBuilder()
                .AppendLine("```");

            foreach (var tag in tags)
            {
                sb.AppendLine(tag.Name);
            }

            sb.AppendLine("```");
            
            return new EmbedBuilder()
                .WithColor(Constants.DefaultEmbedColour)
                .WithAuthor(author =>
                    author
                        .WithName($"{user}'s Tags")
                        .WithIconUrl(user.GetAvatarUrl()))
                .WithDescription(sb.ToString())
                .Build();
        }
        
        public static Embed FormatSimilarTags(string name, IEnumerable<TagSummary> matches)
        {
            var embed = new EmbedBuilder()
                .WithColor(Constants.DefaultEmbedColour);

            var matchesList = matches.ToList();
            
            if (matchesList.Count > 0)
            {
                var builder = new StringBuilder()
                    .AppendLine($"No tag called \"{name}\" found. Did you mean?")
                    .AppendLine("```");

                foreach (var match in matchesList)
                {
                    builder.AppendLine(match.Name);
                }

                builder.AppendLine("```");
                embed.WithDescription(builder.ToString());
            }
            else
            {
                embed.WithDescription($"No tag called \"{name}\" found.");
            }

            return embed.Build();
        }
    }
}