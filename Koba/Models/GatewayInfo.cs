using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Net.Rest {
    public class GatewayInfo {
        public string url { get; set; }
        public int shards { get; set; }
        public SessionInfo session_start_limit { get; set; }
    }
    public class SessionInfo {
        public int total { get; set; }
        public int remaining { get; set; }
        public int reset_after { get; set; }
        public int max_concurrency { get; set; }
    }
}
