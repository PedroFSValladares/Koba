namespace Koba.Contracts.Events;

public record HeartBeatAckEvent(int? SequencialNumber, string? GatewayEventIdentifier) : EventBase(11, SequencialNumber, GatewayEventIdentifier);