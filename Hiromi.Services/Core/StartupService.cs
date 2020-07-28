using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Hiromi.Services.Commands;
using Hiromi.Services.MyAnimeList;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Hiromi.Services.Core
{
    public class StartupService : IHostedService
    {
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly MALStatusService _malStatusService;
        private readonly ICommandStoreService _commandStoreService;

        public StartupService(
            DiscordSocketClient discordSocketClient, 
            IConfiguration configuration,
            ILogger logger, 
            MALStatusService malStatusService, 
            ICommandStoreService commandStoreService)
        {
            _discordSocketClient = discordSocketClient;
            _configuration = configuration;
            _logger = logger;
            _malStatusService = malStatusService;
            _commandStoreService = commandStoreService;
        }
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var token = _configuration["Discord:Token"];

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
            }
            catch (Exception e)
            {
                _logger.Fatal(e.Message);
            }
            
            _malStatusService.Start();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _discordSocketClient.LogoutAsync();
            await _discordSocketClient.StopAsync();
        }
    }
}