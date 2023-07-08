using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord {
    public class ClientConfiguration {
        public ClientConfiguration(string token, int apiVersion) { 
            this.token = token;
            this.apiVersion = apiVersion;
        }

        public string token { get; set; }
        public int apiVersion { get; set; }
    }
}
