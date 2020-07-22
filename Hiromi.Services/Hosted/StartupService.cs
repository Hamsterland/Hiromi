using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Hiromi.Services.Hosted
{
    public class StartupService : IHostedService
    {
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public StartupService(
            DiscordSocketClient discordSocketClient, 
            IConfiguration configuration,
            ILogger logger)
        {
            _discordSocketClient = discordSocketClient;
            _configuration = configuration;
            _logger = logger;
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
                await _discordSocketClient.LoginAsync(TokenType.Bot, token);
                await _discordSocketClient.StartAsync();
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