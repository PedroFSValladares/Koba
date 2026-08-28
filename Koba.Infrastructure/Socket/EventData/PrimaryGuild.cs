using System.Text.Json.Serialization;

namespace Koba.Infrastructure.Socket.EventData;

public class PrimaryGuild
{
    [JsonPropertyName("identity_guild_id")]
    public string? IdentityGuildId { get; set; }

    [JsonPropertyName("identity_enabled")]
    public bool? IdentityEnabled { get; set; }

    [JsonPropertyName("tag")]
    public string? Tag { get; set; }

    [JsonPropertyName("badge")]
    public string? Badge { get; set; }
}