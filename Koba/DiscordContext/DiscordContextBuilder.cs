using Koba.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koba.DiscordContext
{
    public class DiscordContextBuilder
    {
        private DiscordUser botUser;

        public void SetBotUser(DiscordUser user) {
            this.botUser = user;
            botUser.CurrentUser = true;
        }
    }
}
