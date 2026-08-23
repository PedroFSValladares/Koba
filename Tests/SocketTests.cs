using Xunit.Abstractions;

namespace Tests;

public class SocketTests
{
    private readonly ITestOutputHelper testOutputHelper;

    public SocketTests(ITestOutputHelper testOutputHelper)
    {
        this.testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task SocketClient_Deve_Receber_e_Enviar_Mensagem_Sucesso()
    {
        var client = Utils.GetGatewayClient();
        var receiveToken = new CancellationTokenSource();

        string host = "wss://echo.websocket.org";
        string message = "ping";
        string result = "";

        var channels = await client.ConnectAsync(host, receiveToken.Token);

        try
        {
            var sendToken = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            await channels.SendMessageAsync(message, sendToken.Token);
            
            if(host == "wss://echo.websocket.org")
                _ = await channels.GetMessageAsync(receiveToken.Token);
            
            result = await channels.GetMessageAsync(receiveToken.Token);
        }
        catch (OperationCanceledException)
        {
            Assert.Fail("Timeout exceeded.");
        }
        
        Assert.Equal(message, result);
        
        await receiveToken.CancelAsync();
    }
}