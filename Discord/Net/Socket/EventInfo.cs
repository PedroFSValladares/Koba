using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Net.Socket {
    public class EventInfo : EventArgs{
        public string Nome { get; set; }
        public dynamic EventData { get; set; }
    }
}
