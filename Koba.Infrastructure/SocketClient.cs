using System.Net.WebSockets;
using System.Text;
using System.Threading.Channels;

namespace Koba.Infrastructure
{
    public delegate Task SocketMessageReceivedEventHandler(string payload);
    public delegate void SocketComunicationFailedEventHandler(ClientWebSocket socket, Exception exception);

    public class SocketClient : IGatewayClient
    {
        private readonly ClientWebSocket client; 
        private int bufferSize = 2048;
        private Channel<string> sendChannel, receiveMessageChannel;

        public SocketClient()
        {
            client = new();
        }
        
        public void CloseAsync() {
            client.Abort();
            receiveMessageChannel.Writer.TryComplete();
        }

        public async Task<ChannelReader<string>> ConnectAsync(string url, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url), "A URL fornecida não deve ser nula.");
            
            Uri uri = new(url);

            await client.ConnectAsync(uri, cancellationToken);

            if (client.State == WebSocketState.Open)
            {
                receiveMessageChannel = Channel.CreateUnbounded<string>();
                _ = Task.Run(() => BeginListenAsync(cancellationToken), cancellationToken);
            }
            
            return receiveMessageChannel.Reader;
        }

        public async Task SendAsync(string payload, CancellationToken cancellationToken) {
            byte[] buffer = Encoding.UTF8.GetBytes(payload);

            if(client.State != WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            {
                throw new InvalidOperationException(
                    "Conexão do socket foi encerrada, não é possível enviar a mensagem");
            }
            await client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Binary, true, CancellationToken.None);
        }

        private async Task BeginListenAsync(CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[bufferSize];
            Memory<byte> receiveBuffer = new Memory<byte>(buffer);
            List<byte> received = new List<byte>();
            ValueWebSocketReceiveResult result;
            ChannelWriter<string> channelWriter = receiveMessageChannel.Writer;

            while (!cancellationToken.IsCancellationRequested) {
                do {
                    result = await client.ReceiveAsync(receiveBuffer, cancellationToken);
                    received.AddRange(new ArraySegment<byte>(buffer, 0, result.Count));
                } while(!result.EndOfMessage);
                
                string jsonPaylod = Encoding.UTF8.GetString(received.ToArray());
                
                await channelWriter.WriteAsync(jsonPaylod, cancellationToken);

                received.Clear();
            }

            channelWriter.Complete();
        }
    }
}