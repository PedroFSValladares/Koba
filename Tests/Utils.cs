using System.Threading.Channels;
using Koba.Infrastructure.Socket;
using Koba.Infrastructure.Socket.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests;

internal static class Utils
{
    internal static IRawSocket GetSocketClient()
    {
        var webSocket = new System.Net.WebSockets.ClientWebSocket();
        var logger = new Mock<ILogger<SocketClient>>();
        
        return new SocketClient(logger.Object);
    }
}