using System.Text.Json.Serialization;
using Koba.Contracts.Entities;

namespace Koba.Contracts.Events;

public record ReadyEvent
{
    [JsonPropertyName("v")]
    public int ApiVersion { get; }
    [JsonPropertyName("user")]
    public User User { get; }
    [JsonPropertyName("guilds")]
    public IEnumerable<UnavailableGuild> UnavailableGuilds { get; }
    [JsonPropertyName("session_id")]
    public string SessionId { get; }
    [JsonPropertyName("resume_gateway_url")]
    public string ResumeGatewayUrl { get; }
    [JsonPropertyName("shard")]
    public int[]? Shards { get; }
    [JsonPropertyName("application")]
    public DiscordApplication Application { get; }
};