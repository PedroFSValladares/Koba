using System.Text.Json.Serialization;

namespace Koba.Infrastructure.Socket.EventData;

public class IntegrationTypeConfig
{
    [JsonPropertyName("oauth2_install_params")]
    public OAuth2InstallParams? OAuth2InstallParams { get; set; }
}