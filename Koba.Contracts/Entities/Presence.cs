using System.Text.Json.Serialization;
using Koba.Contracts.Enuns;

namespace Koba.Contracts.Entities;

public class Presence
{
    [JsonPropertyName("since")]
    public int Since { get; set; }
    [JsonPropertyName("activities")]
    public IEnumerable<Activity> Activities { get; set; }
    [JsonPropertyName("status")]
    public Status? Status { get; set; }
    [JsonPropertyName("afk")]
    public bool IsAfk { get; set; }
}