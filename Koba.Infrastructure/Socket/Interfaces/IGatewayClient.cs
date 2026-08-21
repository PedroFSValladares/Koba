namespace Koba.Infrastructure.Socket.Interfaces;

public interface IGatewayClient
{
    Task<GatewayChannel> ConnectAsync(string url, CancellationToken cancellationToken);
    void Close();
}