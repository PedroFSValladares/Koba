using System.Text.Json.Serialization;

namespace Koba.Contracts.Events;

public record HeartBeatEvent : EventBase
{
    [JsonPropertyName("d")]
    public int? LastSequencialNumberReceived { get; }

    public HeartBeatEvent(int? lastSequencialNumberReceived) : base(1, null, null)
    {
        LastSequencialNumberReceived = lastSequencialNumberReceived;
    }
}