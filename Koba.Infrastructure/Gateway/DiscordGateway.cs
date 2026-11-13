using System.Text.Json;
using System.Threading.Channels;
using Koba.Contracts.Dtos;
using Koba.Contracts.Events;
using Koba.Contracts.Interfaces;
using Koba.Infrastructure.Socket;
using Koba.Infrastructure.Socket.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Koba.Infrastructure.Gateway;

public class DiscordGateway : IDiscordGateway
{
    private readonly SessionState _cache;
    private readonly GatewayConfiguration _config;
    private readonly Dictionary<Type, List<Func<object, Task>>> _handlers = [];
    private readonly ILogger<DiscordGateway> _logger;
    private readonly IMessagingService<string> _messenger;
    private readonly Channel<string> _sendMessageChannel, _receiveMessageChannel;
    private readonly IRawSocket _socket;

    private Timer _heartbeatTimer;
    private CancellationTokenSource _socketCancellationTokenSource = new();

    internal DiscordGateway(IRawSocket socket, ILogger<DiscordGateway> logger, SessionState cache,
        IOptions<GatewayConfiguration> gatewayConfiguration, IMessagingService<string> messenger)
    {
        _socket = socket;
        _logger = logger;
        _cache = cache;
        _socket.OnMessageReceived += EnqueueReceivedMessage;
        _sendMessageChannel = Channel.CreateUnbounded<string>();
        _receiveMessageChannel = Channel.CreateUnbounded<string>();
        _config = gatewayConfiguration.Value;
        _messenger = messenger;
    }

    public async Task Init(GatewayInitializationParams gatewayInitializationParams)
    {
        using var scope = _logger.BeginScope("[GATEWAY_INIT]");
        _logger.LogInformation("Inicializando Discord Gateway.");

        string gatewayUrl = gatewayInitializationParams.SocketUrl,
            receiveMessageTopicName = _config.ReceiveMessageTopicName,
            sendMessageTopicName = _config.SendMessageTopicName;
        CancellationToken token = _socketCancellationTokenSource.Token,
            cancellationToken = _socketCancellationTokenSource.Token;

        if (string.IsNullOrEmpty(receiveMessageTopicName))
            throw new ArgumentNullException(nameof(receiveMessageTopicName),
                "Nome do tópico de recebimento de mensagems não foi informado.");

        if (string.IsNullOrEmpty(sendMessageTopicName))
            throw new ArgumentNullException(nameof(sendMessageTopicName),
                "Nome do tópico de envio de mensagens não foi informado.");

        _logger.LogInformation("Realizando inscrição no tópico {receiveMessageTopicName}.", receiveMessageTopicName);
        _messenger.Subscribe(receiveMessageTopicName, message => DispatchEvent(message, token));
        _logger.LogInformation("Realizando inscrção no tópico {sendMessageTopicName}.", sendMessageTopicName);
        _messenger.Subscribe(sendMessageTopicName, message => _socket.SendAsync(message, token));

        try
        {
            _logger.LogInformation("Iniciando tentativa de conexão com o Gateway");
            await _socket.ConnectAsync(gatewayUrl, token);
        }
        catch (Exception e)
        {
            _logger.LogError("Erro ao inicializar o Gateway: {errorMessage}\n{exceptions}", e.Message, e);
            throw;
        }
    }

