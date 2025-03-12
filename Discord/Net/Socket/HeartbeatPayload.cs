using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Net.Socket {
    internal class HeartbeatPayload : GatewayPayload{
        public new int d { get; set; } 
    }
}
