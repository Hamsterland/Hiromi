using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Hiromi.Services.MyAnimeList;

namespace Hiromi.Bot.Modules
{
    [Name("MyAnimeList")]
    [Summary("MALLLLL")]
    public class MALModule : ModuleBase<SocketCommandContext>
    {
        private readonly MALStatusService _malStatusService;

        public MALModule(MALStatusService malStatusService)
        {
            _malStatusService = malStatusService;
        }

        [Command("malstatus")]
        [Summary("iS MaL DoWn?")]
        public async Task Status()
        {
            var status = _malStatusService.IsUp;

            var embed = new EmbedBuilder()
                .WithColor(new Color(46, 81, 162))
                .WithAuthor(author =>
                    author
                        .WithName("MyAnimeList Status")
                        .WithIconUrl("https://i.imgur.com/f7iSzJu.jpg"));

            if (status)
            { 
                embed.AddField("Up 💚", "Hiromi can connect to MAL from New Jersey");
            }
            else
            {
                embed.AddField("Down 💔", "Hiromi cannot connect to MAL from New Jersey");
            }

            await ReplyAsync(embed: embed.Build());
        }
    }
}