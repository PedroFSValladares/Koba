using System.Threading.Channels;
using Koba.Infrastructure.Socket;
using Koba.Infrastructure.Socket.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests;

public static class Utils
{
    public static IRawSocket GetGatewayClient()
    {
        var webSocket = new System.Net.WebSockets.ClientWebSocket();
        var logger = new Mock<ILogger<SocketClient>>();
        
        return new SocketClient(webSocket, logger.Object);
    }
}