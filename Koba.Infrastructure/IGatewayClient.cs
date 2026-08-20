using System.Threading.Channels;

namespace Koba.Infrastructure;

public interface IGatewayClient
{
    Task<ChannelReader<string>> ConnectAsync(string url, CancellationToken cancellationToken);
    void CloseAsync();
    Task SendAsync(string data, CancellationToken cancellationToken);
}