using Koba.Core;
using Koba.DiscordEvents;
using Koba.BotSocket;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koba.Heart
{

    public delegate void HeartbeatEventHandler();

    public class HeartBeater
    {
        public event HeartbeatEventHandler HeartbeatTimedOut;
        public event HeartbeatEventHandler ConnectionSucceed;

        private Timer HeartbeatTimer;
        private readonly Dictionary<string, object> appCache;
        private readonly DiscordEventParser eventParser;
        private readonly ILogger logger;
        private int heartBeatTries = 0;
        private bool initialHeartbeat = true;

        private TimeOnly LastHeartbeatSended;
        private TimeOnly LastHeartbeatAcked;

        public HeartBeater(Dictionary<string, object> appCache, DiscordEventParser discordEventParser, ILogger logger) {
            this.appCache = appCache;
            this.eventParser = discordEventParser;
            this.logger = logger;
        }

        public void StartHeartbeat(DiscordEvent discordEvent) {
            discordEvent.EventData.RootElement.GetProperty("heartbeat_interval").TryGetInt32(out int interval);
            
            if(appCache.ContainsKey("SequencialNumber"))
                appCache["SequencialNumber"] = discordEvent.SequenceNumber;
            else
                appCache.Add("SequencialNumber", discordEvent.SequenceNumber);
            logger.LogInformation($"Iniciando HeartBeater com intervalo de aproximado de {interval/1000} segundos.");

            HeartbeatTimer = new Timer(new TimerCallback(Beat), null, 0, interval);
        }

        public void Beat(object state) {
            object heartBeat = new {
                op = 1,
                d = appCache["SequencialNumber"],
            };

            if(heartBeatTries > 3) {
                logger.LogError("Falha ao enviar Heartbeat. Tentativas esgotadas.");
                return;
            }

            try {
                new Task(async () => {
                    await eventParser.SerializeAndSend(heartBeat);
                    LastHeartbeatSended = TimeOnly.FromDateTime(DateTime.Now);
                    heartBeatTries++;
                }).Start();
            } catch (Exception ex) {
                logger.LogError("Falha ao enviar Heartbeat.");
                logger.LogError(ex.Message);
            }
        }

        public void Stop() {
            HeartbeatTimer.Change(0, Timeout.Infinite);
        }

        public void HeartBeatAckReceived(DiscordEvent discordEvent) {
            LastHeartbeatAcked = TimeOnly.FromDateTime(DateTime.Now);
            logger.LogInformation("Ciclo de Heartbeat executado. " +
                $"Tempo de resposta do sevidor: {(float)(LastHeartbeatAcked.Millisecond - LastHeartbeatSended.Millisecond)/1000} segundos.");
            heartBeatTries = 0;
            if(initialHeartbeat) {
                ConnectionSucceed.Invoke();
                initialHeartbeat = false;
            }
        }
    }
}
