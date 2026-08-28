namespace Koba.Infrastructure.Socket.EventData;

using System.Text.Json.Serialization;

public class User
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("global_name")]
    public string? GlobalName { get; set; }

    [JsonPropertyName("discriminator")]
    public string Discriminator { get; set; } = string.Empty;

    [JsonPropertyName("avatar")]
    public string? Avatar { get; set; }

    [JsonPropertyName("verified")]
    public bool Verified { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("flags")]
    public int Flags { get; set; }

    [JsonPropertyName("banner")]
    public string? Banner { get; set; }

    [JsonPropertyName("accent_color")]
    public int? AccentColor { get; set; }

    [JsonPropertyName("premium_type")]
    public int PremiumType { get; set; }

    [JsonPropertyName("public_flags")]
    public int PublicFlags { get; set; }

    [JsonPropertyName("avatar_decoration_data")]
    public AvatarDecorationData? AvatarDecorationData { get; set; }

    [JsonPropertyName("collectibles")]
    public Collectibles? Collectibles { get; set; }

    [JsonPropertyName("primary_guild")]
    public PrimaryGuild? PrimaryGuild { get; set; }
}