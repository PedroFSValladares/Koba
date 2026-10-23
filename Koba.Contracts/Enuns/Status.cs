using System.Text.Json.Serialization;

namespace Koba.Contracts.Enuns;

public enum Status
{
    [JsonStringEnumMemberName("online")]
    Online,
    [JsonStringEnumMemberName("dnd")]
    DoNotDisturb,
    [JsonStringEnumMemberName("idle")]
    AFK,
    [JsonStringEnumMemberName("invisible")]
    Invisible,
    [JsonStringEnumMemberName("offline")]
    Offline,
}