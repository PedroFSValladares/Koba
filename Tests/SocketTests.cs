using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;

namespace Tests;

public class SocketTests
{
    private readonly ITestOutputHelper testOutputHelper;
    private readonly IConfiguration config;

    public SocketTests(ITestOutputHelper testOutputHelper)
    {
        this.testOutputHelper = testOutputHelper;
        config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();
    }

    [Fact]
    public async Task SocketClient_Deve_Receber_e_Enviar_Mensagem_Sucesso()
    {
        var client = Utils.GetGatewayClient();
        var receiveToken = new CancellationTokenSource();

        string host = config["SocketTestConfig:EchoServerHost"];
        string message = config["SocketTestConfig:Message"];
        int skipMessages = Convert.ToInt32(config["SocketTestConfig:ResponsesToSkip"]);
        int testTimeout = Convert.ToInt32(config["SocketTestConfig:Timeout"]);
        string result = "";

        var channels = await client.ConnectAsync(host, receiveToken.Token);

        try
        {
            var sendToken = new CancellationTokenSource(TimeSpan.FromSeconds(testTimeout));
            await channels.SendMessageAsync(message, sendToken.Token);

            for (int i = 0; i < skipMessages; i++)
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