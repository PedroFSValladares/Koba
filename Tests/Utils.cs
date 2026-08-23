using System.Threading.Channels;
using Koba.Infrastructure.Socket;
using Koba.Infrastructure.Socket.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests;

public static class Utils
{
    public static IGatewayClient GetGatewayClient()
    {
        var webSocket = new System.Net.WebSockets.ClientWebSocket();
        Channel<string> channel1 = Channel.CreateUnbounded<string>();
        Channel<string> channel2 = Channel.CreateUnbounded<string>();
        var logger = new Mock<ILogger<SocketClient>>();
        
        return new SocketClient(webSocket, channel1, channel2, logger.Object);
    }
}