using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Models {
    internal class Role {
        public int id { get; set; }
        public string name { get; set; }
        public int color { get; set; }
        public bool hoist { get; set; }
        public string? icon { get; set; }
        public string? unicode_emoji { get; set; }
        public int position { get; set; }
        public string permissions { get; set; }
        public bool managed { get; set; }
        public bool mentionable { get; set; }
        public RoleTags? tags { get; set; }
    }
}
