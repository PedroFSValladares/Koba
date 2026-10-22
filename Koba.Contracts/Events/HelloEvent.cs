using System.Text.Json.Serialization;

namespace Koba.Contracts.Events;

public record HelloEvent : EventBase
{
    [JsonPropertyName("d")]
    public HelloData Data { get; init; }

    public HelloEvent(int heartBeatInterval) : base(10, null, null)
    {
        Data = new HelloData(heartBeatInterval);
    }
    
    public HelloEvent() : base(10, null, null){}
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