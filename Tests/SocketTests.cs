using Koba.Infrastructure.Socket;
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
        var client = Utils.GetSocketClient();
        var receiveToken = new CancellationTokenSource();

        string host = config["SocketTestConfig:EchoServerHost"];
        string message = config["SocketTestConfig:Message"];
        int skipMessages = Convert.ToInt32(config["SocketTestConfig:ResponsesToSkip"]);
        int testTimeout = Convert.ToInt32(config["SocketTestConfig:Timeout"]);
        int skippedMessages = 0;
        string result = "";

        await client.ConnectAsync(host, receiveToken.Token);
        client.OnMessageReceived += (OnMessageReceivedData data) =>
        {
            result = data.Data;
            
            if (skippedMessages > skipMessages)
                Assert.Equal(message, result);
            
            return Task.CompletedTask;
        };

        try
        {
            var sendToken = new CancellationTokenSource(TimeSpan.FromSeconds(testTimeout));
            await client.SendAsync(message, sendToken.Token);
        }
        catch (OperationCanceledException)
        {
            Assert.Fail("Timeout exceeded.");
        }
        await receiveToken.CancelAsync();
    }
}