using Koba.Contracts.Dtos;
using Koba.Contracts.Events;

namespace Koba.Contracts.Interfaces;

public interface IDiscordGateway
{
    public Task SendEvent<TEventType>(TEventType evt, CancellationToken cancellationToken);
    public void Subscribe<TEventType>(Func<TEventType, Task> callback, CancellationToken cancellationToken) where TEventType : EventBase;
    public Task Init(GatewayInitializationParams gatewayInitializationParams);
}