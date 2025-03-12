using Discord.Models;
using Discord.Net.Rest;
using Discord.Net.Socket;
using Newtonsoft.Json;

namespace Discord {
    public class DiscordClient {

        //private readonly DiscordSocketClient _socket;
        private readonly DiscordRestClient _restClient;
        //private readonly int _apiVersion;

        public DiscordClient(DiscordSocketClient socket, DiscordRestClient restClient, int apiVersion) {
            //_socket = socket;
            _restClient = restClient;
            //_apiVersion = apiVersion;
            //socket.EventReceived += OnEventReceived;
        }
        /*
        public async Task StartClientAsync() {
            var info = _restClient.RequestGatewayInfo();
            await _socket.ConnecToGatewayAsync(info.url + $"/?v={_apiVersion}&encoding=json");
        }

        public async Task LoginAsync() {
            await _socket.IdentifyAsync();
        }
        */

        public async Task AddCommandAsync(CommandBase command) {
            string jsonCommand = JsonConvert.SerializeObject(command);
            var response = await _restClient.Request("applications/645321462168944640/commands", jsonCommand, RestSharp.Method.Post);
            Console.WriteLine("\n"+response.Content);
        } 

        public void OnEventReceived(EventInfo e) {
            //Converter o objeto do evento e chamar o evento respectivo
            if(e.Nome == "EVENT_CREATE") {

            }
        }
    }
}