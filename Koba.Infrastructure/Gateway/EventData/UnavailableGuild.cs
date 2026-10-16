using System.Text.Json.Serialization;

namespace Koba.Infrastructure.Gateway.EventData;

public class UnavailableGuild
{
    [JsonPropertyName("id")]
    public string Id { get; init; }
    [JsonPropertyName("unavailable")]
    public bool IsUnavailable { get; init; }
}