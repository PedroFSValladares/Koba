using Koba.Contracts.Interfaces;
using Koba.Infrastructure.Gateway;
using Koba.Infrastructure.Socket;
using Koba.Infrastructure.Socket.Interfaces;
using Microsoft.Extensions.Options;
using Moq;
using Xunit.Abstractions;

namespace Tests;

internal static class Utils
{
    internal static IRawSocket GetSocketClient(ITestOutputHelper outputHelper)
    {
        var logger = new TestLogger<SocketClient>(outputHelper);
        
        return new SocketClient(logger);
    }

    internal static Mock<IRawSocket> GetMockedSocketClient(ITestOutputHelper outputHelper)
    {
        Mock<IRawSocket> socket = new Mock<IRawSocket>();
        CancellationTokenSource tokenSource = new CancellationTokenSource();
        
        socket.Setup(s => s.ConnectAsync(It.Is<string>(x => x.Equals("Ok")), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        socket.Setup(s => s.ConnectAsync(It.Is<string>(x => x.Equals("Generic Error")), It.IsAny<CancellationToken>())).Throws<Exception>();
        socket.Setup(s => s.SendAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        
        return socket;
    }

    internal static IDiscordGateway GetDiscordGateway(IRawSocket socket, ITestOutputHelper outputHelper, string gatewayUrl = "Ok", string token = "", int? sequencialNumber = null)
    {
        var logger = new TestLogger<DiscordGateway>(outputHelper);
        Mock<IOptions<GatewayConfiguration>> options = new Mock<IOptions<GatewayConfiguration>>();
        SessionState sessionState = new SessionState();
        sessionState.sequencialNumber = sequencialNumber;
        sessionState.sessionId = "teste";
        
        GatewayConfiguration gatewayConfig = new GatewayConfiguration();
        gatewayConfig.GatewayUrl = gatewayUrl;
        gatewayConfig.GatewayToken = token;
        options.Setup(o => o.Value).Returns(gatewayConfig);
        
        IDiscordGateway gateway = new DiscordGateway(socket, logger, sessionState,  options.Object);
        return gateway;
    }
}