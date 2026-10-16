using System.Text.Json.Serialization;

namespace Koba.Infrastructure.Gateway.EventData;

public class Collectibles
{
    [JsonPropertyName("nameplate")]
    public Nameplate? Nameplate { get; set; }
}