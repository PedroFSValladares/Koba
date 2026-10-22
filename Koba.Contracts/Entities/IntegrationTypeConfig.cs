using System.Text.Json.Serialization;

namespace Koba.Contracts.Entities;

public class IntegrationTypeConfig
{
    [JsonPropertyName("oauth2_install_params")]
    public OAuth2InstallParams? OAuth2InstallParams { get; set; }
}