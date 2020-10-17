using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Hiromi.Services.Commands;
using Hiromi.Services.Reminders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Hiromi.Services.Core
{
    public class StartupService : IHostedService
    {
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly IConfiguration _configuration;
        private readonly ICommandStoreService _commandStoreService;
        private readonly ILogger _logger;
        private readonly IReminderService _reminderService;

        public StartupService(
            DiscordSocketClient discordSocketClient, 
            IConfiguration configuration,
            ICommandStoreService commandStoreService, 
            ILogger logger, 
            IReminderService reminderService)
        {
            _discordSocketClient = discordSocketClient;
            _configuration = configuration;
            _commandStoreService = commandStoreService;
            _logger = logger;
            _reminderService = reminderService;
        }
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            string token;
            
#if DEBUG
            token = _configuration["Discord:Tokens:Development"];
#else
            token = _configuration["Discord:Tokens:Production"];
#endif

            if (string.IsNullOrWhiteSpace(token))
            {
                _logger.Fatal("The bot token was not found.");
                return;
            }
            
            try
            {
                await _commandStoreService.LoadEnabledCommands();
                await _discordSocketClient.LoginAsync(TokenType.Bot, token);
                await _discordSocketClient.StartAsync();
                await _reminderService.CacheReminders();
            }
            catch (Exception e)
            {
                _logger.Fatal(e.Message);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _discordSocketClient.LogoutAsync();
            await _discordSocketClient.StopAsync();
        }
    }
}