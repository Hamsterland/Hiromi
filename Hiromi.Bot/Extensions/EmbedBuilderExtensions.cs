﻿using Discord;

namespace Hiromi.Bot.Extensions
{
    public static class EmbedBuilderExtensions
    {
        public static EmbedBuilder WithUserAsAuthor(this EmbedBuilder builder, IUser user)
        {
            return builder
                .WithAuthor(author => author
                    .WithName(user.Username)
                    .WithIconUrl(user.GetAvatarUrl()));
        }
    }
}