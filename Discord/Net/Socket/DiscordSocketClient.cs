using Discord.Net.Rest;
using Discord.Net.Socket.Events;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.WebSockets;
using System.Text;

namespace Discord.Net.Socket
{
    public delegate Task SocketMessageReceivedEventHandler(object sender, GatewayEvent payload);
    public delegate Task HeartbeatSendEventHandler(object sender, GatewayEvent payload);
    public delegate void EventReceivedEventHandler(EventInfo payload);

    public class DiscordSocketClient : IHostedService
    {
        private event SocketMessageReceivedEventHandler PayloadReceived;
        private event EventReceivedEventHandler EventReceived;
        internal event EventReceivedEventHandler Ready;


        private readonly ClientWebSocket _client; 
        private readonly DiscordRestClient _restClient;
        private readonly int _apiVersion;
        private readonly string token;
        private Timer HeartbeatTimer;
        private Task Listenner;
        private int? requestNumber;
        private bool listenning = true;

        public DiscordSocketClient(ClientWebSocket client, DiscordRestClient restClient, string token, int apiVersion)
        {
            _client = client;
            _restClient = restClient;
            _apiVersion = apiVersion;
            this.token = token;
        }

        public async Task StartAsync(CancellationToken cancellationToken) {
            HeartbeatTimer = new Timer(Heartbeat, null, Timeout.Infinite, 0);
            PayloadReceived += OnPayloadReceived;
            Ready += OnReady;

            await ConnecToGatewayAsync(cancellationToken);

            await IdentifyAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken) {
            HeartbeatTimer.Dispose();
            await _client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Finalizando aplicação", cancellationToken);
            listenning = false;
        }

        public async Task ConnecToGatewayAsync(CancellationToken cancellationToken) {
            string url = _restClient.RequestGatewayInfo().url + $"/?v={_apiVersion}&encoding=json";
            await _client.ConnectAsync(new Uri(url), cancellationToken);
            Listenner = ListenConnection();
        }

        public async Task IdentifyAsync() {
            GatewayEvent payload = new GatewayEvent();
            payload.op = GatewayPayloadOpCode.Identify;
            payload.d = new IdentifyEvent {
                token = this.token,
                intents = Intents.MESSAGE_CONTENT,
                properties = new ConnectionProperties {
                    os = "windows",
                    browser = "KobaLibrary",
                    device = "KobaLibrary"
                }
            };
            await SendAsync(payload);
        }

        private async Task OnPayloadReceived(object sender, GatewayEvent e) {
            dynamic eventData = e.d;
            JObject obj = (JObject)e.d;
            
            switch(e.op) {
                case GatewayPayloadOpCode.Hello:
                    requestNumber = e.s;
                    HeartbeatTimer.Change(0, (int)eventData.heartbeat_interval);
                    break;
                case GatewayPayloadOpCode.HeartbeatAck:
                    Console.WriteLine("heartbeat respondido");
                    break;
                case GatewayPayloadOpCode.Dispatch:
                    try {
                        switch(e.t) {
                            case "READY":
                                Ready readyEvent = obj.ToObject<Ready>();
                                Ready.Invoke(readyEvent);
                                break;
                            case "INTERACTION_CREATE":

                                break;
                        }
                        requestNumber = e.s;
                    }catch(Exception ex) {
                        Console.WriteLine(ex.ToString());
                    }
                    /*
                    EventInfo info = new EventInfo {
                        Nome = e.t,
                        EventData = e.d
                    };
                    EventReceived.Invoke(eventData);
                    */
                    break;
                default:
                    Console.WriteLine($"Um evento não reconhecido foi recebido {JsonConvert.SerializeObject(e)}");
                    break;
            }
        }

        private void OnReady(EventInfo e) {

        }

        private async void Heartbeat(Object stateInfo) {
            GatewayEvent heartbeat = new GatewayEvent();
            heartbeat.op = GatewayPayloadOpCode.HeartBeat;
            heartbeat.d = requestNumber;
            await SendAsync(heartbeat);
            Console.WriteLine("Heartbeat enviado");
        }

        private async Task SendAsync(GatewayEvent payload) {
            string payloadJson = JsonConvert.SerializeObject(payload);
            byte[] buffer = Encoding.UTF8.GetBytes(payloadJson);
            await _client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private async Task ListenConnection() {
            byte[]? buffer = new byte[2048];
            ArraySegment<byte> receiveBuffer = new ArraySegment<byte>(buffer);
            List<byte> received = new List<byte>();
            WebSocketReceiveResult result;

            while(listenning) {
                do {
                    result = await _client.ReceiveAsync(receiveBuffer, CancellationToken.None);
                    received.AddRange(buffer);
                    Array.Clear(buffer);
                } while(!result.EndOfMessage);

                if (result.MessageType == WebSocketMessageType.Close) {
                    await _client.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                } else {
                    string jsonPaylod = Encoding.UTF8.GetString(received.ToArray());
                    Console.WriteLine($"\n{jsonPaylod}");
                    GatewayEvent payload = JsonConvert.DeserializeObject<GatewayEvent>(jsonPaylod);
                    await PayloadReceived.Invoke(this, payload);
                }
                received.Clear();
            }
        }
    }
}