    public Task SendEvent<TEventType>(TEventType evt, CancellationToken cancellationToken)
    {
        using var scope = _logger.BeginScope("[SEND EVENT]");
        var json = JsonSerializer.Serialize(evt);
        var channelWriter = _sendMessageChannel.Writer;

        _logger.LogInformation("Iniciando enfileiramento de evento a ser enviado.");
        _logger.LogInformation("Evento a ser enfileirado: {json}.", json);

        try
        {
            channelWriter.WriteAsync(json, cancellationToken);
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogWarning("Enfileiramento do evento cancelado.\n{ex}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError("Erro ao enfileirar o evento: {message}.\n{exception}", ex.Message, ex);
        }

        _logger.LogInformation("Fim do enfileiramento da mensagem.");
        return Task.CompletedTask;
    }

    public void Subscribe<TEventType>(Func<TEventType, Task> callback, CancellationToken cancellationToken)
        where TEventType : EventBase
    {
        using var scope = _logger.BeginScope("[SUBSCRIPTION]");
        var type = typeof(TEventType);

        _logger.LogInformation("Nova inscrição recebido para o evento {type}.", type);

        if (!_handlers.ContainsKey(typeof(TEventType)))
            _handlers[type] = [];

        _handlers[type].Add(obj => callback((TEventType)obj));

        _logger.LogInformation("Inscrição  recebido para o evento {type} com sucesso.", type);
    }

    private async Task DispatchEvent(string message, CancellationToken cancellationToken)
    {
        using var scope = _logger.BeginScope("[EVENT HANDLING]");

        _logger.LogInformation("Evento recebido para processasmento.");
        EventBase? parsedEvent = null;
        try
        {
            parsedEvent = JsonSerializer.Deserialize<EventBase>(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro na deserialização do evento: {message}", ex.Message);
            return;
        }

        if (parsedEvent is null) return;
        var sequencialNumber = parsedEvent.SequencialNumber;

        _logger.LogInformation("Digíto sequencial: {sequencialNumber}", sequencialNumber);
        _logger.LogInformation("Evento a ser tratado: {eventType}", parsedEvent.GetType());

        if (sequencialNumber is not null)
            _cache.SequencialNumber = sequencialNumber;

        switch (parsedEvent)
        {
            case HelloEvent helloEvent:
                StartHeartBeatTimer(helloEvent.Data.HeartBeatInterval, cancellationToken);
                _logger.LogInformation("HeartBeating inciado.");
                await IdentifyAsync(cancellationToken);
                _logger.LogInformation("Identificação com o Gateway finalizada.");
                break;
            case HeartBeatEvent heartBeatEvent:
                _logger.LogInformation("HeartBeat solicitado pelo gateway.");
                SendHeartBeat(cancellationToken);
                break;
            case HeartBeatAckEvent heartBeatAckEvent:
                _logger.LogInformation("HeatBeat respondido.");
                break;
            case InvalidSessionEvent invalidSessionEvent:
                _logger.LogInformation("Sessão atual inválida. Sessão resumivel? {isResumable}",
                    invalidSessionEvent.CanResume);
                await Reconnect(invalidSessionEvent.CanResume);
                break;
            default:
                _logger.LogInformation("Evento não é de conexão. Despachando para aplicação.");
                CallHandlers(parsedEvent);
                break;
        }
    }

    private async Task IdentifyAsync(CancellationToken cancellationToken)
    {
        using var scope = _logger.BeginScope("[IDENTIFY HANDLER]");
        _logger.LogInformation("Carregando informações para identificação no Gateway.");
        var identifyEvent = new IdentifyEvent
        {
            Data = new IdentifyData
            {
                AppProperties = _config.AppProperties,
                InitialPresence = _config.InitialPresence,
                Intents = _config.Intent,
                LargeThreshold = _config.LargeThreshold,
                Token = _config.GatewayToken,
                UseCompression = _config.UseCompression
            }
        };
        var payload = JsonSerializer.Serialize(identifyEvent);

        try
        {
            _logger.LogInformation("Enviando identificação.");
            await _socket.SendAsync(payload, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro ao prosseguir na etapa de identifcação: {message}", e.Message);
        }
    }

    private void CallHandlers(EventBase eventToSend)
    {
        var eventType = eventToSend.GetType();

        if (!_handlers.TryGetValue(eventType, out var callbacks)) return;
        if (callbacks.Count <= 0) return;

        try
        {
            callbacks.ForEach(handler => handler(eventToSend));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro ao processar chamada de handler.");
        }
    }

    private void StartHeartBeatTimer(int interval, CancellationToken cancellationToken, bool first = true)
    {
        _heartbeatTimer?.Dispose();
        var dueTime = first ? interval : 0;

        _heartbeatTimer = new Timer(SendHeartBeat, cancellationToken, dueTime, interval);
    }


    private void SendHeartBeat(object? state)
    {
        var cancellationToken = (CancellationToken)(state ?? CancellationToken.None);
        int? sequencialNumber = null;

        if (_cache.SequencialNumber != null)
        {
            var cacheValue = _cache.SequencialNumber.ToString()!;
            sequencialNumber = Convert.ToInt32(cacheValue);
        }

        if (state is not CancellationToken)
            _logger.LogWarning(
                "Token de cancelamento não recebido no envio do HeartBeat. Comportamentos inesperados ao desconectar podem ocorrer.");

        var heartBeat = new HeartBeatEvent(sequencialNumber);
        var json = JsonSerializer.Serialize(heartBeat);
        _ = Task.Run(() => _socket.SendAsync(json, cancellationToken), cancellationToken);

        _logger.LogInformation("HeartBeat enviado.");
    }

    private async Task Reconnect(bool isResumable = false)
    {
        using var scope = _logger.BeginScope("[RECONNECTION HANDLER]");
        var url = isResumable && _cache.ResumeGatewayUrl is not null ? _cache.ResumeGatewayUrl : _cache.GatewayUrl;

        Disconnect();

        try
        {
            _logger.LogInformation("Reconectando ao host {url}", url);
            await _socket.ConnectAsync(url, _socketCancellationTokenSource.Token);
            if (isResumable)
            {
                _logger.LogInformation("Enviando info para reconexão resumida.");

                var resumeEvent = new ResumeEvent(_config.GatewayToken, _cache.SessionId, _cache.SequencialNumber);
                var json = JsonSerializer.Serialize(resumeEvent);

                await _socket.SendAsync(json, CancellationToken.None);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro ao tentar reconectar: {message}", e.Message);
            throw;
        }

        _logger.LogInformation("Reconexão finalizada.");
    }

    private void Disconnect()
    {
        using var scope = _logger.BeginScope("[DISCONNECT HANDLING]");

        _logger.LogInformation("Desconectando do host remoto.");

        _socketCancellationTokenSource.Cancel();
        _socketCancellationTokenSource.Dispose();
        _socketCancellationTokenSource = new CancellationTokenSource();
    }

    public void Close()
    {
        throw new NotImplementedException();
    }

    private async Task EnqueueReceivedMessage(OnMessageReceivedData messageData)
    {
        using var scope = _logger.BeginScope("[ENQUEUE TASK]");

        await _receiveMessageChannel.Writer.WriteAsync(messageData.Data);
        _logger.LogInformation("Mensagem {messageData} enfileirada para processamento.", messageData.Data);
    }
}