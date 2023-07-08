using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Models {
    internal class Emoji {
        public string id { get; set; }
        public string? name { get; set; }
        public int[] roles { get; set; }
        public User? user { get; set; }
        public bool? require_colons { get; set; }
        public bool? managed { get; set; }
        public bool? animated { get; set; }
        public bool? available { get; set; }
    }
}
