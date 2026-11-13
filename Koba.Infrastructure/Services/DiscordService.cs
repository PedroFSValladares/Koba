using System.Text.Json;
using Koba.Contracts.Interfaces;
using Koba.Contracts.Payloads;
using Microsoft.Extensions.Logging;

namespace Koba.Infrastructure.Services;

public class DiscordService : IDiscordService
{
    private readonly DiscordServiceConfiguration _discordServiceConfiguration;

    private readonly HttpClient _httpClient;
    private readonly ILogger<DiscordService> _logger;

    public DiscordService(HttpClient httpClient, ILogger<DiscordService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<GetDiscordSocketUrlResponse> GetDiscordSocketUrlAsync(CancellationToken cancellationToken)
    {
        using var scope = _logger.BeginScope("[GET SOCKET URL]");
        var botToken = _discordServiceConfiguration.BotToken;
        HttpResponseMessage result;
        var response = new GetDiscordSocketUrlResponse();

        _logger.LogInformation("Buscando url e informações do Gateway.");

        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bot {botToken}");

        try
        {
            result = await _httpClient.GetAsync("/gateway/bot", cancellationToken);
            result.EnsureSuccessStatusCode();
            var responseBody = await result.Content.ReadAsStringAsync(cancellationToken);
            response = JsonSerializer.Deserialize<GetDiscordSocketUrlResponse>(responseBody);
        }
        catch (HttpRequestException e)
        {
            _logger.LogError(e, "Erro ao obter url: {reason}", e.HttpRequestError);
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro ao obter url: {reason}", e.Message);
            throw;
        }

        return response;
    }
}