using System.Text.Json.Serialization;

namespace Koba.Infrastructure.Gateway.EventData;

public class AvatarDecorationData
{
    [JsonPropertyName("asset")]
    public string Asset { get; set; } = string.Empty;

    [JsonPropertyName("sku_id")]
    public string SkuId { get; set; } = string.Empty;
}