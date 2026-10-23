using System.Text.Json.Serialization;
using Koba.Contracts.Entities;
using Koba.Contracts.Enuns;

namespace Koba.Contracts.Events;

public record IdentifyEvent : EventBase
{
    [JsonPropertyName("d")]
    public IdentifyData Data { get; init; }
    
    public IdentifyEvent() : base(2, null, null){}
}

public record IdentifyData
{
    [JsonPropertyName("token")]
    public string Token { get; init; }
    [JsonPropertyName("properties")]
    public AppProperties AppProperties { get; init; }
    [JsonPropertyName("compress")]
    public bool? UseCompression { get; init; }
    [JsonPropertyName("large_threshold")]
    public int? LargeThreshold { get; init; }
    [JsonPropertyName("shard")]
    public int[]? Shard { get; init; }
    [JsonPropertyName("presence")]
    public Presence? Presence { get; init; }
    [JsonPropertyName("intents")]
    public DiscordIntents Intents { get; init; }
    [JsonPropertyName("capabilities")]
    public int? Capabilities { get; init; }
}

public record AppProperties
{
    [JsonPropertyName("os")]
    public string Os { get; init; }
    [JsonPropertyName("browser")]
    public string Browser { get; init; }
    [JsonPropertyName("device")]
    public string Device { get; init; }
}