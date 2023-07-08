using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Net.Socket {
    internal class HelloPayload {
        public new HelloPayloadEventData? d { get; set; }
    }

    public class HelloPayloadEventData{
        public int heartbeat_interval { get; set; }
    }
}
