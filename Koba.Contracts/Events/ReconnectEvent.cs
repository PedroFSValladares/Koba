namespace Koba.Contracts.Events;

internal record ReconnectEvent : EventBase
{
    public ReconnectEvent() : base(7, null, null){}
}