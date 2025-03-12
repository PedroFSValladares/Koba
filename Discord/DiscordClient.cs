using Discord.Models;
using Discord.Net.Rest;
using Discord.Net.Socket;
using Newtonsoft.Json;

namespace Discord {
    public class DiscordClient {

        private readonly DiscordSocketClient socket;
        private readonly DiscordRestClient restClient;
        private readonly ClientConfiguration configs;

        public DiscordClient(ClientConfiguration clientConfigs) {
            configs = clientConfigs;
            restClient = new DiscordRestClient(configs.apiVersion, configs.token);
            socket = new DiscordSocketClient(configs.token, configs.apiVersion);

            //socket.EventReceived += OnEventReceived;
        }

        public async Task StartClientAsync() {
            var info = restClient.RequestGatewayInfo();
            await socket.ConnecToGatewayAsync(info.url + $"/?v={configs.apiVersion}&encoding=json");
        }

        public async Task LoginAsync() {
            await socket.IdentifyAsync();
        }

        public async Task AddCommandAsync(CommandBase command) {
            string jsonCommand = JsonConvert.SerializeObject(command);
            var response = await restClient.Request("applications/645321462168944640/commands", jsonCommand, RestSharp.Method.Post);
            Console.WriteLine("\n"+response.Content);
        } 

        public void OnEventReceived(EventInfo e) {
            //Converter o objeto do evento e chamar o evento respectivo
            if(e.Nome == "EVENT_CREATE") {

            }
        }
    }
}