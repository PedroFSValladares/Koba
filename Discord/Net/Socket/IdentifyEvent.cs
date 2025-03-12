using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Net.Socket {
    internal class IdentifyEvent {
        public string token { get; set; }
        public Intents intents { get; set; }
        public ConnectionProperties properties { get; set; }
    }

    class ConnectionProperties { 
        public string os { get; set; }
        public string browser { get; set; }
        public string device { get; set; }
    }
}
