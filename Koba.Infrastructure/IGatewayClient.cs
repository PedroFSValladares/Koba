using System.Threading.Channels;

namespace Koba.Infrastructure;

public interface IGatewayClient
{
    Task<GatewayChannel> ConnectAsync(string url, CancellationToken cancellationToken);
    void Close();
}