using Koba.DiscordEvents;
using Koba.Enuns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koba.EventHandler
{
    public delegate void DiscordEventReceivedEventHandler(DiscordEvent discordEvent);

    class DiscordEventHandler
    {
        public event DiscordEventReceivedEventHandler HelloEventReceived;
        public event DiscordEventReceivedEventHandler HeartbeatAckReceived;
        public event DiscordEventReceivedEventHandler UnknowEventReceived;
        public event DiscordEventReceivedEventHandler HeartbeatRequested;
        public event DiscordEventReceivedEventHandler ReadyEventReceived;
        private readonly DiscordEventParser discordEventParser;
        private readonly Dictionary<string, object> appCache;
        //private DiscordEventRunner eventRunner;

        public DiscordEventHandler(DiscordEventParser discordEventParser, Dictionary<string, object> appCache) {
            this.discordEventParser = discordEventParser;
            this.appCache = appCache;
        }

        public void ReceiveEvent(DiscordEvent discordEvent) {
            appCache["SequencialNumber"] = discordEvent.SequenceNumber;
            switch (discordEvent.OpCode) {
                case (int)DiscordEventOpCode.Ready:

                    switch (discordEvent.EventName) {
                        case "READY":
                            ReadyEventReceived.Invoke(discordEvent);
                            break;
                        default:
                            UnknowEventReceived.Invoke(discordEvent);
                            break;
                    }

                    break;
                case (int)DiscordEventOpCode.HeartBeat:
                    HeartbeatRequested.Invoke(discordEvent);
                    break;
                case (int)DiscordEventOpCode.Hello:
                    HelloEventReceived.Invoke(discordEvent);
                    break;
                case (int)DiscordEventOpCode.HeartbeatAck:
                    HeartbeatAckReceived.Invoke(discordEvent);
                    break;
                default:
                    UnknowEventReceived.Invoke(discordEvent);
                    break;
            }

            //eventRunner.Run();
        }
    }
}
