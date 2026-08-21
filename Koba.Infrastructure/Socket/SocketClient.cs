using System.Net.WebSockets;
using System.Text;
using System.Threading.Channels;
using Koba.Infrastructure.Socket.Interfaces;
using Microsoft.Extensions.Logging;

namespace Koba.Infrastructure.Socket
{
    public class SocketClient : IGatewayClient
    {
        private readonly ClientWebSocket client; 
        private int bufferSize = 2048;
        private Channel<string> sendMessageChannel, receiveMessageChannel;
        private ILogger _logger;

        public SocketClient()
        {
            client = new();
        }
        
        public void Close() {
            client.Abort();
            receiveMessageChannel.Writer.TryComplete();
        }

        public async Task<GatewayChannel> ConnectAsync(string url, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url), "A URL fornecida não deve ser nula.");
            
            _logger.LogInformation($"Preparando para se conectar a url {url}.");
            
            Uri uri = new(url);
            await client.ConnectAsync(uri, cancellationToken);
            
            _logger.LogInformation($"Conectado ao host com sucesso.");

            if (client.State != WebSocketState.Open)
                throw new InvalidOperationException("Estado do Socket não permite inicialização do listener.");
                    
            _logger.LogInformation($"Iniciando filas de envio e recebimento....");
            
            receiveMessageChannel = Channel.CreateUnbounded<string>();
            sendMessageChannel = Channel.CreateUnbounded<string>();
            
            _ = Task.Run(() => BeginListenAsync(cancellationToken), cancellationToken);
            _ = Task.Run(() => SendAsync(cancellationToken), cancellationToken);
            
            _logger.LogInformation($"Socket conectado com sucesso e pronto para envio de mensagens.");
            
            return GatewayChannel.Create(sendMessageChannel.Writer, receiveMessageChannel.Reader);
        }

        private async Task SendAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Task de envio iniciada");
            await foreach (string message in sendMessageChannel.Reader.ReadAllAsync(cancellationToken))
            {
                var buffer = Encoding.UTF8.GetBytes(message);
                
                _logger.LogDebug($"Mensagem recebida para envio {message}.");
                
                if(client.State != WebSocketState.Open && !cancellationToken.IsCancellationRequested)
                    throw new InvalidOperationException(
                        "Conexão do socket foi encerrada, não é possível enviar a mensagem");
                
                await client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Binary, true, cancellationToken);
            }
            _logger.LogInformation($"Task de envio encerrada");
        }

        private async Task BeginListenAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Listener iniciado.");
            
            byte[] buffer = new byte[bufferSize];
            Memory<byte> receiveBuffer = new Memory<byte>(buffer);
            List<byte> received = new List<byte>();
            ValueWebSocketReceiveResult result;
            ChannelWriter<string> channelWriter = receiveMessageChannel.Writer;

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    do
                    {
                        result = await client.ReceiveAsync(receiveBuffer, cancellationToken);
                        received.AddRange(new ArraySegment<byte>(buffer, 0, result.Count));
                    } while (!result.EndOfMessage);

                    string jsonPaylod = Encoding.UTF8.GetString(received.ToArray());
                    
                    _logger.LogDebug($"Payload recebido: {jsonPaylod}.");

                    await channelWriter.WriteAsync(jsonPaylod, cancellationToken);

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
                _logger.LogInformation("Finalizando canal de envio....");
                bool closeResult = channelWriter.TryComplete();
                _logger.LogInformation($"Canal finalizado com sucesso: {closeResult}.");
            }
        }
    }
}