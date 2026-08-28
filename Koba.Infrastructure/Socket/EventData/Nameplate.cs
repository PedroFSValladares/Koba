using System.Text.Json.Serialization;

namespace Koba.Infrastructure.Socket.EventData;

public class Nameplate
{
    [JsonPropertyName("sku_id")]
    public string SkuId { get; set; } = string.Empty;

    [JsonPropertyName("asset")]
    public string Asset { get; set; } = string.Empty;

    [JsonPropertyName("label")]
    public string Label { get; set; } = string.Empty;

    [JsonPropertyName("palette")]
    public string Palette { get; set; } = string.Empty;
}