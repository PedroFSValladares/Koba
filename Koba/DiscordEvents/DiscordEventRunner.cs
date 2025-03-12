using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koba.DiscordEvents
{
    public abstract class DiscordEventRunner
    {
        public DiscordEvent EventData { get; set; }

        public DiscordEventRunner(DiscordEvent discordEvent) {
            EventData = discordEvent;
        }

        public abstract void Run();
    }
}
