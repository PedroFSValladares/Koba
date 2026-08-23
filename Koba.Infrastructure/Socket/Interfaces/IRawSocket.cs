using System.Threading.Channels;

namespace Koba.Infrastructure.Socket.Interfaces;

internal interface IRawSocket
{
    Task SendAsync(string message, CancellationToken cancellationToken);
    ChannelWriter<string> GetOutputChannel();
    event Func<OnEventReceivedData, Task> OnEventReceived;
}