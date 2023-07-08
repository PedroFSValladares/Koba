using Discord.Models;
using Newtonsoft.Json;
using RestSharp;

namespace Discord.Net.Rest
{
    internal class DiscordRestClient
    {
        private readonly RestClient client;
        private readonly string baseUrl = "https://discord.com/api/v";
        private readonly int apiVersion;
        private readonly string token;

        public DiscordRestClient(int apiVersion, string token) {
            this.apiVersion = apiVersion;
            client = new RestClient(BuildUrl());
            client.AddDefaultHeader("Authorization", $"Bot {token}");
        }

        public GatewayInfo RequestGatewayInfo() {
            var request = new RestRequest("gateway/bot", Method.Get);
            var response = client.Get(request);
            return JsonConvert.DeserializeObject<GatewayInfo>(response.Content);
        }

        private string BuildUrl() {
            return baseUrl + apiVersion;
        }

        public async Task<RestResponse> Request(string endoint, string content, Method requestMethod) {
            var request = new RestRequest(endoint, requestMethod);
            request.AddJsonBody(content);
            return await client.ExecuteAsync(request);
        }

        public async Task<bool> RegisterCommand(CommandBase command) {
            var request = new RestRequest("applications/645321462168944640/commands");
            string commandJson = JsonConvert.SerializeObject(command);

            request.AddJsonBody(commandJson);

            var response = await client.PostAsync(request);

            return response.IsSuccessStatusCode;
        }
    }
}
