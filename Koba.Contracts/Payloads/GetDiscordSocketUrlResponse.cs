using System.Text.Json.Serialization;

namespace Koba.Contracts.Payloads;

public record GetDiscordSocketUrlResponse
{
    [JsonPropertyName("url")]
    public string SocketUrl { get; }
    [JsonPropertyName("shards")]
    public int Shards { get; }
    [JsonPropertyName("session_start_limit")]
    public SessionStartLimit SessionStartLimitInfo { get; }
}