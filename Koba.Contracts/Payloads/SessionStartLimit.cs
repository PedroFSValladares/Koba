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

    private SessionStartLimit(int totalAllowedStarts, int remainingAllowedStarts, int milissecondsToResetStarts,
        int maxConcurrency)
    {
        TotalAllowedStarts = totalAllowedStarts;
        RemainingAllowedStarts = remainingAllowedStarts;
        MilissecondsToResetStarts = milissecondsToResetStarts;
        MaxConcurrency = maxConcurrency;
    }
    
    public static SessionStartLimit Create(int totalAllowedStarts, int remainingAllowedStarts, int milissecondsToResetStarts, int maxConcurrency) =>
        new SessionStartLimit(totalAllowedStarts, remainingAllowedStarts, milissecondsToResetStarts, maxConcurrency);
    
}