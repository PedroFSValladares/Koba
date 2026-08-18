namespace Koba.Infrastructure;

public interface IGatewayClient
{
    Task ConnectAsync(string url, CancellationToken cancellationToken);
    void CloseAsync();
    Task SendAsync(string data);
}