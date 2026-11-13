namespace Koba.Infrastructure.Socket;

public class OnMessageReceivedData
{
    internal OnMessageReceivedData(string data, CancellationToken cancellationToken)
    {
        Data = data;
        CancellationToken = cancellationToken;
    }

    public string Data { get; private set; }
    public CancellationToken CancellationToken { get; private set; }
}