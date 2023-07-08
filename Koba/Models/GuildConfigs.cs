using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koba.Models {
    public class GuildConfigs {
        public int id { get; set; }
        public string Prefix { get; set; }

        public Guild Guild { get; set; }
    }
}
