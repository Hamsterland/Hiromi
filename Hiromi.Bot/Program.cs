using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Hiromi.Bot.TypeReaders;
using Hiromi.Data;
using Hiromi.Data.Models;
using Hiromi.Services.Commands;
using Hiromi.Services.Core;
using Hiromi.Services.Eval;
using Hiromi.Services.Tracker;
using Hiromi.Services.Listeners;
using Hiromi.Services.Listeners.Log;
using Hiromi.Services.Reminders;
using Hiromi.Services.Stats;
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
                    .MinimumLevel.Verbose()
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
                                     GatewayIntents.GuildMessageReactions | GatewayIntents.GuildPresences,
                    
                    LogLevel = LogSeverity.Info
                });
                
                var commandService = new CommandService(new CommandServiceConfig
                {
                    LogLevel = LogSeverity.Debug,
                    DefaultRunMode = RunMode.Sync,
                    CaseSensitiveCommands = false,
                    IgnoreExtraArgs = false,
                });
                
                collection
                    .AddMediatR(typeof(StartupService).Assembly)
                    .AddSingleton(discordSocketClient)
                    .AddSingleton(provider =>
                    {
                        commandService.AddTypeReader<CommandInfo>(new CommandTypeReader());
                        commandService.AddTypeReader<ModuleInfo>(new ModuleTypeReader());
                        commandService.AddTypeReader<ReminderSummary>(new ReminderTypeReader());
                        commandService.AddModulesAsync(Assembly.GetEntryAssembly(), provider);
                        return commandService;
                    })
                    .AddDbContext<HiromiContext>(options => options.UseNpgsql(configuration["Postgres:Connection"]))
                    .AddHostedService<StartupService>()
                    .AddHostedService<DiscordSocketListener>()
                    .AddSingleton<DiscordLogListener>()
                    .AddSingleton<InteractiveService>()
                    .AddSingleton<ICommandStoreService, CommandStoreService>()
                    .AddScoped<ICommandToggleService, CommandToggleService>()
                    .AddScoped<ITagService, TagService>()
                    .AddScoped<IReminderService, ReminderService>()
                    .AddScoped<IGuildStatsService, GuildStatsService>()
                    .AddScoped<ITrackerService, TrackerService>()
                    .AddScoped<IEvalService, EvalService>();
            })
            .RunConsoleAsync();
    }
}