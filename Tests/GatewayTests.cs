using System.Text.Json;
using Koba.Contracts;
using Koba.Contracts.Events;
using Koba.Contracts.Interfaces;
using Koba.Infrastructure.Gateway;
using Koba.Infrastructure.Socket;
using Koba.Infrastructure.Socket.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit.Abstractions;

namespace Tests;

public class GatewayTests
{
    
    private readonly ITestOutputHelper _output;

    public GatewayTests(ITestOutputHelper output)
    {
        _output = output;
    }
    
    [Fact(DisplayName = "Testa inicialização do gateway, se não obtem erro então obtem sucesso.")]
    public async Task TestaInicializarOGateway_SeNaoObterErro_ObtemSucesso()
    {
        CancellationTokenSource tokenSource = new CancellationTokenSource();
        Mock<IRawSocket> socket = Utils.GetMockedSocketClient(_output);
        IDiscordGateway gateway = Utils.GetDiscordGateway(socket.Object, _output);

        await gateway.Init(tokenSource.Token);
        socket.Verify(s => s.ConnectAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact(DisplayName = "Testa publicação de eventos de gatweay, se não obtem erro então obtem sucesso.")]
    public async Task TestaInscricaoEPublicacaoDeEventosDeGateway_SeNaoObterErro_ObtemSucesso()
    {
        CancellationTokenSource tokenSource = new CancellationTokenSource();
        Mock<IRawSocket> socket = Utils.GetMockedSocketClient(_output);
        IDiscordGateway gateway = Utils.GetDiscordGateway(socket.Object, _output);
        
        HelloEvent helloEvent = new HelloEvent(2000);
        string falseEvent = JsonSerializer.Serialize<EventBase>(helloEvent);
        bool callbackExecuted = false, eventNotNull = false, opCodeOk = false, heartbeatOk = false;

        await gateway.Init(tokenSource.Token);
        
        gateway.Subscribe<HelloEvent>((e) =>
        {
            callbackExecuted = e is
            {
                Data.HeartBeatInterval: 2000,
                OpCode: 10,
                SequencialNumber: null,
                GatewayEventIdentifier: null
            };
            return Task.CompletedTask;
        }, tokenSource.Token);
        
        socket.Raise(e => e.OnMessageReceived += null, new OnMessageReceivedData(falseEvent,tokenSource.Token));
        Thread.Sleep(200);
        Assert.True(callbackExecuted);
    }
    
    [Fact(DisplayName = "Testa envio de HeartBeat, se fizer dois envios, obtem sucesso.")]
    public async Task TestaHeartBeat_SeFizerDoisEnvios_ObtemSucesso()
    {
        CancellationTokenSource tokenSource = new CancellationTokenSource();
        Mock<IRawSocket> socket = Utils.GetMockedSocketClient(_output);
        IDiscordGateway gateway = Utils.GetDiscordGateway(socket.Object, _output);
        HelloEvent helloEvent = new HelloEvent(100);
        string falseEvent = JsonSerializer.Serialize<EventBase>(helloEvent);
        bool callbackExecuted = false;
        string rawPayload = """{"d":0,"s":null,"t":null}""";

        await gateway.Init(tokenSource.Token);
        
        socket.Raise(e => e.OnMessageReceived += null, new OnMessageReceivedData(falseEvent,tokenSource.Token));
        Thread.Sleep(210);
        socket.Verify(s => s.SendAsync(It.Is<string>(x => x.Equals(rawPayload)), It.IsAny<CancellationToken>()), Times.AtLeast(2));
    }
    
    [Fact(DisplayName = "Testa reconexão após InvalidSessionEvent, se não obtem erro então obtem sucesso.")]
    public async Task TestaReconexaoRapidaAoGateway_SeNaoObterErro_ObtemSucesso()
    {
        CancellationTokenSource tokenSource = new CancellationTokenSource();
        Mock<IRawSocket> socket = Utils.GetMockedSocketClient(_output);
        IDiscordGateway gateway = Utils.GetDiscordGateway(socket.Object, _output);
        
        InvalidSessionEvent resumeEvent = new InvalidSessionEvent(true);
        string falseEvent = JsonSerializer.Serialize<EventBase>(resumeEvent);

        await gateway.Init(tokenSource.Token);
        
        gateway.Subscribe<InvalidSessionEvent>((e) =>
        {
            Assert.Fail("Evento foi despachado indevidamente.");
            return Task.CompletedTask;
        }, tokenSource.Token);
        
        await socket.RaiseAsync(e => e.OnMessageReceived += null, new OnMessageReceivedData(falseEvent,tokenSource.Token));
        
        await Task.Delay(200);
        
        socket.Verify(s => s.ConnectAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.AtLeast(2));
        socket.Verify(s => s.SendAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}