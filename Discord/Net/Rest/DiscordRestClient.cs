using Discord.Models;
using Newtonsoft.Json;
using RestSharp;

namespace Discord.Net.Rest
{
    public class DiscordRestClient
    {
        private readonly RestClient _client;

        public DiscordRestClient(RestClient restClient) {
            _client = restClient;
        }

        public GatewayInfo RequestGatewayInfo() {
            var request = new RestRequest("gateway/bot", Method.Get);
            var response = _client.Get(request);
            return JsonConvert.DeserializeObject<GatewayInfo>(response.Content);
        }

        public async Task<RestResponse> Request(string endoint, string content, Method requestMethod) {
            var request = new RestRequest(endoint, requestMethod);
            request.AddJsonBody(content);
            return await _client.ExecuteAsync(request);
        }

        public async Task<bool> RegisterCommand(CommandBase command) {
            var request = new RestRequest("applications/645321462168944640/commands");
            string commandJson = JsonConvert.SerializeObject(command);

            request.AddJsonBody(commandJson);

            var response = await _client.PostAsync(request);

            return response.IsSuccessStatusCode;
        }
    }
}
