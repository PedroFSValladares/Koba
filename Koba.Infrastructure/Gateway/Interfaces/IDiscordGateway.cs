using Koba.Infrastructure.Socket.Events;

namespace Koba.Infrastructure.Gateway.Interfaces;

public interface IDiscordGateway
{
    public Task SendEvent<TEventType>(TEventType evt, CancellationToken cancellationToken);
    public void Subscribe<TEventType>(Func<TEventType, Task> callback, CancellationToken cancellationToken) where TEventType : EventBase;
    public Task Init(CancellationToken cancellationToken);
}