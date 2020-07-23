using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using Hiromi.Bot.TypeReaders;
using Hiromi.Data;
using Hiromi.Services.Help;
using Hiromi.Services.Hosted;
using Hiromi.Services.Listeners;
using Hiromi.Services.Logging;
using Hiromi.Services.Tags;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using MediatR;

namespace Hiromi.Bot
{
    internal class Program
    {
        private static async Task Main(string[] args) => await Host.CreateDefaultBuilder(args)
            .UseSerilog((context, configuration) =>
            {
                configuration
                    .Enrich.FromLogContext()
                    .MinimumLevel.Information()
                    .WriteTo.Console(theme: SystemConsoleTheme.Literate);
            })
            .ConfigureAppConfiguration((context, builder) =>
            {
                builder
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddYamlFile("appsettings.yaml", false, true);
            })
            .ConfigureServices((context, collection) =>
            {
                var configuration = context.Configuration;
                var discordSocketClient = new DiscordSocketClient(new DiscordSocketConfig
                {
                    AlwaysDownloadUsers = true,
                    MessageCacheSize = 10000,
                    GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages |
                                     GatewayIntents.GuildMessageReactions | GatewayIntents.GuildVoiceStates | GatewayIntents.GuildPresences,
                    LogLevel = LogSeverity.Verbose
                });
                
                var commandService = new CommandService(new CommandServiceConfig
                {
                    LogLevel = LogSeverity.Verbose,
                    DefaultRunMode = RunMode.Sync,
                    CaseSensitiveCommands = false
                });

                collection
                    .AddMediatR(typeof(StartupService).Assembly)
                    .AddSingleton(discordSocketClient)
                    .AddSingleton(provider =>
                    {
                        commandService.AddTypeReader<CommandInfo>(new CommandTypeReader());
                        commandService.AddTypeReader<ModuleInfo>(new ModuleTypeReader());
                        commandService.AddModulesAsync(Assembly.GetEntryAssembly(), provider);
                        return commandService;
                    })
                    .AddDbContext<HiromiContext>(options => options.UseNpgsql(configuration["Postgres:Connection"]))
                    .AddHostedService<StartupService>()
                    .AddHostedService<DiscordSocketListener>()
                    .AddHostedService<PostCommandService>()
                    .AddSingleton<InteractiveService>()
                    .AddScoped<ITagService, TagService>()
                    .AddScoped<ILogChannelService, LogChannelService>()
                    .AddScoped<IHelpService, HelpService>();
            })
            .RunConsoleAsync();
    }
}