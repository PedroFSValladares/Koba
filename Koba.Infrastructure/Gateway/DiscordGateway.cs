using System.Text.Json;
using System.Threading.Channels;
using Koba.Infrastructure.Gateway.Interfaces;
using Koba.Infrastructure.Socket;
using Koba.Infrastructure.Socket.Events;
using Koba.Infrastructure.Socket.Interfaces;
using Microsoft.Extensions.Logging;

namespace Koba.Infrastructure.Gateway;

public class DiscordGateway : IDiscordGateway
{
    private readonly IRawSocket socket;
    private readonly ILogger<DiscordGateway> logger;
    private readonly SessionState cache;
    private readonly Channel<string> sendMessageChannel, receiveMessageChannel;
    
    private Timer heartbeatTimer;

    private DiscordGateway(IRawSocket socket, ILogger<DiscordGateway> logger, SessionState cache, Channel<string> sendMessageChannel, Channel<string> receiveMessageChannel)
    {
        this.socket = socket;
        this.logger = logger;
        this.cache = cache;
        this.socket.OnMessageReceived += EnqueueReceivedMessage;
        this.sendMessageChannel = sendMessageChannel;
        this.receiveMessageChannel = receiveMessageChannel;
    }

    private async Task MessageTriggered(OnMessageReceivedData messageData)
    {
        string rawData = messageData.Data;
        CancellationToken cancellationToken = messageData.CancellationToken;
        var parsedEvent = JsonSerializer.Deserialize<EventBase>(rawData);
        int? sequencialNumber = parsedEvent.s;
        
        if (sequencialNumber != null)
            cache.sequencialNumber = sequencialNumber;

        switch (parsedEvent)
        {
            case HelloEvent helloEvent:
                StartHeartBeatTimer(helloEvent.d.heartbeat_interval, cancellationToken);
                logger.LogInformation("HeartBeating inciado.");
                break;
            case HeartBeatEvent heartBeatEvent:
                logger.LogInformation("HeartBeat solicitado pelo gateway.");
                SendHeartBeat(cancellationToken);
                break;
            case HeartBeatAckEvent heartBeatAckEvent:
                logger.LogInformation("HeatBeat respondido.");
                break;
            case InvalidSessionEvent invalidSessionEvent:
                logger.LogInformation("Sessão inválida.");
                if(invalidSessionEvent.d)
                    ResumeConnection();
                else
                    Disconnect();
                break;
            default:
                await receiveMessageChannel.Writer.WriteAsync(rawData, cancellationToken);
                break;
        }
    }

    private void StartHeartBeatTimer(int interval, CancellationToken cancellationToken) => 
        heartbeatTimer = new Timer(SendHeartBeat, cancellationToken, 0, interval);
    
    
    private void SendHeartBeat(object? state)
    {
        CancellationToken cancellationToken = (CancellationToken)(state ?? CancellationToken.None);
        int? sequencialNumber = null;

        if (cache.sequencialNumber != null)
        {
            string cacheValue = cache.sequencialNumber.ToString()!;
             sequencialNumber = Convert.ToInt32(cacheValue);
        }
        
        if (state is not CancellationToken)
            logger.LogWarning("Token de cancelamento não recebido no envio do HeartBeat. Comportamentos inesperados ao desconectar podem ocorrer.");
        
        HeartBeatEvent heartBeat = new HeartBeatEvent(10, 0, sequencialNumber, null);
        string json = JsonSerializer.Serialize(heartBeat);
        _ = Task.Run(() => socket.SendAsync(json, cancellationToken), cancellationToken);
        
        logger.LogInformation("HeartBeat enviado.");
    }

    private void ResumeConnection()
    {
        
    }

    private void Disconnect()
    {
        
    }
    
    public Task SendEvent<TEventType>(TEventType evt, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private async Task EnqueueReceivedMessage(OnMessageReceivedData messageData)
    {
        using var scope = logger.BeginScope("[ENQUEUE TASK]");
        
        await receiveMessageChannel.Writer.WriteAsync(messageData.Data);
        logger.LogInformation("Mensagem {messageData} enfileirada para processamento.", messageData.Data);
    }
}