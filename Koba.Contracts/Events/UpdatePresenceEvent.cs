using System.Text.Json.Serialization;
using Koba.Contracts.Entities;

namespace Koba.Contracts.Events;

public record UpdatePresenceEvent : EventBase
{
    [JsonPropertyName("d")]
    public Presence Presence { get; set; }
    
    public UpdatePresenceEvent() : base(3, null, null){}
}