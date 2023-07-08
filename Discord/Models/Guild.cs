using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Models {
    internal class Guild {
        public string id { get; set; }
        public string name { get; set; }
        public string icon { get; set; }
        public string splash { get; set; }
        public string discovery_splash { get; set; }
        public Emoji[] emojis { get; set; }
        public string[] features { get; set; }
        public int approximate_member_count { get; set; }
        public int approximate_presence_count { get; set; }
        public string description { get; set; }
    }

    internal class UnavaibleGuild {
        public bool enabled { get; set; }
        public string id { get; set; }
    }
}
