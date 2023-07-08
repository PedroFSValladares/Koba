using Discord.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Net.Socket.Events {
    internal class Ready : EventInfo{
        public int v { get; set; }
        public User user { get; set; }
        public UnavaibleGuild[] guilds { get; set; }
        public string session_id { get; set; }
        public string resume_gateway_url { get; set;}
        public int[] shard { get; set; }
        //implementar Application
    }
}
