using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Hiromi.Bot.Extensions;
using Hiromi.Services.Eval;

namespace Hiromi.Bot.Modules
{
    [Name("Evaluation")]
    [Summary("Evaluate C# Code")]
    public class EvalModule : ModuleBase<SocketCommandContext>
    {
        private readonly IEvalService _evalService;

        public EvalModule(IEvalService evalService)
        {
            _evalService = evalService;
        }

        [Command("eval")]
        [Summary("Runs code")]
        public async Task Eval([Remainder] string code)
        {
            var message = await Context.Channel
                .SendMessageAsync(embed: new EmbedBuilder()
                    .WithTitle("REPL Executing")
                    .WithUserAsAuthor(Context.User)
                    .WithColor(Color.LightOrange)
                    .WithDescription($"Compiling and Executing [your code]({Context.Message.GetJumpUrl()})...")
                    .Build());

            var result = await _evalService.EvaluateAsync(code);
            
            if (result.Diagnostics is null)
            {
                var successEmbed = new EmbedBuilder()
                    .WithTitle("REPL Result: Success")
                    .WithUserAsAuthor(Context.User)
                    .WithColor(Color.Green)
                    .AddField("Code", $"```cs\n{code}\n```")
                    .AddField("Result", $"{result.Result}")
                    .Build();

                await message.ModifyAsync(x =>
                {
                    x.Content = string.Empty;
                    x.Embed = successEmbed;
                });
            }
            else
            {
                var exceptionsBuilder = new StringBuilder();

                foreach (var diagnostic in result.Diagnostics)
                {
                    exceptionsBuilder.AppendLine($"- {diagnostic}");
                }

                var exceptions = exceptionsBuilder.ToString();

                var failureEmbed = new EmbedBuilder()
                    .WithTitle("REPL Result: Failure")
                    .WithUserAsAuthor(Context.User)
                    .WithColor(Color.Red)
                    .AddField("Code", $"```cs\n{code}\n```")
                    .AddField("Exceptions", $"```diff\n{exceptions}\n```")
                    .Build();
                
                await message.ModifyAsync(x =>
                {
                    x.Content = string.Empty;
                    x.Embed = failureEmbed;
                });
            }
            
        }
    }
}