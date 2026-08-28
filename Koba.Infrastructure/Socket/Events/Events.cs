using System.Text.Json.Serialization;
using Koba.Infrastructure.Socket.EventData;

namespace Koba.Infrastructure.Socket.Events;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "op")]
[JsonDerivedType(typeof(HelloEvent), typeDiscriminator: 10)]
[JsonDerivedType(typeof(HeartBeatEvent), typeDiscriminator: 1)]
[JsonDerivedType(typeof(ReconnectEvent), typeDiscriminator: 7)]
[JsonDerivedType(typeof(HeartBeatAckEvent), typeDiscriminator: 11)]
[JsonDerivedType(typeof(InvalidSessionEvent), typeDiscriminator: 9)]
internal abstract record EventBase(int op, int? s, string? t);

internal abstract record HelloEvent(int op, HelloData d, int? s, string? t) : EventBase(op, s, t);
internal record HeartBeatEvent(int op, int d, int? s, string? t)  : EventBase(op, s , t);
internal abstract record HeartBeatAckEvent(int op, int? s, string? t) : EventBase(op, s, t);
internal abstract record ReconnectEvent(int op, int? s, string? t) : EventBase(op, s, t);
internal abstract record InvalidSessionEvent(int op, bool d, int? s, string? t) : EventBase(op, s, t);
internal abstract record ReadyEvent(int v, User user, UnavailableGuild guild, string session_id, string resume_gateway_url, int[]? shard, DiscordApplication application);

internal abstract record HelloData(int heartbeat_interval); 
internal record ResumeData(string token, string session_id, int seq);