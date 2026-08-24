using System.Reflection.Metadata;
using System.Text.Json;
using System.Threading.Channels;
using Koba.Infrastructure.Socket;
using Koba.Infrastructure.Socket.Events;
using Koba.Infrastructure.Socket.Interfaces;
using Microsoft.Extensions.Logging;

namespace Koba.Infrastructure;

internal class GatewayManager
{
    private readonly IRawSocket socket;
    private readonly ILogger<GatewayManager> logger;
    private readonly Dictionary<string, string> cache;
    
    private Timer heartbeatTimer;

    private GatewayManager(IRawSocket socket, ILogger<GatewayManager> logger, Dictionary<string, string> cache)
    {
        this.socket = socket;
        this.logger = logger;
        this.cache = cache;
        this.socket.OnEventReceived += EventTriggered;
    }

    private async Task EventTriggered(OnEventReceivedData eventData)
    {
        string rawData = eventData.Data;
        CancellationToken cancellationToken = eventData.CancellationToken;
        var parsedEvent = JsonSerializer.Deserialize<EventBase>(rawData);
        ChannelWriter<string> channel = socket.GetOutputChannel();
        int? sequencialNumber = parsedEvent.s;
        
        if (sequencialNumber != null)
            cache.TryAdd("sequencialNumber", sequencialNumber.ToString());

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
                await channel.WriteAsync(rawData, cancellationToken);
                break;
        }
    }

    private void StartHeartBeatTimer(int interval, CancellationToken cancellationToken) => 
        heartbeatTimer = new Timer(SendHeartBeat, cancellationToken, 0, interval);
    
    
    private void SendHeartBeat(object? state)
    {
        CancellationToken cancellationToken = (CancellationToken)(state ?? CancellationToken.None);
        int? sequencialNumber = null;
        
        if (cache.TryGetValue("sequencialNumber", out string? cacheValue))
            sequencialNumber = Convert.ToInt32(cacheValue);
        
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
}