using System.Text.Json;
using System.Threading.Channels;
using Koba.Infrastructure.Gateway.Events;
using Koba.Infrastructure.Gateway.Interfaces;
using Koba.Infrastructure.Socket;
using Koba.Infrastructure.Socket.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Koba.Infrastructure.Gateway;

public class DiscordGateway : IDiscordGateway
{
    private readonly IRawSocket socket;
    private readonly ILogger<DiscordGateway> logger;
    private readonly SessionState cache;
    private readonly Channel<string> sendMessageChannel, receiveMessageChannel;
    private readonly Dictionary<Type, List<Func<object, Task>>> handlers = [];
    private readonly GatewayConfiguration config;
    private CancellationTokenSource socketCancellationTokenSource = new CancellationTokenSource();
    
    private Timer heartbeatTimer;

    internal DiscordGateway(IRawSocket socket, ILogger<DiscordGateway> logger, SessionState cache, IOptions<GatewayConfiguration> gatewayConfiguration)
    {
        this.socket = socket;
        this.logger = logger;
        this.cache = cache;
        this.socket.OnMessageReceived += EnqueueReceivedMessage;
        this.sendMessageChannel = Channel.CreateUnbounded<string>();
        this.receiveMessageChannel = Channel.CreateUnbounded<string>();
        config = gatewayConfiguration.Value;
    }
    
