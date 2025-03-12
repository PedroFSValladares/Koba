using Discord.Net.Rest;
using Discord.Net.Socket;
using Microsoft.Extensions.DependencyInjection;
using RestSharp;
using System.Net.WebSockets;

namespace Discord.Extensions {
    public static class DependencyInjection {
        public static void AddDiscordDependecies(this IServiceCollection services, string token, string ApiUrl, int apiVersion) {
            services.AddSingleton<DiscordClient>(x => 
            new DiscordClient(x.GetRequiredService<DiscordSocketClient>(), x.GetRequiredService<DiscordRestClient>(), apiVersion))
                .AddScoped<DiscordRestClient>(x => new DiscordRestClient(x.GetRequiredService<RestClient>()))
                .AddScoped<RestClient>(x => new RestClient(ApiUrl).AddDefaultHeader("Authorization", $"Bot {token}"))
                .AddScoped<ClientWebSocket>();
        }
    }
}
