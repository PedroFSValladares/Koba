using Koba.Contracts;
using Koba.Contracts.Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Koba.Application.Bot;

public class Worker : BackgroundService
{
    private readonly IDiscordGateway _gateway;
    private readonly ILogger<Worker> _logger;

    public Worker(IDiscordGateway gateway, ILogger<Worker> logger)
    {
        _gateway = gateway;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _logger.BeginScope("[BOT INITIALIZATION]");
        
        _logger.LogInformation("Inciando o Gateway.");
        await _gateway.Init(stoppingToken);
        
        _logger.LogInformation("Configurando HelloEvent.");
        _gateway.Subscribe<HelloEvent>(e =>
        {
            _logger.LogInformation("HelloEvent recebido.");
            return Task.CompletedTask;
        }, stoppingToken);
        
        await Task.Delay(-1, stoppingToken);
    }
}