    public async Task Init(CancellationToken cancellationToken)
    {
        using var scope = logger.BeginScope("[GATEWAY_INIT]");
        string gatewayUrl = config.GatewayUrl;
        CancellationToken token = socketCancellationTokenSource.Token;
        try
        {
            logger.LogInformation("Iniciando tentativa de conexão com o Gateway");
            await socket.ConnectAsync(gatewayUrl, token);

            logger.LogInformation("Iniando EventDispatching.");
            _ = Task.Run(() => ProccessReceivedMessages(cancellationToken), cancellationToken);
            logger.LogInformation("Iniciando Task de envio de mensagens.");
            _ = Task.Run(() => SendEqueuedMessages(cancellationToken), cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError("Erro ao inicializar o Gateway: {errorMessage}\n{exceptions}", e.Message, e);
            throw;
        }
    }

    private async Task DispatchEvent(string message, CancellationToken cancellationToken)
    {
        using var scope = logger.BeginScope("[EVENT HANDLING]");
        
        logger.LogInformation("Evento recebido para processasmento.");
        EventBase? parsedEvent = null;
        try
        {
            parsedEvent = JsonSerializer.Deserialize<EventBase>(message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro na deserialização do evento: {message}", ex.Message);
            return;
        }
        if (parsedEvent is null) return;
        int? sequencialNumber = parsedEvent.s;
        
        logger.LogInformation("Digíto sequencial: {sequencialNumber}", sequencialNumber);
        logger.LogInformation("Evento a ser tratado: {eventType}", parsedEvent.GetType());
        
        if (sequencialNumber is not null)
            cache.sequencialNumber = sequencialNumber;

        switch (parsedEvent)
        {
            case HelloEvent helloEvent:
                StartHeartBeatTimer(helloEvent.d.heartbeat_interval, cancellationToken);
                logger.LogInformation("HeartBeating inciado.");
                CallHandlers(parsedEvent);
                logger.LogInformation("Hello Event despachado.");
                break;
            case HeartBeatEvent heartBeatEvent:
                logger.LogInformation("HeartBeat solicitado pelo gateway.");
                SendHeartBeat(cancellationToken);
                break;
            case HeartBeatAckEvent heartBeatAckEvent:
                logger.LogInformation("HeatBeat respondido.");
                break;
            case InvalidSessionEvent invalidSessionEvent:
                logger.LogInformation("Sessão atual inválida. Sessão resumivel? {isResumable}", invalidSessionEvent.d);
                await Reconnect(invalidSessionEvent.d);
                break;
            default:
                logger.LogInformation("Evento não é de conexão. Despachando para aplicação.");
                CallHandlers(parsedEvent);
                break;
        }
    }

    private void CallHandlers(EventBase eventToSend)
    {
        Type eventType = eventToSend.GetType();

        if (!handlers.TryGetValue(eventType, out var callbacks)) return;
        if (callbacks.Count <= 0) return;
        
        try
        {
            callbacks.ForEach(handler => handler(eventToSend));
        }
        catch (Exception e)
        {
            logger.LogError(e, "Erro ao processar chamada de handler.");
        }
    }

    private void StartHeartBeatTimer(int interval, CancellationToken cancellationToken, bool first = true)
    {
        heartbeatTimer?.Dispose();
        int dueTime = first ? interval : 0;
        
        heartbeatTimer = new Timer(SendHeartBeat, cancellationToken, dueTime, interval);
    }
    
    
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
        
        HeartBeatEvent heartBeat = new HeartBeatEvent(0, sequencialNumber, null);
        string json = JsonSerializer.Serialize(heartBeat);
        _ = Task.Run(() => socket.SendAsync(json, cancellationToken), cancellationToken);
        
        logger.LogInformation("HeartBeat enviado.");
    }

    private async Task Reconnect(bool isResumable = false)
    {
        using var scope = logger.BeginScope("[RECONNECTION HANDLER]");
        string url = isResumable && cache.resumeGatewayUrl is not null ? cache.resumeGatewayUrl : config.GatewayUrl;
        
        Disconnect();
        
        try
        {
            logger.LogInformation("Reconectando ao host {url}", url);
            await socket.ConnectAsync(url, socketCancellationTokenSource.Token);
            if (isResumable)
            {
                logger.LogInformation("Enviando info para reconexão resumida.");
                
                ResumeData data = new ResumeData(config.GatewayToken, cache.sessionId, (int)cache.sequencialNumber);
                ResumeEvent resumeEvent = new ResumeEvent(null, null, data);
                string json = JsonSerializer.Serialize(resumeEvent);
                
                await socket.SendAsync(json, CancellationToken.None);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Erro ao tentar reconectar: {message}", e.Message);
            throw;
        }
        logger.LogInformation("Reconexão finalizada.");
    }

    private void Disconnect()
    {
        using var scope = logger.BeginScope("[DISCONNECT HANDLING]");
        
        logger.LogInformation("Desconectando do host remoto.");
        
        socketCancellationTokenSource.Cancel();
        socketCancellationTokenSource.Dispose();
        socketCancellationTokenSource = new CancellationTokenSource();
    }
    
    public Task SendEvent<TEventType>(TEventType evt, CancellationToken cancellationToken)
    {
        using var scope = logger.BeginScope("[SEND EVENT]");
        string json = JsonSerializer.Serialize(evt);
        ChannelWriter<string> channelWriter = sendMessageChannel.Writer;
        
        logger.LogInformation("Iniciando enfileiramento de evento a ser enviado.");
        logger.LogInformation("Evento a ser enfileirado: {json}.", json);

        try
        {
            channelWriter.WriteAsync(json, cancellationToken);
        }
        catch (OperationCanceledException ex)
        {
            logger.LogWarning("Enfileiramento do evento cancelado.\n{ex}", ex);
        }
        catch (Exception ex)
        {
            logger.LogError("Erro ao enfileirar o evento: {message}.\n{exception}", ex.Message, ex);
        }
        
        logger.LogInformation("Fim do enfileiramento da mensagem.");
        return Task.CompletedTask;
    }

    public void Subscribe<TEventType>(Func<TEventType, Task> callback, CancellationToken cancellationToken) where TEventType : EventBase
    {
        using var scope = logger.BeginScope("[SUBSCRIPTION]");
        var type = typeof(TEventType);
        
        logger.LogInformation("Nova inscrição recebido para o evento {type}.", type);
        
        if (!handlers.ContainsKey(typeof(TEventType)))
            handlers[type] = [];
        
        handlers[type].Add(obj => callback((TEventType)obj));
        
        logger.LogInformation("Inscrição  recebido para o evento {type} com sucesso.", type);
    }

    public void Close()
    {
        throw new NotImplementedException();
    }
    
    private async Task EnqueueReceivedMessage(OnMessageReceivedData messageData)
    {
        using var scope = logger.BeginScope("[ENQUEUE TASK]");
        
        await receiveMessageChannel.Writer.WriteAsync(messageData.Data);
        logger.LogInformation("Mensagem {messageData} enfileirada para processamento.", messageData.Data);
    }

    private async Task ProccessReceivedMessages(CancellationToken cancellationToken)
    {
        using var scope = logger.BeginScope("[PROCESS MESSAGE TASK]");
        
        logger.LogInformation("Task de processamento iniciada.");
        while (!cancellationToken.IsCancellationRequested)
            await foreach (var receivedMessage in receiveMessageChannel.Reader.ReadAllAsync(CancellationToken.None))
            {
                logger.LogInformation("Evento enviado para despacho.");
                await DispatchEvent(receivedMessage, cancellationToken);
            }
        logger.LogInformation("Task de processamento finalizada.");
    }

    private async Task SendEqueuedMessages(CancellationToken cancellationToken)
    {
        using var scope = logger.BeginScope("[SEND MESSAGE TASK]");
        ChannelReader<string> channelReader = sendMessageChannel.Reader;
        
        logger.LogInformation("Task de envio iniciada.");
        while (!cancellationToken.IsCancellationRequested)
            await foreach (var receivedMessage in channelReader.ReadAllAsync(cancellationToken))
            {
                try
                {
                    await socket.SendAsync(receivedMessage, cancellationToken);
                    logger.LogInformation("Evento enviado com sucesso: {receivedMessage}", receivedMessage);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Erro ao enviar mensagem.");
                }
            }
        logger.LogInformation("Task de envio finalizada.");
    }
}