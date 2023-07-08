using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Net.Socket
{
    public enum Intents
    {
        GUILDS = 0,
        GUILD_MEMBERS = 1,
        GUILD_BANS = 2,
        GUILD_EMOJIS_AND_STICKERS = 3,
        GUILD_INTEGRATIONS = 4,
        GUILD_WEBHOOKS = 5,
        GUILD_INVITES = 6,
        GUILD_VOICE_STATES = 7,
        GUILD_PRESENCES = 8,
        GUILD_MESSAGES = 9,
        GUILD_MESSAGE_REACTIONS = 10,
        GUILD_MESSAGE_TYPING = 11,
        DIRECT_MESSAGE = 12,
        DIRECT_MESSAGE_REACTIONS = 13,
        DIRECT_MESSAGE_TYPING = 14,
        MESSAGE_CONTENT = 15,
        GUILD_SCHEDULED_EVENTS = 16,
        AUTO_MODERATION_CONFIGURATION = 20,
        AUTO_MODERATION_EXECUTION = 21
    }
}
