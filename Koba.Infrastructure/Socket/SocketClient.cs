using System.Net.WebSockets;
using System.Text;
using System.Threading.Channels;
using Koba.Infrastructure.Socket.Interfaces;
using Microsoft.Extensions.Logging;

namespace Koba.Infrastructure.Socket
{
    internal class SocketClient : IRawSocket
    {
        public event Func<OnMessageReceivedData, Task> OnMessageReceived;
        
        private ClientWebSocket? client; 
        private int bufferSize = 2048;
        
        private readonly ILogger<SocketClient> logger;

        internal SocketClient(ILogger<SocketClient> logger)
        {
            this.logger = logger;
        }
        
        internal void Close() {
            client.Abort();
        }

        public async Task ConnectAsync(string url, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url), "A URL fornecida não deve ser nula.");
            
            logger.LogInformation($"Preparando para se conectar a url {url}.");
            
            Uri uri = new(url);
            
            logger.LogInformation($"Conectado ao host com sucesso.");

            if (client.State != WebSocketState.Open)
                throw new InvalidOperationException("Estado do Socket não permite inicialização do listener.");
            client?.Dispose();
            try
            {
                client = new ClientWebSocket();
                logger.LogInformation("Preparando para se conectar ao host {url}.", url);
                await client.ConnectAsync(uri, cancellationToken);
                logger.LogInformation("Conectado ao host com sucesso.");
                
                _ = Task.Run(() => BeginListenAsync(cancellationToken), cancellationToken);
            }
            catch (OperationCanceledException ex)
            {
                logger.LogInformation("Operação de conecção interrompida. Não foi possível conectar ao host.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro desconhecido ao tentar conectar ao host: {mesage}",ex.Message);
                throw;
            }
                    
        }

        private async Task BeginListenAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation($"Listener iniciado.");
            
            byte[] buffer = new byte[bufferSize];
            Memory<byte> receiveBuffer = new Memory<byte>(buffer);
            List<byte> received = new List<byte>();
            ValueWebSocketReceiveResult result;

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    do
                    {
                        result = await client.ReceiveAsync(receiveBuffer, cancellationToken);

                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            await client.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, cancellationToken);
                            return;
                        }

                        received.AddRange(new ArraySegment<byte>(buffer, 0, result.Count));
                    } while (!result.EndOfMessage);

                    string jsonPaylod = Encoding.UTF8.GetString(received.ToArray());
                    
                    logger.LogDebug($"Payload recebido: {jsonPaylod}.");
                    
                    OnMessageReceivedData messageData = new OnMessageReceivedData(jsonPaylod, cancellationToken);
                    _ = Task.Run(() => OnMessageReceived.Invoke(messageData), cancellationToken);

                    received.Clear();
                }
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("Execução do socket foi encerrada.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
            }
        }

        public async Task SendAsync(string message, CancellationToken cancellationToken)
        {
            logger.LogInformation($"Task de envio iniciada");
            var buffer = Encoding.UTF8.GetBytes(message);
            
            if(client.State != WebSocketState.Open && !cancellationToken.IsCancellationRequested)
                throw new InvalidOperationException(
                    "Conexão do socket foi encerrada, não é possível enviar a mensagem");
            
            await client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, cancellationToken);
           
            logger.LogInformation("Mensagem enviada com sucesso");
        }
    }
}