using Discord.Net.Socket.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.WebSockets;
using System.Text;

namespace Discord.Net.Socket
{
    public delegate Task SocketMessageReceivedEventHandler(object sender, GatewayPayload payload);
    public delegate Task HeartbeatSendEventHandler(object sender, GatewayPayload payload);
    public delegate void EventReceivedEventHandler(EventInfo payload);

    internal class DiscordSocketClient
    {
        private readonly ClientWebSocket client = new ClientWebSocket(); 
        private event SocketMessageReceivedEventHandler PayloadReceived;
        private event EventReceivedEventHandler EventReceived;
        internal event EventReceivedEventHandler Ready;
        private readonly int apiVersion;
        private Timer HeartbeatTimer;
        private Task Listenner;
        private readonly string token;
        private string baseUrl;
        private bool waitingHeartbeatAck;
        private int? requestNumber;

        public DiscordSocketClient(string token, int apiVersion)
        {
            this.token = token;
            this.apiVersion = apiVersion;
            HeartbeatTimer = new Timer(Heartbeat, null, Timeout.Infinite, 0);
            PayloadReceived += OnPayloadReceived;
            Ready += OnReady;
        }

        public async Task ConnecToGatewayAsync(string url) {
            baseUrl = url;
            await client.ConnectAsync(new Uri(baseUrl), CancellationToken.None);
            Listenner = ListenConnection();
        }

        public async Task IdentifyAsync() {
            GatewayPayload payload = new GatewayPayload();
            payload.op = GatewayPayloadOpCode.Identify;
            payload.d = new IdentifyEventData {
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

        private async Task OnPayloadReceived(object sender, GatewayPayload e) {
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
            GatewayPayload heartbeat = new GatewayPayload();
            heartbeat.op = GatewayPayloadOpCode.HeartBeat;
            heartbeat.d = requestNumber;
            await SendAsync(heartbeat);
            Console.WriteLine("Heartbeat enviado");
        }

        private async Task SendAsync(GatewayPayload payload) {
            string payloadJson = JsonConvert.SerializeObject(payload);
            byte[] buffer = Encoding.UTF8.GetBytes(payloadJson);
            await client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private async Task ListenConnection() {
            byte[]? buffer = new byte[2048];
            ArraySegment<byte> receiveBuffer = new ArraySegment<byte>(buffer);
            List<byte> received = new List<byte>();
            WebSocketReceiveResult result;

            while(true) {
                do {
                    result = await client.ReceiveAsync(receiveBuffer, CancellationToken.None);
                    received.AddRange(buffer);
                    Array.Clear(buffer);
                } while(!result.EndOfMessage);

                if (result.MessageType == WebSocketMessageType.Close) {
                    await client.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                } else {
                    string jsonPaylod = Encoding.UTF8.GetString(received.ToArray());
                    Console.WriteLine($"\n{jsonPaylod}");
                    GatewayPayload payload = JsonConvert.DeserializeObject<GatewayPayload>(jsonPaylod);
                    await PayloadReceived.Invoke(this, payload);
                }
                received.Clear();
            }
        }
    }
}