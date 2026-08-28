using System.Text.Json.Serialization;

namespace Koba.Infrastructure.Socket.EventData;

public class Team
{
    [JsonPropertyName("icon")]
    public string? Icon { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("members")]
    public List<TeamMember> Members { get; set; } = new();

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("owner_user_id")]
    public string OwnerUserId { get; set; } = string.Empty;
}