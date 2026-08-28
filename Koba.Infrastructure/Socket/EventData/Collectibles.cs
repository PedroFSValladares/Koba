using System.Text.Json.Serialization;

namespace Koba.Infrastructure.Socket.EventData;

public class Collectibles
{
    [JsonPropertyName("nameplate")]
    public Nameplate? Nameplate { get; set; }
}