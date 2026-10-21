using System.Threading.Channels;

namespace Koba.Infrastructure.Socket.Interfaces;

internal interface IRawSocket
{
    Task SendAsync(string message, CancellationToken cancellationToken);
    Task ConnectAsync(string url, CancellationToken cancellationToken);
    event Func<OnMessageReceivedData, Task> OnMessageReceived;
    event Func<IRawSocket, Task> OnConnectionClosed;
}