namespace Koba.Infrastructure.Socket.EventData;

using System.Text.Json.Serialization;

public class DiscordApplication
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("icon")]
    public string? Icon { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("bot_public")]
    public bool BotPublic { get; set; }

    [JsonPropertyName("bot_require_code_grant")]
    public bool BotRequireCodeGrant { get; set; }

    [JsonPropertyName("terms_of_service_url")]
    public string? TermsOfServiceUrl { get; set; }

    [JsonPropertyName("privacy_policy_url")]
    public string? PrivacyPolicyUrl { get; set; }

    [JsonPropertyName("owner")]
    public Owner? Owner { get; set; }

    [JsonPropertyName("verify_key")]
    public string VerifyKey { get; set; } = string.Empty;

    [JsonPropertyName("team")]
    public Team? Team { get; set; }

    [JsonPropertyName("guild_id")]
    public string? GuildId { get; set; }

    [JsonPropertyName("primary_sku_id")]
    public string? PrimarySkuId { get; set; }

    [JsonPropertyName("slug")]
    public string? Slug { get; set; }

    [JsonPropertyName("cover_image")]
    public string? CoverImage { get; set; }

    [JsonPropertyName("flags")]
    public int Flags { get; set; }

    [JsonPropertyName("flags_new")]
    public string? FlagsNew { get; set; }

    [JsonPropertyName("interactions_endpoint_url")]
    public string? InteractionsEndpointUrl { get; set; }

    [JsonPropertyName("role_connections_verification_url")]
    public string? RoleConnectionsVerificationUrl { get; set; }

    [JsonPropertyName("event_webhooks_url")]
    public string? EventWebhooksUrl { get; set; }

    [JsonPropertyName("event_webhooks_status")]
    public int EventWebhooksStatus { get; set; }

    [JsonPropertyName("integration_types_config")]
    public Dictionary<string, IntegrationTypeConfig>? IntegrationTypesConfig { get; set; }
}