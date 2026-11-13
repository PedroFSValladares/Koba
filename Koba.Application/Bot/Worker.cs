using Koba.Contracts;
using Koba.Contracts.Dtos;
using Koba.Contracts.Events;
using Koba.Contracts.Interfaces;
using Koba.Contracts.Payloads;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Koba.Application.Bot;

public class Worker : BackgroundService
{
    private readonly IDiscordGateway _gateway;
    private readonly IDiscordService _discordService;
    private readonly ILogger<Worker> _logger;

    public Worker(IDiscordGateway gateway, IDiscordService discordService, ILogger<Worker> logger)
    {
        _gateway = gateway;
        discordService = discordService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _logger.BeginScope("[BOT INITIALIZATION]");
        
        _logger.LogInformation("Obtendo informações para conexão junto ao gateway.");
        var discordResponse = await _discordService.GetDiscordSocketUrlAsync(stoppingToken);
        
        _logger.LogInformation("Inciando o Gateway.");
        int totalAllowedStarts = discordResponse.SessionStartLimitInfo.TotalAllowedStarts,
            remainingAllowedStarts = discordResponse.SessionStartLimitInfo.RemainingAllowedStarts,
            milissecondsToResetStarts = discordResponse.SessionStartLimitInfo.MilissecondsToResetStarts,
            maxConcurrency = discordResponse.SessionStartLimitInfo.MaxConcurrency;
        var gatewayConfig = new GatewayInitializationParams
        {
            cancellationToken =  stoppingToken,
            sessionStartInfo = SessionStartLimit.Create(totalAllowedStarts, remainingAllowedStarts, milissecondsToResetStarts, maxConcurrency),
            Shards = [0, discordResponse.Shards],
            SocketUrl =  discordResponse.SocketUrl
        };
        
        await _gateway.Init(gatewayConfig);
        
        _logger.LogInformation("Configurando HelloEvent.");
        _gateway.Subscribe<HelloEvent>(e =>
        {
            _logger.LogInformation("HelloEvent recebido.");
            return Task.CompletedTask;
        }, stoppingToken);
        
        await Task.Delay(-1, stoppingToken);
    }
}