namespace Koba.Infrastructure.Socket;

public class OnEventReceivedData
{
    internal string Data;
    internal CancellationToken CancellationToken;
    
    internal OnEventReceivedData(string data, CancellationToken cancellationToken)
    {
        Data = data;
        CancellationToken = cancellationToken;
    }
}