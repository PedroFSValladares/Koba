namespace Koba.Infrastructure.Socket;

public class OnMessageReceivedData
{
    public string Data { get; private set; }
    public CancellationToken CancellationToken {get; private set;}
    
    internal OnMessageReceivedData(string data, CancellationToken cancellationToken)
    {
        Data = data;
        CancellationToken = cancellationToken;
    }
}