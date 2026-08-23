using System.Text.Json.Serialization;

namespace Koba.Infrastructure.Socket.Events;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "op")]
[JsonDerivedType(typeof(HelloEvent), typeDiscriminator: 10)]
[JsonDerivedType(typeof(HeartBeatEvent), typeDiscriminator: 1)]
[JsonDerivedType(typeof(HeartBeatAckEvent), typeDiscriminator: 11)]
internal abstract record EventBase(int op, int? s, string? t);

internal abstract record HelloEvent(int op, HelloData d) : EventBase(op, null, null);
internal abstract record HeartBeatEvent(int op, int d)  : EventBase(op, null , null);
internal abstract record HeartBeatAckEvent(int op) : EventBase(op, null, null);