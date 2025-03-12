using System.Net.WebSockets;
using System.Text;

namespace Koba.BotSocket
{
    public delegate Task SocketMessageReceivedEventHandler(string payload);
    public delegate void SocketComunicationFailedEventHandler(ClientWebSocket socket, Exception exception);

    public class DiscordSocketClient
    {
        public event SocketMessageReceivedEventHandler OnMessageReceived;
        public event SocketComunicationFailedEventHandler OnComunicationFailed;

        private readonly ClientWebSocket client; 
        private Task Listenner;
        private bool listenning = true;

        public DiscordSocketClient()
        {
            client = new();
        }
        
        public async Task StopAsync(CancellationToken cancellationToken) {
            listenning = false;
            client.Abort();
        }

        public async Task ConnecToGatewayAsync(Uri url, CancellationToken cancellationToken) {
            await client.ConnectAsync(url, CancellationToken.None);

            Listenner = ListenConnection();
        }

        public async Task SendAsync(string payload) {
            byte[] buffer = Encoding.UTF8.GetBytes(payload);

            if(client.State != WebSocketState.Open) {
                OnComunicationFailed.Invoke(client, new Exception("Conexão com o gateway não está aberta."));
                throw new SocketConnectionExecpetion("Conexão com o gateway não está aberta.");
            }
            await client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Binary, true, CancellationToken.None);
        }

        private async Task ListenConnection() {
            byte[] buffer = new byte[2048];
            Memory<byte> receiveBuffer = new Memory<byte>(buffer);
            List<byte> received = new List<byte>();
            ValueWebSocketReceiveResult result;

            while (listenning) {
                do {
                    result = await client.ReceiveAsync(receiveBuffer, CancellationToken.None);
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