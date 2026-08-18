using System.Net.WebSockets;
using System.Text;

namespace Koba.Infrastructure
{
    public delegate Task SocketMessageReceivedEventHandler(string payload);
    public delegate void SocketComunicationFailedEventHandler(ClientWebSocket socket, Exception exception);

    public class SocketClient : IGatewayClient
    {
        public event SocketMessageReceivedEventHandler OnMessageReceived;
        public event SocketComunicationFailedEventHandler OnComunicationFailed;

        private readonly ClientWebSocket client; 
        private int bufferSize = 2048;
        private CancellationToken token;

        public SocketClient()
        {
            client = new();
        }
        
        public void CloseAsync() {
            client.Abort();
        }

        public async Task ConnectAsync(string url, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url), "A URL fornecida não deve ser nula.");
            
            Uri uri = new(url);
            token = cancellationToken;

            await client.ConnectAsync(uri, token);
            
            await BeginListenAsync();
        }

        public async Task SendAsync(string payload) {
            byte[] buffer = Encoding.UTF8.GetBytes(payload);

            if(client.State != WebSocketState.Open) {
                OnComunicationFailed.Invoke(client, new InvalidOperationException("Conexão com o gateway não está aberta."));
            }
            await client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Binary, true, CancellationToken.None);
        }

        private async Task BeginListenAsync()
        {
            byte[] buffer = new byte[bufferSize];
            Memory<byte> receiveBuffer = new Memory<byte>(buffer);
            List<byte> received = new List<byte>();
            ValueWebSocketReceiveResult result;

            while (!token.IsCancellationRequested) {
                do {
                    result = await client.ReceiveAsync(receiveBuffer, token);
                    received.AddRange(buffer);
                    Array.Clear(buffer);
                } while(!result.EndOfMessage);
                
                string jsonPaylod = Encoding.UTF8.GetString(received.ToArray());
                await OnMessageReceived.Invoke(jsonPaylod);

                received.Clear();
            }
        }
    }
}