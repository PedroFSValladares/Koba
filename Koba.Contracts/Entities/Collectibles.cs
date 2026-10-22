using System.Text.Json.Serialization;

namespace Koba.Contracts.Entities;

public class Collectibles
{
    [JsonPropertyName("nameplate")]
    public Nameplate? Nameplate { get; set; }
}