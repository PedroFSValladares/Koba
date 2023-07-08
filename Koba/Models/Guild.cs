using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koba.Models {
    public class Guild {
        public int Id { get; set; }
        public string Name { get; set; }

        public GuildConfigs GuildConfigs { get; set; }
        public PlayList PlayLis { get; set; }
    }
}
