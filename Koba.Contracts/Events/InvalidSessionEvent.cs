using System.Text.Json.Serialization;

namespace Koba.Contracts.Events;

public record InvalidSessionEvent : EventBase
{
    [JsonPropertyName("d")]
    public bool CanResume { get; }

    public InvalidSessionEvent(bool canResume) : base(9, null, null)
    {
        CanResume = canResume;
    }
}