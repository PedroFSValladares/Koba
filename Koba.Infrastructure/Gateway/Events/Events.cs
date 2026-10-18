using System.Text.Json.Serialization;
using Koba.Infrastructure.Gateway.EventData;

namespace Koba.Infrastructure.Gateway.Events;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "op")]
[JsonDerivedType(typeof(HeartBeatEvent), typeDiscriminator: 1)]
[JsonDerivedType(typeof(ResumeEvent), typeDiscriminator: 6)]
[JsonDerivedType(typeof(ReconnectEvent), typeDiscriminator: 7)]
[JsonDerivedType(typeof(InvalidSessionEvent), typeDiscriminator: 9)]
[JsonDerivedType(typeof(HelloEvent), typeDiscriminator: 10)]
[JsonDerivedType(typeof(HeartBeatAckEvent), typeDiscriminator: 11)]
public abstract record EventBase([property: JsonIgnore] int op, int? s, string? t);

public record HelloEvent(HelloData d, int? s, string? t) : EventBase(10, s, t);
internal record HeartBeatEvent(int d, int? s, string? t)  : EventBase(1, s , t);
internal record HeartBeatAckEvent(int? s, string? t) : EventBase(11, s, t);
internal record ReconnectEvent(int? s, string? t) : EventBase(7, s, t);
internal record InvalidSessionEvent(bool d, int? s, string? t) : EventBase(9, s, t);
public record ReadyEvent(int v, User user, UnavailableGuild guild, string session_id, string resume_gateway_url, int[]? shard, DiscordApplication application);
internal record ResumeEvent(int? s, string? t, ResumeData d) : EventBase(6, s, t);

public record HelloData(int heartbeat_interval); 
internal record ResumeData(string token, string session_id, int seq);