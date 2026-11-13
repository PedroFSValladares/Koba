using System.Net.WebSockets;
using System.Text;
using Koba.Infrastructure.Socket.Interfaces;
using Microsoft.Extensions.Logging;

namespace Koba.Infrastructure.Socket;

internal class SocketClient : IRawSocket
{
    private readonly ILogger<SocketClient> _logger;
    private readonly int _bufferSize = 2048;

    private ClientWebSocket? _client;

    internal SocketClient(ILogger<SocketClient> logger)
    {
        _logger = logger;
    }

    public event Func<OnMessageReceivedData, Task> OnMessageReceived;
    public event Func<IRawSocket, Task> OnConnectionClosed;

    public async Task ConnectAsync(string url, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(url))
            throw new ArgumentNullException(nameof(url), "A URL fornecida não deve ser nula.");

        using var scope = _logger.BeginScope("[SOCKET CONNECTION]");
        var uri = new Uri(url);

        _client?.Dispose();
        try
        {
            _client = new ClientWebSocket();
            _logger.LogInformation("Preparando para se conectar ao host {url}.", url);
            await _client.ConnectAsync(uri, cancellationToken);
            _logger.LogInformation("Conectado ao host com sucesso.");

            _ = Task.Run(() => BeginListenAsync(cancellationToken), cancellationToken);
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogInformation("Operação de conecção interrompida. Não foi possível conectar ao host.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro desconhecido ao tentar conectar ao host: {mesage}", ex.Message);
            throw;
        }
    }

    public async Task SendAsync(string message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Task de envio iniciada");
        var buffer = Encoding.UTF8.GetBytes(message);

        if (_client.State != WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            throw new InvalidOperationException(
                "Conexão do socket foi encerrada, não é possível enviar a mensagem");

        await _client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, cancellationToken);

        _logger.LogInformation("Mensagem enviada com sucesso");
    }

    internal void Close()
    {
        _client.Abort();
    }

    private async Task BeginListenAsync(CancellationToken cancellationToken)
    {
        using var scope = _logger.BeginScope("[SOCKET LISTENNER]");
        var buffer = new byte[_bufferSize];
        var receiveBuffer = new Memory<byte>(buffer);
        var received = new List<byte>();
        ValueWebSocketReceiveResult result;

        _logger.LogInformation("Iniciando linstenner.");

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                do
                {
                    result = await _client.ReceiveAsync(receiveBuffer, cancellationToken);

                    if (result.MessageType is WebSocketMessageType.Close)
                    {
                        await _client.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty,
                            cancellationToken);
                        throw new OperationCanceledException("Conexão encerrada pelo host remoto.");
                    }

                    received.AddRange(new ArraySegment<byte>(buffer, 0, result.Count));
                } while (!result.EndOfMessage);

                var jsonPaylod = Encoding.UTF8.GetString(received.ToArray());
                _logger.LogDebug($"Payload recebido: {jsonPaylod}.");

                var messageData = new OnMessageReceivedData(jsonPaylod, cancellationToken);
                _ = Task.Run(() => OnMessageReceived?.Invoke(messageData), cancellationToken);

                received.Clear();
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Execução do socket foi encerrada.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }
        finally
        {
            _ = Task.Run(() => OnConnectionClosed?.Invoke(this), CancellationToken.None);
            _logger.LogInformation("Listener finalizado.");
        }
    }
}