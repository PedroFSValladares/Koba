//using Discord;
//using Discord.WebSocket;
using Koba.Services.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koba.Services.Client
{
    public class DiscordClient : IDiscordClient
    {
        /*
        private DiscordSocketClient client;
        private readonly IDataBaseService _context;
        private string token; 

        public DiscordClient(string token,DiscordSocketConfig configs)
        {
            this.token = token;
            client = new DiscordSocketClient(configs);
        }

        public DiscordClient(string token, IDataBaseService context) {
            _context = context;
            this.token = token;
            client = new DiscordSocketClient();
        }
        

        public async Task Run()
        {
            client.Log += Log;
            await client.IdentifyAsync(TokenType.Bot, token);
            await client.StartAsync();
            await Task.Delay(-1);
        }

        private Task Log(LogMessage message) {
            Console.WriteLine(message);
            return Task.CompletedTask;
        }
        */
    }
}
