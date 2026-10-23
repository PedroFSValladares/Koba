using System.Text.Json.Serialization;
using Koba.Contracts.Enuns;

namespace Koba.Contracts.Entities;

public class Activity
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("type")]
    public ActivityType Type { get; set; }
    [JsonPropertyName("url")]
    public string? Url { get; set; }
    [JsonPropertyName("state")]
    public string? Description { get; set; }
}