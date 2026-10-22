using System.Text.Json;
using Koba.Infrastructure.Services.Dtos;
using Koba.Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Koba.Infrastructure.Services;

public class DiscordService : IDiscordService
{
    
    private readonly HttpClient _httpClient;
    private readonly ILogger<DiscordService> _logger;
    private readonly DiscordServiceConfiguration _discordServiceConfiguration;
    
    public DiscordService(HttpClient httpClient, ILogger<DiscordService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    
    public async Task<GetDiscordSocketUrlResponse> GetDiscordSocketUrlAsync(CancellationToken cancellationToken)
    {
        using var scope = _logger.BeginScope("[GET SOCKET URL]");
        string botToken = _discordServiceConfiguration.BotToken;
        HttpResponseMessage result;
        GetDiscordSocketUrlResponse response = new GetDiscordSocketUrlResponse();
        
        _logger.LogInformation("Buscando url e informações do Gateway.");
        
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bot {botToken}");

        try
        {
            result = await _httpClient.GetAsync("/gateway/bot", cancellationToken);
            result.EnsureSuccessStatusCode();
            string responseBody = await result.Content.ReadAsStringAsync(cancellationToken);
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