using System.Text.Json.Serialization;

namespace Koba.Contracts.Events;

public record HelloEvent : EventBase
{
    [JsonPropertyName("d")]
    public HelloData Data { get; }

    public HelloEvent(int heartBeatInterval) : base(10, null, null)
    {
        Data = new HelloData(heartBeatInterval);
    }
}

public record HelloData
{
    [JsonPropertyName("heartbeat_interval")]
    public int HeartBeatInterval { get; }

    public HelloData(int heartBeatInterval)
    {
        HeartBeatInterval = heartBeatInterval;
    }
}