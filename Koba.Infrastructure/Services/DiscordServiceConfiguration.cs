namespace Koba.Infrastructure.Services;

internal class DiscordServiceConfiguration
{
    internal const string Section = "DiscordServiceConfiguration";
    internal string BaseAddress { get; init; }
    internal string Token { get; init; }
}