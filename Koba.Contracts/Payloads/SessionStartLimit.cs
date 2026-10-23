using System.Text.Json.Serialization;

namespace Koba.Contracts.Payloads;

public record SessionStartLimit
{
    [JsonPropertyName("total")]
    public int TotalAllowedStarts { get; }
    [JsonPropertyName("remaining")]
    public int RemainingAllowedStarts { get; }
    [JsonPropertyName("reset_after")]
    public int MilissecondsToResetStarts { get; }
    [JsonPropertyName("max_concurrency")]
    public int MaxConcurrency { get; }
}