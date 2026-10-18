using System.Threading.Channels;
using Koba.Infrastructure.Socket;
using Koba.Infrastructure.Socket.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests;

internal static class Utils
{
    internal static IRawSocket GetSocketClient(ITestOutputHelper outputHelper)
    {
        var logger = new TestLogger<SocketClient>(outputHelper);
        
        return new SocketClient(logger);
    }
}