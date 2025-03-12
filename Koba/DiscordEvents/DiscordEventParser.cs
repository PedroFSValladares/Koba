using Koba.BotSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Koba.DiscordEvents
{

    public delegate void DiscordEventDispachedEventHandler(DiscordEvent discordEvent);
    public delegate Task DiscordEventSendRequest(object eventToSend);
    public delegate Task DiscordEventReceivedEventHandler();

    public class DiscordEventParser
    {
        public event DiscordEventDispachedEventHandler OnEventDispached;
        public event DiscordEventReceivedEventHandler OnEventReceived;
        public event DiscordEventSendRequest OnEventSendRequested;
        
        //private Queue<string> eventsReceived = new();
        private Task eventDequeuerTask;
        private DiscordSocketClient socketClient;

        public DiscordEventParser(DiscordSocketClient discordSocketClient) {

            socketClient = discordSocketClient;

            OnEventSendRequested += SerializeAndSend;
            /*
            eventDequeuerTask = new Task(() => {
                while (true) {
                    if(eventsReceived.Count > 0) {
                        ParseEvent(eventsReceived.Dequeue());
                    }
                }
            });
            eventDequeuerTask.Start();
            */
        }

        public async Task SerializeAndSend(object eventToSend) {
            string serializedEvent = JsonSerializer.Serialize(eventToSend);
            await socketClient.SendAsync(serializedEvent);
        }

        public Task ParseEvent(string eventToParse) {
            if(eventToParse == null || eventToParse == string.Empty)
                throw new ArgumentNullException("Mensagem recebida do socket era Nula");

            DiscordEvent discordEvent = JsonSerializer.Deserialize<DiscordEvent>(eventToParse.Trim('\0'));
            OnEventDispached.Invoke(discordEvent);
            return Task.CompletedTask;
        }
        /*
        public Task GetEvent(string messageEvent) {
            eventsReceived.Enqueue(messageEvent);
            //OnEventReceived.Invoke();
            return Task.CompletedTask;
        }
        */
        public void Dispose() {
            eventDequeuerTask.Dispose();
        }
    }
}
