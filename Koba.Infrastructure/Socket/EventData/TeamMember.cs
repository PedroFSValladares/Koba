using System.Text.Json.Serialization;

namespace Koba.Infrastructure.Socket.EventData;

public class TeamMember
{
    [JsonPropertyName("membership_state")]
    public int MembershipState { get; set; }

    [JsonPropertyName("permissions")]
    public List<string> Permissions { get; set; } = new();

    [JsonPropertyName("team_id")]
    public string TeamId { get; set; } = string.Empty;

    [JsonPropertyName("user")]
    public TeamUser? User { get; set; }
}