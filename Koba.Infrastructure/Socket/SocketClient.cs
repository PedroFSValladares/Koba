using System.Net.WebSockets;
using System.Text;
using System.Threading.Channels;
using Koba.Infrastructure.Socket.Interfaces;

namespace Koba.Infrastructure.Socket
{
    public class SocketClient : IGatewayClient
    {
        private readonly ClientWebSocket client; 
        private int bufferSize = 2048;
        private Channel<string> sendMessageChannel, receiveMessageChannel;

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
            
            Uri uri = new(url);

            await client.ConnectAsync(uri, cancellationToken);

            if (client.State != WebSocketState.Open)
                throw new InvalidOperationException("Estado do Socket não permite inicialização do listener.");
                    
            receiveMessageChannel = Channel.CreateUnbounded<string>();
            sendMessageChannel = Channel.CreateUnbounded<string>();
            
            _ = Task.Run(() => BeginListenAsync(cancellationToken), cancellationToken);
            _ = Task.Run(() => SendAsync(cancellationToken), cancellationToken);
            
            return GatewayChannel.Create(sendMessageChannel.Writer, receiveMessageChannel.Reader);
        }

        private async Task SendAsync(CancellationToken cancellationToken)
        {
            await foreach (string message in sendMessageChannel.Reader.ReadAllAsync(cancellationToken))
            {
                var buffer = Encoding.UTF8.GetBytes(message);
                
                if(client.State != WebSocketState.Open && !cancellationToken.IsCancellationRequested)
                    throw new InvalidOperationException(
                        "Conexão do socket foi encerrada, não é possível enviar a mensagem");
                
                await client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Binary, true, cancellationToken);
            }
        }

        private async Task BeginListenAsync(CancellationToken cancellationToken)
        {
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

                    await channelWriter.WriteAsync(jsonPaylod, cancellationToken);

                    received.Clear();
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Execução do socket foi encerrada.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
            finally
            {
                channelWriter.TryComplete();
            }
        }
    }
}