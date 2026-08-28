using System.Text.Json.Serialization;

namespace Koba.Infrastructure.Socket.EventData;

public class TeamUser
{
    [JsonPropertyName("avatar")]
    public string? Avatar { get; set; }

    [JsonPropertyName("discriminator")]
    public string Discriminator { get; set; } = string.Empty;

    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;
}