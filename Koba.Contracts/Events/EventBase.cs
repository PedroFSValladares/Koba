using System.Text.Json.Serialization;

namespace Koba.Contracts.Events;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "op")]
[JsonDerivedType(typeof(HeartBeatEvent), typeDiscriminator: 1)]
[JsonDerivedType(typeof(ResumeEvent), typeDiscriminator: 6)]
[JsonDerivedType(typeof(ReconnectEvent), typeDiscriminator: 7)]
[JsonDerivedType(typeof(InvalidSessionEvent), typeDiscriminator: 9)]
[JsonDerivedType(typeof(HelloEvent), typeDiscriminator: 10)]
[JsonDerivedType(typeof(HeartBeatAckEvent), typeDiscriminator: 11)]
public abstract record EventBase
{
    [JsonIgnore]
    public int OpCode { get; }
    [JsonPropertyName("s")]
    public int? SequencialNumber { get; }
    [JsonPropertyName("t")]
    public string? GatewayEventIdentifier { get; }

    public EventBase(int OpCode, int? SequencialNumber, string? GatewayEventIdentifier)
    {
        this.OpCode = OpCode;
        this.SequencialNumber = SequencialNumber;
        this.GatewayEventIdentifier = GatewayEventIdentifier;
    }
}