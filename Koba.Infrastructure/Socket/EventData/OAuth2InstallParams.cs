using System.Text.Json.Serialization;

namespace Koba.Infrastructure.Socket.EventData;

public class OAuth2InstallParams
{
    [JsonPropertyName("scopes")]
    public List<string> Scopes { get; set; } = new();

    [JsonPropertyName("permissions")]
    public string Permissions { get; set; } = string.Empty;
}