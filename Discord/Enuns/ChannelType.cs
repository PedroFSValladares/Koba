using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Enuns {
    public enum ChannelType {
        GUILD_TEXT,
        DM,
        GUILD_VOICE,
        GROUP_DM,
        GUILD_CATEGORY,
        GUILD_ANNOUNCEMENT,
        ANNOUNCEMENT_THREAD = 10,
        PUBLIC_THREAD,
        PRIVATE_THREAD,
        GUILD_STAGE_VOICE,
        GUILD_DIRECTORY,
        GUILD_FORUM
    }
}
